using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

using CameraLib;

using OpenCvSharp;

using Timer = System.Timers.Timer;

namespace CameraExtension;

public class VideoFileCamera : ICamera
{
    public CameraDescription Description { get; set; }
    public bool IsRunning { get; private set; }
    public FrameFormat? CurrentFrameFormat { get; private set; }
    public double CurrentFps { get; private set; }
    public int FrameTimeout { get; set; } = 10000;
    public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;
    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
    public bool RepeatFile = true;

    private const string CameraName = "Video file(s)";
    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
    private readonly object _getPictureThreadLock = new();
    private Task? _captureTask;
    private readonly Stopwatch _fpsTimer = new();
    private byte _frameCount;
    private readonly List<string> _fileNames = [];
    private int _fileIndex = 0;
    private VideoCapture? _videoFile;
    private int _delay = 100;
    private readonly Timer _keepAliveTimer = new();
    private string _format = string.Empty;
    private CancellationToken _token = CancellationToken.None;
    private int _gcCounter = 0;
    private bool _disposedValue;

    public VideoFileCamera(string path, string name = "")
    {
        if (string.IsNullOrEmpty(name))
            name = CameraName;

        Description = new CameraDescription(CameraType.VideoFile,
            path,
            name,
            GetFileResolution(path));
        CurrentFps = 0;

        _keepAliveTimer.Elapsed += CameraDisconnected;
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
            Description.FrameFormats = GetFileResolution(_videoFile);
            CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
            CurrentFps = CurrentFrameFormat?.Fps ?? 0;
            _format = CurrentFrameFormat?.Format ?? string.Empty;
            _delay = (int)(1000 / _videoFile?.Fps ?? 25);
        }
    }

    private bool SetNextFile(bool repeat)
    {
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
        Description.FrameFormats = GetFileResolution(_videoFile);
        CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
        CurrentFps = CurrentFrameFormat?.Fps ?? 0;
        _format = CurrentFrameFormat?.Format ?? string.Empty;
        _delay = (int)(1000 / _videoFile?.Fps ?? 25);

        return true;
    }

    private async void CameraDisconnected(object? sender, ElapsedEventArgs e)
    {
        if (_fpsTimer.ElapsedMilliseconds > FrameTimeout)
        {
            Console.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} Camera connection restarted ({_fpsTimer.ElapsedMilliseconds} timeout)");
            Stop(false);
            await Start(0, 0, _format, _token);
        }
    }

    public List<CameraDescription> DiscoverCamerasAsync(int discoveryTimeout, CancellationToken token)
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
            catch { }
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

    public async Task<bool> Start(int width, int height, string format, CancellationToken token)
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
            _delay = (int)(1000 / _videoFile?.Fps ?? 25);
            var nextFrameTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
            {
                var now = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                if (now >= nextFrameTime)
                {
                    nextFrameTime += _delay;
                    if (_videoFile?.Grab() ?? false)
                        CaptureImage();
                    else if (!SetNextFile(RepeatFile))
                        await _cancellationTokenSourceCameraGrabber.CancelAsync();
                }
                else
                    await Task.Delay(1, _cancellationTokenSourceCameraGrabber.Token);
            }

            Stop(true);
        }, _cancellationTokenSourceCameraGrabber.Token);

        IsRunning = true;

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
                var frame = new Mat();
                if (!(_videoFile?.Retrieve(frame) ?? false) || frame == null)
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

                _gcCounter++;
                if (_gcCounter >= 10)
                {
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                    _gcCounter = 0;
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

        lock (_getPictureThreadLock)
        {
            _keepAliveTimer.Stop();

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

        if (_videoFile == null)
            SetFile(_fileNames);

        Mat? image = null;
        if (_videoFile?.Grab() ?? false)
        {
            image = new Mat();
            _videoFile?.Retrieve(image);
        }
        else if (RepeatFile)
        {
            SetNextFile(true);
            if (_videoFile?.Grab() ?? false)
            {
                image = new Mat();
                _videoFile?.Retrieve(image);
            }
        }

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
                _cancellationTokenSourceCameraGrabber?.Dispose();
                _cancellationTokenSource?.Dispose();
                _videoFile?.Dispose();
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