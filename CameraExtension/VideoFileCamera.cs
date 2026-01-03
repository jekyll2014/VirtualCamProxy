using CameraLib;

using OpenCvSharp;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

using Timer = System.Timers.Timer;

namespace CameraExtension;

public class VideoFileCamera : ICamera
{
    public CameraDescription Description { get; set; }
    public bool IsRunning { get; private set; } = false;
    public FrameFormat? CurrentFrameFormat { get; private set; }
    public double CurrentFps { get; private set; }
    public int FrameTimeout { get; set; } = 10000;
    public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;
    public event ICamera.CameraConnectedEventHandler? CameraConnectedEvent;
    public event ICamera.CameraDisconnectedEventHandler? CameraDisconnectedEvent;

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
    public bool RepeatFile = true;

    private const string CameraName = "Video file(s)";
    private const int FpsCalculationFrameCount = 100;
    private const int FrameCheckDelayMs = 10;

    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
    private readonly object _getPictureThreadLock = new();
    private Task? _captureTask;
    private readonly Stopwatch _fpsTimer = new();
    private Mat? _frame = new Mat();
    private byte _frameCount;
    private readonly List<string> _fileNames = [];
    private int _fileIndex = 0;
    private VideoCapture? _videoFile;
    private int _stepToNextFile = 0;
    private int _delay = 100;
    private readonly Timer _keepAliveTimer = new();
    private string _format = string.Empty;
    private CancellationToken _token = CancellationToken.None;
    private bool _disposedValue;
    private bool _isReconnecting = false;

    public VideoFileCamera(string path, string name = "")
    {
        if (string.IsNullOrEmpty(name))
            name = CameraName;

        Description = new CameraDescription(CameraType.VideoFile,
            path,
            name,
            GetFileResolution(path).ToArray());
        CurrentFps = 0;

        _keepAliveTimer.Elapsed += CameraDisconnected;
    }

    public async Task<bool> GetImageDataAsync(int discoveryTimeout = 1000)
    {
        if (_videoFile == null)
            return false;

        Description.FrameFormats = GetFileResolution(_videoFile).ToArray();

        return Description.FrameFormats.Any();
    }

    public void SetFile(string path)
    {
        SetFile([path]);
    }

    public void SetFile(List<string> paths)
    {
        _videoFile?.Dispose();
        _fileNames.Clear();
        _fileNames.AddRange(paths);
        _fileIndex = 0;
        if (_fileNames.Count != 0)
        {
            var file = _fileNames.FirstOrDefault() ?? "";
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                return;

            _videoFile = new VideoCapture(file)
            {
                ConvertRgb = true
            };

            Description.Path = file;
            Description.FrameFormats = GetFileResolution(_videoFile).ToArray();
            CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
            CurrentFps = CurrentFrameFormat?.Fps ?? 0;
            _format = CurrentFrameFormat?.Format ?? string.Empty;
            _delay = (int)(1000 / (_videoFile?.Fps ?? 25));
        }
    }

    public void StepToNextFile()
    {
        Interlocked.Increment(ref _stepToNextFile);
    }

    private bool SetNextFile(bool repeat)
    {
        if (_stepToNextFile > 0)
            Interlocked.Decrement(ref _stepToNextFile);

        _videoFile?.Dispose();

        if (_fileNames.Count == 0)
            return false;

        if (_fileIndex < _fileNames.Count - 1)
            _fileIndex++;
        else if (repeat)
            _fileIndex = 0;
        else
            return false;

        var file = _fileNames[_fileIndex];
        if (string.IsNullOrEmpty(file) || !File.Exists(file))
            return false;

        _videoFile = new VideoCapture(file)
        {
            ConvertRgb = true
        };

        Description.Path = file;
        Description.FrameFormats = GetFileResolution(_videoFile).ToArray();
        CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
        CurrentFps = CurrentFrameFormat?.Fps ?? 0;
        _format = CurrentFrameFormat?.Format ?? string.Empty;
        _delay = (int)(1000 / (_videoFile?.Fps ?? 25));

        return true;
    }

