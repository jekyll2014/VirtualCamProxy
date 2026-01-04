using FlashCap;

using OpenCvSharp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CameraLib.FlashCap;

public class UsbCameraFc : ICamera
{
    public CaptureDevice? CaptureDevice => _captureDevice;
    public CameraDescription Description { get; set; }
    public bool IsRunning { get; private set; } = false;
    public FrameFormat? CurrentFrameFormat { get; private set; }
    public double CurrentFps { get; private set; }
    public int FrameTimeout { get; set; } = 10000;
    public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;
    public event ICamera.CameraConnectedEventHandler? CameraConnectedEvent;
    public event ICamera.CameraDisconnectedEventHandler? CameraDisconnectedEvent;

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    private CancellationTokenSource? _cancellationTokenSource;
    private readonly CaptureDeviceDescriptor _usbCamera;
    private CaptureDevice? _captureDevice;
    private readonly object _getPictureThreadLock = new();
    private readonly Stopwatch _fpsTimer = new();
    private Mat? _frame = new Mat();
    private byte _frameCount;
    private readonly System.Timers.Timer _keepAliveTimer = new();
    private int _width = 0;
    private int _height = 0;
    private string _format = string.Empty;
    private bool _disposedValue;
    private bool _isReconnecting = false;

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

        Description = new CameraDescription(CameraType.USB_FC, path, name, GetAllAvailableResolution(_usbCamera).ToArray());
        CurrentFps = Description.FrameFormats.FirstOrDefault()?.Fps ?? 10;

        _keepAliveTimer.Elapsed += CheckCameraDisconnected;
    }

    public async Task<bool> GetImageDataAsync(int discoveryTimeout = 1000)
    {
        Description.FrameFormats = GetAllAvailableResolution(_usbCamera).ToArray();

        return Description.FrameFormats.Any();
    }

    private void CheckCameraDisconnected(object? sender, ElapsedEventArgs e)
    {
        if (_fpsTimer.ElapsedMilliseconds > FrameTimeout && !_isReconnecting)
        {
            _isReconnecting = true;
            _keepAliveTimer.Stop();

            _ = Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} Camera connection restarted ({_fpsTimer.ElapsedMilliseconds} timeout)");
                    Stop(false);
                    await StartAsync(_width, _height, _format, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Camera reconnection failed: {ex}");
                }
                finally
                {
                    _isReconnecting = false;
                    if (IsRunning)
                        _keepAliveTimer.Start();
                }
            });
        }
    }

    public List<CameraDescription> DiscoverCameras(int discoveryTimeout)
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
            var formats = GetAllAvailableResolution(camera).ToArray();
            result.Add(new CameraDescription(CameraType.USB_FC, camera.Identity.ToString() ?? string.Empty, camera.Name, formats));
        }

        return result;
    }

    private static List<FrameFormat> GetAllAvailableResolution(CaptureDeviceDescriptor usbCamera)
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

    public async Task<bool> StartAsync(int width, int height, string format, CancellationToken token)
    {
        if (IsRunning)
            return true;

        var cameraCharacteristics = GetCaptureDevice(width, height, format);
        if (cameraCharacteristics == null)
            return false;

        try
        {
            if (_captureDevice != null)
                await _captureDevice.DisposeAsync();

            _captureDevice = await _usbCamera.OpenAsync(cameraCharacteristics, OnPixelBufferArrived, token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting UsbCameraFC: {ex}");

            return false;
        }

        if (_captureDevice == null)
            return false;

        _width = width;
        _height = height;
        _format = format;
        CurrentFrameFormat = new FrameFormat(_width, _height, _format);

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        _fpsTimer.Reset();
        _frameCount = 0;
        _keepAliveTimer.Interval = FrameTimeout;
        _keepAliveTimer.Start();

        try
        {
            await _captureDevice.StartAsync(token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Camera Start() failed: {ex}");
            Stop();

            return false;
        }

        IsRunning = true;
        CameraConnectedEvent?.Invoke(this);

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
                .Where(n => n.Width == width && n.Height == height)
                .ToList();
        }
        else
        {
            characteristics = [
                characteristics.Aggregate((n, m) =>
                    {
                        if (n.Width * n.Height > m.Width * m.Height)
                            return n;
                        else
                            return m;
                    })
            ];
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
                _frame = Cv2.ImDecode(bufferScope.Buffer.ReferImage(), ImreadModes.Color);
                bufferScope.ReleaseNow();
                if (_frame == null || _frame.Empty())
                    return;

                //CurrentFrameFormat ??= new FrameFormat(_frame.Width, _frame.Height);
                ImageCapturedEvent?.Invoke(this, _frame.Clone());
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
    }

    public void Stop()
    {
        Stop(true);
    }

    private void Stop(bool cancellation)
    {
        if (!IsRunning)
            return;

        _keepAliveTimer.Stop();

        if (_captureDevice != null)
        {
            try
            {
                _captureDevice?.StopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera Stop() failed: {ex}");
            }
        }

        if (cancellation)
            _cancellationTokenSource?.Cancel();

        CurrentFrameFormat = null;
        _fpsTimer.Reset();
        IsRunning = false;
        CameraDisconnectedEvent?.Invoke(this);
    }

    public async Task<Mat?> GrabFrameAsync(CancellationToken token, int width = 0, int height = 0, string format = "")
    {
        if (IsRunning)
        {
            Mat? capturedFrame = null;
            void CameraImageCapturedEvent(ICamera camera, Mat image)
            {
                capturedFrame ??= image.Clone();
            }

            ImageCapturedEvent += CameraImageCapturedEvent;
            var watch = new Stopwatch();
            watch.Restart();
            while (IsRunning
                   && capturedFrame == null
                   && !token.IsCancellationRequested
                   && watch.ElapsedMilliseconds < FrameTimeout)
                await Task.Delay(10, token);

            ImageCapturedEvent -= CameraImageCapturedEvent;
            watch.Stop();

            return capturedFrame;
        }

        await Task.Run(async () =>
        {
            try
            {
                var cameraCharacteristics = GetCaptureDevice(width, height, format);
                if (cameraCharacteristics == null)
                    return;

                var imageData = await _usbCamera.TakeOneShotAsync(cameraCharacteristics, token);
                _frame = Cv2.ImDecode(imageData, ImreadModes.Color);
                CurrentFrameFormat ??= new FrameFormat(_frame?.Width ?? 0, _frame?.Height ?? 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }, token);

        return _frame;
    }

    public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var image = await GrabFrameAsync(token);
            if (image == null)
                await Task.Delay(10, CancellationToken.None);
            else
                yield return image;
        }
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
