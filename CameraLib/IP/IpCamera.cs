﻿using OpenCvSharp;

using QuickNV.Onvif;
using QuickNV.Onvif.Discovery;
using QuickNV.Onvif.Media;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using IPAddress = System.Net.IPAddress;

namespace CameraLib.IP
{
    public class IpCamera : ICamera
    {
        public AuthType AuthenticationType { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }
        public CameraDescription Description { get; set; }
        public bool IsRunning { get; private set; } = false;
        public FrameFormat? CurrentFrameFormat { get; private set; }
        public double CurrentFps { get; private set; }
        public int FrameTimeout { get; set; } = 30000;
        public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;
        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
        private static List<CameraDescription> _lastCamerasFound = [];
        private readonly object _getPictureThreadLock = new();
        private VideoCapture? _captureDevice;
        private Task? _captureTask;
        private readonly Stopwatch _fpsTimer = new();
        private Mat? _frame = new Mat();
        private byte _frameCount;
        private readonly System.Timers.Timer _keepAliveTimer = new();
        private int _width = 0;
        private int _height = 0;
        private string _format = string.Empty;
        private byte _gcCounter = 0;
        private bool _disposedValue;

        public static async Task<List<CameraDescription>> DiscoverOnvifCamerasAsync(int discoveryTimeout)
        {
            var result = new List<CameraDescription>();
            var discovery = new DiscoveryController2(TimeSpan.FromMilliseconds(discoveryTimeout));
            var devices = await discovery.RunDiscovery();
            Console.WriteLine($"Found {devices.Length} cameras");

            if (devices.Length == 0)
            {
                return result;
            }

            foreach (var device in devices)
            {
                Console.WriteLine($"Detecting media size: {device.ServiceAddresses[0]}");

                var uri = new Uri(device.ServiceAddresses[0]);
                var client = new OnvifClient(new OnvifClientOptions
                {
                    Scheme = uri.Scheme,
                    Host = uri.Host,
                    Port = uri.Port
                });

                try
                {
                    await client.ConnectAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Can not connect to camera: {uri}\r\n{ex}");
                }

                var mediaClient = new MediaClient(client);
                var profilesResponse = await mediaClient.GetProfilesAsync();

                foreach (var profile in profilesResponse.Profiles)
                {
                    var stream = await mediaClient.QuickOnvif_GetStreamUriAsync(profile.token, true);
                    result.Add(new CameraDescription(
                        CameraType.IP,
                        stream,
                        $"{client.DeviceInformation.Manufacturer} {client.DeviceInformation.Model} [{device.EndPointAddress}]",
                        [
                                new(profile.VideoEncoderConfiguration.Resolution.Width,
                                    profile.VideoEncoderConfiguration.Resolution.Height,
                                    profile.VideoEncoderConfiguration.Encoding.ToString(),
                                    profile.VideoEncoderConfiguration.RateControl.FrameRateLimit)
                        ]));
                }
            }

            _lastCamerasFound = result;

            return result;
        }

        private static async Task<bool> PingAddress(string host, int pingTimeout = 5000)
        {
            if (!IPAddress.TryParse(host, out var destIp))
            {
                var h = await Dns.GetHostEntryAsync(host).ConfigureAwait(false);
                if (h.AddressList.Length > 0)
                    host = h.AddressList[0].ToString();

                if (!IPAddress.TryParse(host, out destIp))
                    return false;
            }

            PingReply pingResultTask;
            using (var ping = new Ping())
            {
                pingResultTask = await ping.SendPingAsync(destIp, pingTimeout).ConfigureAwait(true);
            }

            return pingResultTask.Status == IPStatus.Success;
        }

        public IpCamera(string path,
            string name = "",
            AuthType authenticationType = AuthType.None,
            string login = "",
            string password = "")
        {
            AuthenticationType = authenticationType;
            Login = login;
            Password = password;

            if (AuthenticationType == AuthType.Plain)
                path = string.Format(path, Login, Password);

            name = string.IsNullOrEmpty(name)
                ? Dns.GetHostAddresses(new Uri(path).Host).FirstOrDefault()?.ToString() ?? path
                : name;

            var frameFormats = _lastCamerasFound?
                .Find(n => n.Path == path)?
                .FrameFormats.ToArray() ?? [];

            Description = new CameraDescription(CameraType.IP, path, name, frameFormats);
            _keepAliveTimer.Elapsed += CheckCameraDisconnected;
        }

