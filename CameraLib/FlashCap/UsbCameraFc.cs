using FlashCap;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using OpenCvSharp;

namespace CameraLib.FlashCap
{
    public class UsbCameraFc : ICamera
    {
        public CameraDescription Description { get; set; }
        public bool IsRunning { get; private set; }
        public FrameFormat? CurrentFrameFormat { get; private set; }
        public double CurrentFps { get; private set; }
        public int FrameTimeout { get; set; } = 10000;

        public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;

        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly CaptureDeviceDescriptor _usbCamera;
        private CaptureDevice? _captureDevice;
        private readonly object _getPictureThreadLock = new();
        private readonly Stopwatch _fpsTimer = new();
        private byte _frameCount;

        private readonly System.Timers.Timer _keepAliveTimer = new System.Timers.Timer();
        private int _width = 0;
        private int _height = 0;
        private string _format = string.Empty;
        private CancellationToken _token = CancellationToken.None;
        private int _gcCounter = 0;

        private bool _disposedValue;

        public UsbCameraFc(string path, string name = "")
        {
            var devices = new CaptureDevices();
            var descriptors = devices
                .EnumerateDescriptors()
                .Where(d => d.Characteristics.Length >= 1);             // One or more valid video characteristics.

            _usbCamera = descriptors.FirstOrDefault(n => n.Identity?.ToString() == path)
                         ?? throw new ArgumentException("Can not find camera", nameof(path));

            if (string.IsNullOrEmpty(name))
                name = _usbCamera.Name;
            if (string.IsNullOrEmpty(name))
                name = path;

            Description = new CameraDescription(CameraType.USB_FC, path, name, GetAllAvailableResolution(_usbCamera));
            CurrentFps = Description.FrameFormats.FirstOrDefault()?.Fps ?? 10;

            _keepAliveTimer.Elapsed += CameraDisconnected;
        }

        private async void CameraDisconnected(object? sender, ElapsedEventArgs e)
        {
            if (_fpsTimer.ElapsedMilliseconds > FrameTimeout)
            {
                Console.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} Camera connection restarted ({_fpsTimer.ElapsedMilliseconds} timeout)");
                Stop(false);
                await Start(_width, _height, _format, _token);
            }
        }

        public List<CameraDescription> DiscoverCamerasAsync(int discoveryTimeout, CancellationToken token)
        {
            return DiscoverUsbCameras();
        }

        public static List<CameraDescription> DiscoverUsbCameras()
        {
            var devices = new CaptureDevices();
            var descriptors = devices
                .EnumerateDescriptors()
                .Where(d => d.Characteristics.Length > 0 && d.DeviceType == DeviceTypes.DirectShow);             // One or more valid video characteristics.

            var result = new List<CameraDescription>();
            foreach (var camera in descriptors)
            {
                var formats = GetAllAvailableResolution(camera);
                result.Add(new CameraDescription(CameraType.USB, camera.Identity.ToString() ?? string.Empty, camera.Name, formats));
            }

            return result;
        }

        private static IEnumerable<FrameFormat> GetAllAvailableResolution(CaptureDeviceDescriptor usbCamera)
        {
            var formats = new List<FrameFormat>();
            foreach (var cameraCharacteristic in usbCamera.Characteristics)
            {
                formats.Add(new FrameFormat(cameraCharacteristic.Width,
                    cameraCharacteristic.Height,
                    cameraCharacteristic.PixelFormat.ToString(),
                    (double)cameraCharacteristic.FramesPerSecond.Numerator / cameraCharacteristic.FramesPerSecond.Denominator));
            }

            return formats;
        }

        public async Task<bool> Start(int width, int height, string format, CancellationToken token)
        {
            if (IsRunning)
                return true;

            var cameraCharacteristics = GetCaptureDevice(width, height, format);
            if (cameraCharacteristics == null)
                return false;

            _captureDevice = await _usbCamera.OpenAsync(cameraCharacteristics, OnPixelBufferArrived, token);
            if (_captureDevice == null)
            {
                IsRunning = false;

                return false;
            }

            _width = width;
            _height = height;
            _format = format;
            _token = token;

            _cancellationTokenSource = new CancellationTokenSource();
            _frameCount = 0;
            _fpsTimer.Reset();
            _keepAliveTimer.Interval = FrameTimeout;
            _keepAliveTimer.Start();

            await _captureDevice.StartAsync(token);

            IsRunning = true;

            return true;
        }

        private VideoCharacteristics? GetCaptureDevice(int width, int height, string format)
        {
            var characteristics = _usbCamera.Characteristics
                .Where(n => n.PixelFormat != PixelFormats.Unknown);

            if (!string.IsNullOrEmpty(format))
                characteristics = _usbCamera.Characteristics
                    .Where(n => n.PixelFormat.ToString() == format);

            if (width > 0 && height > 0)
            {
                characteristics = characteristics
                    .Where(n => n.Width == width && n.Height == height).ToList();
            }
            else
            {
                characteristics = new List<VideoCharacteristics>(){
                    characteristics.Aggregate((n, m) =>
                    {
                        if (n.Width * n.Height > m.Width * m.Height)
                            return n;
                        else
                            return m;
                    })};
            }

            return characteristics.FirstOrDefault();
        }

        private void OnPixelBufferArrived(PixelBufferScope bufferScope)
        {
            if (Monitor.IsEntered(_getPictureThreadLock))
            {
                bufferScope.ReleaseNow();

                return;
            }

            lock (_getPictureThreadLock)
            {
                try
                {
                    var imageBuffer = bufferScope.Buffer.CopyImage();
                    bufferScope.ReleaseNow();
                    var frame = Cv2.ImDecode(imageBuffer, ImreadModes.Color);
                    if (frame == null)
                        return;

                    CurrentFrameFormat ??= new FrameFormat(frame.Width, frame.Height);

                    ImageCapturedEvent?.Invoke(this, frame);

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
                }
                catch
                {
                    Stop();
                }
                finally
                {
                    _gcCounter++;
                    if (_gcCounter >= 10)
                    {
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                        _gcCounter = 0;
                    }
                }
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

            lock (_getPictureThreadLock)
            {
                _keepAliveTimer.Stop();

                if (cancellation)
                    _cancellationTokenSource?.Cancel();

                if (_captureDevice != null)
                {
                    try
                    {
                        _captureDevice.StopAsync().Wait(5000);
                    }
                    catch { }
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
                Mat? frame = null;
                ImageCapturedEvent += CameraImageCapturedEvent;
                while (IsRunning && frame == null && !token.IsCancellationRequested)
                    await Task.Delay(10, token);

                ImageCapturedEvent -= CameraImageCapturedEvent;

                return frame;

                void CameraImageCapturedEvent(ICamera camera, Mat image)
                {
                    frame = image?.Clone();
                }
            }

            Mat? image = null;
            await Task.Run(async () =>
            {
                try
                {
                    var cameraCharacteristics = GetCaptureDevice(0, 0, string.Empty);
                    if (cameraCharacteristics == null)
                        return;

                    var imageData = await _usbCamera.TakeOneShotAsync(cameraCharacteristics, token);
                    image = Cv2.ImDecode(imageData, ImreadModes.Color);
                    CurrentFrameFormat ??= new FrameFormat(image?.Width ?? 0, image?.Height ?? 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }, token);

            return image;
        }

        public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var image = await GrabFrame(token);
                if (image == null)
                    await Task.Delay(10, token);
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
                    _keepAliveTimer.Close();
                    _keepAliveTimer.Dispose();
                    _cancellationTokenSource?.Dispose();
                    _captureDevice?.Dispose();
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
