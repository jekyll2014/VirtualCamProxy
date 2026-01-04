using DirectShowLib;

using OpenCvSharp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CameraLib.USB;

public class UsbCamera : ICamera
{
    private const int FpsCalculationFrameCount = 100;
    private const int FrameCheckDelayMs = 10;

    public VideoCapture? CaptureDevice => _captureDevice;
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
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
    private readonly DsDevice _usbCamera;
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
    private bool _disposedValue;
    private bool _isReconnecting = false;

    public UsbCamera(string path, string name = "")
    {
        var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice) ?? [];
        _usbCamera = devices.FirstOrDefault(n => n.DevicePath == path)
                     ?? throw new ArgumentException("Can not find camera", nameof(path));

        if (string.IsNullOrEmpty(name))
            name = _usbCamera.Name;

        if (string.IsNullOrEmpty(name))
            name = path;

        Description = new CameraDescription(CameraType.USB, path, name, GetAllAvailableResolution(_usbCamera).ToArray());
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
        var descriptors = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        var result = new List<CameraDescription>();
        foreach (var camera in descriptors)
        {
            var formats = GetAllAvailableResolution(camera).ToArray();
            result.Add(new CameraDescription(CameraType.USB, camera.DevicePath, camera.Name, formats));
        }

        return result;
    }

    private static List<FrameFormat> GetAllAvailableResolution(DsDevice usbCamera)
    {
        try
        {
            var bitCount = 0;
            var availableResolutions = new List<FrameFormat>();

            if (!(new FilterGraph() is IFilterGraph2 mFilterGraph2))
                return availableResolutions;

            mFilterGraph2.AddSourceFilterForMoniker(usbCamera.Mon, null, usbCamera.Name, out var sourceFilter);

            var pRaw2 = DsFindPin.ByCategory(sourceFilter, PinCategory.Capture, 0);
            if (pRaw2 == null)
                return availableResolutions;

            var videoInfoHeader = new VideoInfoHeader();
            pRaw2.EnumMediaTypes(out var mediaTypeEnum);

            var mediaTypes = new AMMediaType[1];
            var fetched = IntPtr.Zero;
            mediaTypeEnum.Next(1, mediaTypes, fetched);

            while (mediaTypes[0] != null)
            {
                Marshal.PtrToStructure(mediaTypes[0].formatPtr, videoInfoHeader);
                var header = videoInfoHeader.BmiHeader;
                if (header.Size != 0 && header.BitCount != 0)
                {
                    if (header.BitCount > bitCount)
                    {
                        availableResolutions.Clear();
                        bitCount = header.BitCount;
                    }

                    FrameFormat.Codecs.TryGetValue(header.Compression, out var format);
                    availableResolutions.Add(new FrameFormat(
                        header.Width,
                        header.Height,
                        format ?? string.Empty,
                        (double)10000000 / videoInfoHeader.AvgTimePerFrame));
                }

                mediaTypeEnum.Next(1, mediaTypes, fetched);
            }

            return availableResolutions;
        }

        catch (Exception)
        {
            return new List<FrameFormat>();
        }
    }

    public async Task<bool> StartAsync(int width, int height, string format, CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);

        if (width < 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width cannot be negative");

        if (height < 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height cannot be negative");

        if (IsRunning)
            return true;

        try
        {
            _captureDevice?.Dispose();
            _captureDevice = await GetCaptureDevice(token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting UsbCamera: {ex}");

            return false;
        }

        if (_captureDevice == null)
            return false;

        if (width > 0 && height > 0)
        {
            var res = GetAllAvailableResolution(_usbCamera);
            if (res.Exists(n => n.Width == width && n.Height == height))
            {
                _captureDevice.Set(VideoCaptureProperties.FrameWidth, width);
                _captureDevice.Set(VideoCaptureProperties.FrameHeight, height);
            }

            if (!string.IsNullOrEmpty(format))
            {
                var codecId = FrameFormat.Codecs.FirstOrDefault(n => n.Value == format);
                if (!string.IsNullOrEmpty(codecId.Value))
                    _captureDevice.Set(VideoCaptureProperties.FourCC, codecId.Key);
            }
        }

        _width = width;
        _height = height;
        _format = format;
        CurrentFrameFormat = new FrameFormat(_width, _height, _format);

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
                        await Task.Delay(FrameCheckDelayMs, _cancellationTokenSourceCameraGrabber.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera Start() failed: {ex}");
                Stop();
            }

            IsRunning = false;

        }, _cancellationTokenSourceCameraGrabber.Token);

        IsRunning = true;
        CameraConnectedEvent?.Invoke(this);

        return true;
    }

    private async Task<VideoCapture?> GetCaptureDevice(CancellationToken token)
    {
        var cameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice) ?? [];
        var camNumber = cameras.TakeWhile(cam => _usbCamera?.DevicePath != cam.DevicePath).Count();

        if (cameras.Length == 0 || camNumber >= cameras.Length)
            return null;

        return await Task.Run(() => new VideoCapture(camNumber, VideoCaptureAPIs.DSHOW), token);
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
            if (_frameCount >= FpsCalculationFrameCount)
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
            _cancellationTokenSourceCameraGrabber?.Cancel();
            try
            {
                //_captureTask?.Wait();
                _captureDevice?.Release();
                _captureDevice?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera Stop() failed: {ex}");
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
        CameraDisconnectedEvent?.Invoke(this);
    }

    public async Task<Mat?> GrabFrameAsync(CancellationToken token, int width = 0, int height = 0, string format = "")
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);

        if (width < 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width cannot be negative");

        if (height < 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height cannot be negative");

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
                await Task.Delay(FrameCheckDelayMs, token);

            ImageCapturedEvent -= CameraImageCapturedEvent;
            watch.Stop();

            return capturedFrame;
        }

        await Task.Run(async () =>
        {
            try
            {
                _captureDevice = await GetCaptureDevice(token);
                if (_captureDevice == null)
                    return;

                if (width > 0 && height > 0)
                {
                    var res = GetAllAvailableResolution(_usbCamera);
                    if (res.Exists(n => n.Width == width && n.Height == height))
                    {
                        _captureDevice.Set(VideoCaptureProperties.FrameWidth, width);
                        _captureDevice.Set(VideoCaptureProperties.FrameHeight, height);
                    }

                    if (!string.IsNullOrEmpty(format))
                    {
                        var codecId = FrameFormat.Codecs.FirstOrDefault(n => n.Value == format);
                        if (!string.IsNullOrEmpty(codecId.Value))
                            _captureDevice.Set(VideoCaptureProperties.FourCC, codecId.Key);
                    }
                }

                if (_captureDevice.Grab())
                {
                    _frame ??= new Mat();
                    if (_captureDevice.Retrieve(_frame) && !_frame.Empty())
                        CurrentFrameFormat ??= new FrameFormat(_frame.Width, _frame.Height);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            _captureDevice?.Release();
            _captureDevice?.Dispose();
        }, token);

        return _frame;
    }

    public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);

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
                _usbCamera?.Dispose();
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
