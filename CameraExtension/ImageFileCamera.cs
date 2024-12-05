using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;

using CameraLib;

using OpenCvSharp;

using Timer = System.Timers.Timer;

namespace CameraExtension;

public class ImageFileCamera : ICamera, IDisposable
{
    public CameraDescription Description { get; set; }
    public bool IsRunning { get; private set; }
    public FrameFormat? CurrentFrameFormat { get; private set; }
    public double CurrentFps { get; private set; }
    public int FrameTimeout { get; set; } = 30000;

    public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    public bool RepeatFile = true;

    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;

    private readonly object _getPictureThreadLock = new();
    private Task? _captureTask;
    private readonly Stopwatch _fpsTimer = new();
    private byte _frameCount;

    private List<string> _fileNames = new List<string>();
    private int _fileIndex = 0;
    private string _imageFile = string.Empty;
    public int Delay = 1000;
    private readonly Timer _keepAliveTimer = new Timer();
    private string _format = string.Empty;
    private CancellationToken _token = CancellationToken.None;
    private int _gcCounter = 0;

    private bool _disposedValue;

    public ImageFileCamera(string path, string name = "")
    {
        Description = new CameraDescription(CameraType.VideoFile,
            path,
            "Image file(s)",
            GetAllAvailableResolution(path));
        CurrentFps = 0;

        _keepAliveTimer.Elapsed += CameraDisconnected;
    }

    public void SetFile(string path)
    {
        SetFile(new List<string>() { path });
    }

    public void SetFile(List<string> paths)
    {
        if (!string.IsNullOrEmpty(_imageFile))
            _imageFile = string.Empty;

        _fileNames.Clear();
        _fileNames.AddRange(paths);
        _fileIndex = 0;

        if (_fileNames.Any())
        {
            var file = _fileNames.FirstOrDefault() ?? "";

            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                return;

            var name = new FileInfo(file).Name;
            _imageFile = file;
            Description = new CameraDescription(CameraType.ImageFile,
                file,
                name,
                GetAllAvailableResolution(file));

            CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
            CurrentFps = CurrentFrameFormat?.Fps ?? 0;
            _format = CurrentFrameFormat?.Format ?? string.Empty;
        }
    }

    private bool SetNextFile(bool repeat)
    {
        if (!string.IsNullOrEmpty(_imageFile))
            _imageFile = string.Empty;

        if (!_fileNames.Any())
            return false;

        if (_fileIndex < _fileNames.Count - 1)
            _fileIndex++;
        else if (repeat)
            _fileIndex = 0;
        else
            return false;

        _imageFile = _fileNames[_fileIndex];

        if (string.IsNullOrEmpty(_imageFile) || !File.Exists(_imageFile))
            return false;

        var name = new FileInfo(_imageFile).Name;
        Description = new CameraDescription(CameraType.ImageFile,
            _imageFile,
            name,
            GetAllAvailableResolution(_imageFile));

        CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
        CurrentFps = CurrentFrameFormat?.Fps ?? 0;
        _format = CurrentFrameFormat?.Format ?? string.Empty;

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

    private List<FrameFormat> GetAllAvailableResolution(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return new List<FrameFormat>()
            {
                new FrameFormat(0, 0, "", 1000f / Delay)
            };

        Mat? img = null;
        try
        {
            img = Cv2.ImRead(path, ImreadModes.Color);
        }
        catch { }

        return new List<FrameFormat>()
        {
            new FrameFormat(img?.Width??0, img?.Height??0, new FileInfo(path).Extension, 1000f / Delay)
        };
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
            var nextFrameTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
            {
                var now = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                if (now >= nextFrameTime)
                {
                    nextFrameTime += Delay;
                    CaptureImage(_imageFile);
                    if (!SetNextFile(RepeatFile))
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

    private void CaptureImage(string file)
    {
        if (Monitor.IsEntered(_getPictureThreadLock))
            return;

        try
        {
            lock (_getPictureThreadLock)
            {
                Mat? frame = null;
                try
                {
                    frame = Cv2.ImRead(file, ImreadModes.Color);
                }
                catch { }

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
            ImageCapturedEvent += Camera_ImageCapturedEvent;
            void Camera_ImageCapturedEvent(ICamera camera, Mat image)
            {
                frame = image?.Clone();
            }

            while (IsRunning && frame == null && !token.IsCancellationRequested)
                await Task.Delay(10, token);

            ImageCapturedEvent -= Camera_ImageCapturedEvent;

            return frame;
        }

        if (string.IsNullOrEmpty(_imageFile) || !File.Exists(_imageFile))
            SetNextFile(true);

        Mat? image = null;
        try
        {
            image = Cv2.ImRead(_imageFile, ImreadModes.Color);
        }
        catch { }

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