    private void CameraDisconnected(object? sender, ElapsedEventArgs e)
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
                    await StartAsync(0, 0, _format, _token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Video file camera reconnection failed: {ex}");
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
        return DiscoverScreenCameras();
    }

    public static List<CameraDescription> DiscoverScreenCameras()
    {
        var result = new List<CameraDescription>();

        return result;
    }

    private static List<FrameFormat> GetFileResolution(string path)
    {
        VideoCapture? videoFile = null;
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                videoFile = new VideoCapture(path);
            }
            catch
            {
                // Ignore if the file is not readable
            }
        }

        return GetFileResolution(videoFile);
    }

    private static List<FrameFormat> GetFileResolution(VideoCapture? videoFile)
    {
        return
                [
                    new FrameFormat(videoFile?.FrameWidth??0, videoFile?.FrameHeight??0, $"{videoFile?.FourCC}", videoFile?.Fps??0)
                ];
    }

    public async Task<bool> StartAsync(int width, int height, string format, CancellationToken token)
    {
        if (IsRunning)
            return true;

        _token = token;

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSourceCameraGrabber?.Dispose();
        _cancellationTokenSourceCameraGrabber = new CancellationTokenSource();
        _fpsTimer.Reset();
        _frameCount = 0;
        _keepAliveTimer.Interval = FrameTimeout;
        _keepAliveTimer.Start();

        _captureTask?.Dispose();
        _captureTask = Task.Run(async () =>
        {
            _delay = (int)(1000 / (_videoFile?.Fps ?? 25));
            var nextFrameTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
            {
                var now = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                if (now >= nextFrameTime)
                {
                    nextFrameTime += _delay;
                    if ((_videoFile?.Grab() ?? false) && _stepToNextFile <= 0)
                        CaptureImage();
                    else if (!SetNextFile(RepeatFile))
                        await _cancellationTokenSourceCameraGrabber.CancelAsync();
                }
                else
                    await Task.Delay(FrameCheckDelayMs, _cancellationTokenSourceCameraGrabber.Token);
            }

            Stop(true);
        }, _cancellationTokenSourceCameraGrabber.Token);

        IsRunning = true;
        CameraConnectedEvent?.Invoke(this);

        return true;
    }

    private void CaptureImage()
    {
        if (Monitor.IsEntered(_getPictureThreadLock))
            return;

        try
        {
            lock (_getPictureThreadLock)
            {
                if (_frame == null)
                    _frame = new Mat();

                if (!(_videoFile?.Retrieve(_frame) ?? false) || (_frame == null || _frame.Empty()))
                    return;

                CurrentFrameFormat ??= new FrameFormat(_frame.Width, _frame.Height);
                ImageCapturedEvent?.Invoke(this, _frame.Clone());
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
        catch
        {
            Stop();
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

        if (_videoFile != null)
        {
            _cancellationTokenSourceCameraGrabber?.Cancel();
            try
            {
                //_captureTask?.Wait();
                _videoFile?.Release();
                _videoFile?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error releasing camera: {ex}");
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

                if (_videoFile == null)
                    SetFile(_fileNames);

                if (_videoFile?.Grab() ?? false)
                {
                    _frame ??= new Mat();
                    _videoFile?.Retrieve(_frame);
                }
                else if (RepeatFile)
                {
                    SetNextFile(true);
                    if (_videoFile?.Grab() ?? false)
                    {
                        _frame ??= new Mat();
                        _videoFile?.Retrieve(_frame);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            _videoFile?.Release();
            _videoFile?.Dispose();
        }, token);

        return _frame;
    }

    public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var image = await GrabFrameAsync(token);
            if (image == null)
                await Task.Delay(FrameCheckDelayMs, token);
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
                _cancellationTokenSourceCameraGrabber?.Dispose();
                _cancellationTokenSource?.Dispose();
                _videoFile?.Dispose();
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