        public async Task<bool> GetImageData(int discoveryTimeout = 5000)
        {
            var cameraUri = new Uri(Description.Path);
            if (await PingAddress(cameraUri.Host, discoveryTimeout))
            {
                var image = await GrabFrame(CancellationToken.None);
                if (image != null)
                {
                    Description.FrameFormats = new[] { new FrameFormat(image.Width, image.Height) };
                    image.Dispose();
                }
                else
                    return false;
            }
            else
                return false;

            return Description.FrameFormats.Any();
        }

        private async void CheckCameraDisconnected(object? sender, ElapsedEventArgs e)
        {
            if (_fpsTimer.ElapsedMilliseconds > FrameTimeout)
            {
                Console.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} Camera connection restarted ({_fpsTimer.ElapsedMilliseconds} timeout)");
                Stop(false);
                await Start(_width, _height, _format, CancellationToken.None);
            }
        }

        public List<CameraDescription> DiscoverCameras(int discoveryTimeout)
        {
            return DiscoverOnvifCamerasAsync(discoveryTimeout).Result;
        }

        public async Task<bool> Start(int width, int height, string format, CancellationToken token)
        {
            if (IsRunning)
                return true;

            try
            {
                _captureDevice?.Dispose();
                _captureDevice = await GetCaptureDevice(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return false;
            }

            if (_captureDevice == null)
                return false;

            _width = width;
            _height = height;
            _format = format;

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSourceCameraGrabber?.Dispose();
            _cancellationTokenSourceCameraGrabber = new CancellationTokenSource();
            _captureDevice.SetExceptionMode(false);
            _fpsTimer.Reset();
            _frameCount = 0;
            _keepAliveTimer.Interval = FrameTimeout;
            _keepAliveTimer.Start();

            _captureTask?.Dispose();
            _captureTask = Task.Run(async () =>
            {
                try
                {
                    while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
                    {
                        if (_captureDevice?.Grab() ?? false)
                            CaptureImage();
                        else
                            await Task.Delay(10, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting image from camera: {ex}");
                    Stop();
                }

                IsRunning = false;
            }, _cancellationTokenSourceCameraGrabber.Token);

            IsRunning = true;

            return true;
        }

        private async Task<VideoCapture?> GetCaptureDevice(CancellationToken token)
        {
            try
            {
                var t = Task.Run(() => new VideoCapture(Description.Path), token);
                await t.WaitAsync(TimeSpan.FromMilliseconds(FrameTimeout), token);

                return t.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can not connect to camera: {Description.Path}\r\n{ex}");
            }

            return null;
        }

        private void CaptureImage()
        {
            if (Monitor.IsEntered(_getPictureThreadLock))
                return;

            lock (_getPictureThreadLock)
            {
                try
                {
                    _frame ??= new Mat();
                    if (!(_captureDevice?.Retrieve(_frame) ?? false) || _frame.Empty())
                        return;

                    CurrentFrameFormat ??= new FrameFormat(_frame.Width, _frame.Height);
                    if (!Description.FrameFormats.Any()
                        || (Description.FrameFormats.Count() == 1
                            && Description.FrameFormats.First().Width == 0))
                        Description.FrameFormats = new[] { new FrameFormat(_frame.Width, _frame.Height) };

                    ImageCapturedEvent?.Invoke(this, _frame);
                }
                catch
                {
                    Stop();
                }
            }

            if (!_fpsTimer.IsRunning)
            {
                _fpsTimer.Start();
                _frameCount = 0;
            }
            else
            {
                _frameCount++;
                if (_frameCount >= 100)
                {
                    if (_fpsTimer.ElapsedMilliseconds > 0)
                        CurrentFps = (double)_frameCount / ((double)_fpsTimer.ElapsedMilliseconds / (double)1000);

                    _fpsTimer.Reset();
                    _frameCount = 0;
                }
            }

            _gcCounter++;
            if (_gcCounter >= 100)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                _gcCounter = 0;
            }
        }

        public void Stop()
        {
            Stop(true);
        }

        private void Stop(bool cancellation)
        {
            if (!IsRunning)
                return;

            //lock (_getPictureThreadLock)
            {
                _keepAliveTimer.Stop();

                if (_captureDevice != null)
                {
                    _cancellationTokenSourceCameraGrabber?.Cancel();
                    try
                    {
                        _captureTask?.Wait(5000);
                        _captureDevice?.Release();
                        _captureDevice?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error releasing camera: {ex}");
                    }
                }

                if (cancellation)
                {
                    _cancellationTokenSourceCameraGrabber?.Cancel();
                    _cancellationTokenSource?.Cancel();
                }

                CurrentFrameFormat = null;
                _fpsTimer.Reset();
                IsRunning = false;
            }
        }

        public async Task<Mat?> GrabFrame(CancellationToken token)
        {
            if (IsRunning)
            {
                ImageCapturedEvent += CameraImageCapturedEvent;
                var watch = new Stopwatch();
                watch.Restart();
                while (IsRunning
                       && (_frame == null || _frame.Empty())
                       && !token.IsCancellationRequested
                       && watch.ElapsedMilliseconds < FrameTimeout)
                    await Task.Delay(10, CancellationToken.None);

                ImageCapturedEvent -= CameraImageCapturedEvent;
                watch.Stop();

                return _frame;

                void CameraImageCapturedEvent(ICamera camera, Mat image)
                {
                    _frame = image;
                }
            }

            await Task.Run(async () =>
            {
                try
                {
                    _captureDevice = await GetCaptureDevice(token);
                    if (_captureDevice == null)
                        return;

                    if (_captureDevice.Grab())
                    {
                        _frame ??= new Mat();
                        if (_captureDevice.Retrieve(_frame) && !_frame.Empty())
                        {
                            CurrentFrameFormat ??= new FrameFormat(_frame.Width, _frame.Height);
                            if (!Description.FrameFormats.Any()
                                || (Description.FrameFormats.Count() == 1
                                    && Description.FrameFormats.First().Width == 0))
                                Description.FrameFormats = new[] { new FrameFormat(_frame.Width, _frame.Height) };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                _captureDevice?.Release();
                _captureDevice?.Dispose();
            }, token);

            return _frame;
        }

        public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var image = await GrabFrame(token);
                if (image == null)
                    await Task.Delay(10, CancellationToken.None);
                else
                    yield return image;
            }
        }

        public FrameFormat GetNearestFormat(int width, int height, string format)
        {
            FrameFormat? selectedFormat;

            if (!Description.FrameFormats.Any())
                return new FrameFormat(0, 0);

            if (Description.FrameFormats.Count() == 1)
                return Description.FrameFormats.First();

            if (width > 0 && height > 0)
            {
                var mpix = width * height;
                selectedFormat = Description.FrameFormats.MinBy(n => Math.Abs(n.Width * n.Height - mpix));
            }
            else
                selectedFormat = Description.FrameFormats.MaxBy(n => n.Width * n.Height);

            var result = Description.FrameFormats
                .Where(n =>
                    n.Width == (selectedFormat?.Width ?? 0)
                    && n.Height == (selectedFormat?.Height ?? 0))
                .ToArray();

            if (result.Length != 0)
            {
                var result2 = result.Where(n => n.Format == format)
                    .ToArray();

                if (result2.Length != 0)
                    result = result2;
            }

            if (result.Length == 0)
                return new FrameFormat(0, 0);

            var result3 = result.MaxBy(n => n.Fps) ?? result[0];

            return result3;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    _keepAliveTimer.Elapsed -= CheckCameraDisconnected;
                    _keepAliveTimer.Close();
                    _keepAliveTimer.Dispose();
                    _captureDevice?.Dispose();
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSourceCameraGrabber?.Dispose();
                    _frame?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
