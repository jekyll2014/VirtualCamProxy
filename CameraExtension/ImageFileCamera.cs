﻿using CameraLib;

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
    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
    public bool RepeatFile = true;

    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
    private const string CameraName = "Image file(s)";
    private readonly object _getPictureThreadLock = new();
    private Task? _captureTask;
    private readonly Stopwatch _fpsTimer = new();
    private Mat? _frame = new Mat();
    private byte _frameCount;
    private readonly List<string> _fileNames = [];
    private int _fileIndex = 0;
    private string _imageFile = string.Empty;
    public int Delay = 1000;
    private int _gcCounter = 0;
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

    public async Task<bool> GetImageData(int discoveryTimeout = 1000)
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

    public async Task<bool> Start(int width, int height, string format, CancellationToken token)
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
                    await Task.Delay(10, CancellationToken.None);
            }

            Stop(true);
        }, _cancellationTokenSourceCameraGrabber.Token);

        IsRunning = true;

        return true;
    }

    private void CaptureImage(string file)
    {
        if (Monitor.IsEntered(_getPictureThreadLock) || string.IsNullOrEmpty(file))
            return;

        try
        {
            lock (_getPictureThreadLock)
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
                ImageCapturedEvent?.Invoke(this, _frame);
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
            if (_gcCounter >= 10)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                _gcCounter = 0;
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

        //lock (_getPictureThreadLock)
        {
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
