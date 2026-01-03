using CameraLib;

using OpenCvSharp;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CameraExtension;

public class ImageFileCamera : ICamera
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

    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
    private const string CameraName = "Image file(s)";
    private const int FpsCalculationFrameCount = 100;
    private const int FrameCheckDelayMs = 10;
    private Task? _captureTask;
    private readonly Stopwatch _fpsTimer = new();
    private Mat? _frame = new Mat();
    private byte _frameCount;
    private readonly List<string> _fileNames = [];
    private int _fileIndex = 0;
    private string _imageFile = string.Empty;
    private int _stepToNextFile = 0;
    public int Delay = 1000;
    private bool _disposedValue;

    public ImageFileCamera(string path, string name = "")
    {
        if (string.IsNullOrEmpty(name))
            name = CameraName;

        Description = new CameraDescription(CameraType.VideoFile,
            path,
            name,
            GetFileResolution(path).ToArray());
        CurrentFps = 0;
    }

    public async Task<bool> GetImageDataAsync(int discoveryTimeout = 1000)
    {
        if (string.IsNullOrEmpty(_imageFile))
            return false;

        Description.FrameFormats = GetFileResolution(_imageFile).ToArray();

        return Description.FrameFormats.Any();
    }

    public void SetFile(string path)
    {
        SetFiles([path]);
    }

    public void SetFiles(List<string> paths)
    {
        if (!string.IsNullOrEmpty(_imageFile))
            _imageFile = string.Empty;

        _fileNames.Clear();
        _fileNames.AddRange(paths);
        _fileIndex = 0;
        if (_fileNames.Count != 0)
        {
            var file = _fileNames.FirstOrDefault() ?? "";
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                return;

            _imageFile = file;
            Description.Path = file;
            Description.FrameFormats = GetFileResolution(_imageFile).ToArray();
            CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
            CurrentFps = CurrentFrameFormat?.Fps ?? 0;
        }
    }

    public void StepToNextFile()
    {
        Interlocked.Increment(ref _stepToNextFile);
    }

    private bool SetNextFile(bool repeat)
    {
        if (!string.IsNullOrEmpty(_imageFile))
            _imageFile = string.Empty;

        if (_fileNames.Count == 0)
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

        Description.Path = _imageFile;
        Description.FrameFormats = GetFileResolution(_imageFile).ToArray();
        CurrentFrameFormat = Description.FrameFormats.FirstOrDefault();
        CurrentFps = CurrentFrameFormat?.Fps ?? 0;

        return true;
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

    private List<FrameFormat> GetFileResolution(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return
            [
                new FrameFormat(0, 0, "", 1000f / Delay)
            ];

        try
        {
            _frame = Cv2.ImRead(path, ImreadModes.Color);
        }
        catch
        {
            // Ignore if the file is not an image
        }

        return
        [
            new FrameFormat(_frame?.Width??0, _frame?.Height??0, new FileInfo(path).Extension, 1000f / Delay)
        ];
    }

    public async Task<bool> StartAsync(int width, int height, string format, CancellationToken token)
    {
        if (IsRunning)
            return true;

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSourceCameraGrabber?.Dispose();
        _cancellationTokenSourceCameraGrabber = new CancellationTokenSource();
        _fpsTimer.Reset();
        _frameCount = 0;
        _captureTask?.Dispose();
        _captureTask = Task.Run(async () =>
        {
            var nextFrameTime = DateTime.Now;
            _stepToNextFile = 0;
            while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
            {
                if (DateTime.Now >= nextFrameTime || _stepToNextFile > 0)
                {
                    if (_stepToNextFile > 0)
                        Interlocked.Decrement(ref _stepToNextFile);

                    nextFrameTime = DateTime.Now.AddMilliseconds(Delay);
                    CaptureImage(_imageFile);
                    if (!SetNextFile(RepeatFile))
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

    private void CaptureImage(string file)
    {
        try
        {
            try
            {
                _frame = Cv2.ImRead(file, ImreadModes.Color);
            }
            catch
            {
                // Ignore if the file is not an image
            }

            if (_frame == null)
                return;

            CurrentFrameFormat ??= new FrameFormat(_frame.Width, _frame.Height);
            ImageCapturedEvent?.Invoke(this, _frame.Clone());

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

        if (string.IsNullOrEmpty(_imageFile) || !File.Exists(_imageFile))
            SetNextFile(true);

        try
        {
            _frame = Cv2.ImRead(_imageFile, ImreadModes.Color);
        }
        catch
        {
            // Ignore if the file is not an image
        }

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
                _cancellationTokenSourceCameraGrabber?.Dispose();
                _cancellationTokenSource?.Dispose();
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
