using CameraLib;

using OpenCvSharp;
using OpenCvSharp.Extensions;

using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Timers;

using Size = System.Drawing.Size;
using Timer = System.Timers.Timer;

namespace CameraExtension;

public class ScreenCamera : ICamera
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
    public bool ShowCursor = true;
    // ToDo: Not implemented yet
    public bool ShowClicks = true;
    public double Fps
    {
        get => _fps;
        set
        {
            _fps = value;
            _delay = (int)(1000 / _fps);
        }
    }

    private const string NamePrefix = "Desktop#";
    private const int FpsCalculationFrameCount = 100;
    private const int FrameCheckDelayMs = 10;

    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;
    private readonly Screen Screen;
    private readonly object _getPictureThreadLock = new();
    private Task? _captureTask;
    private readonly Stopwatch _fpsTimer = new();
    private Mat? _frame = new Mat();
    private byte _frameCount;
    private double _fps = 15;
    private int _delay = 100;
    private readonly Timer _keepAliveTimer = new();
    private int _width = 0;
    private int _height = 0;
    private CancellationToken _token = CancellationToken.None;
    private bool _disposedValue;
    private bool _isReconnecting = false;
    private Bitmap? _bitmapBuffer;
    private Graphics? _graphicsBuffer;

    public ScreenCamera(string path, string name = "", double fps = 15)
    {
        if (string.IsNullOrEmpty(name))
            name = path.Replace("\\", "");

        Screen = Screen.AllScreens.FirstOrDefault(n => n.DeviceName == path)
                     ?? throw new ArgumentException("Can not find camera", nameof(path));

        Fps = fps;
        Description = new CameraDescription(CameraType.Screen, path, name, GetAllAvailableResolution().ToArray());
        CurrentFps = Description.FrameFormats.FirstOrDefault()?.Fps ?? 15;

        _keepAliveTimer.Elapsed += CameraDisconnected;
    }

    public async Task<bool> GetImageDataAsync(int discoveryTimeout = 1000)
    {
        Description.FrameFormats = GetAllAvailableResolution().ToArray();

        return Description.FrameFormats.Any();
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
                    await StartAsync(_width, _height, string.Empty, _token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Screen camera reconnection failed: {ex}");
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
        var n = 0;
        foreach (var screen in System.Windows.Forms.Screen.AllScreens)
        {
            result.Add(new CameraDescription(CameraType.Screen, $"{screen.DeviceName}", $"{NamePrefix}{n}",
                [
                        new FrameFormat(screen.Bounds.Width, screen.Bounds.Height, $"{screen.BitsPerPixel}bpp", 15)
                ]));

            n++;
        }

        return result;
    }

    private List<FrameFormat> GetAllAvailableResolution()
    {
        return
                [
                    new FrameFormat(Screen.Bounds.Width, Screen.Bounds.Height, $"{Screen.BitsPerPixel}bpp", _fps)
                ];
    }

    public async Task<bool> StartAsync(int width, int height, string format, CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);

        if (IsRunning)
            return true;

        _width = width;
        _height = height;
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
            var size = new Size(Screen.Bounds.Width, Screen.Bounds.Height);

            // Reuse bitmap buffer
            if (_bitmapBuffer == null || _bitmapBuffer.Width != size.Width || _bitmapBuffer.Height != size.Height)
            {
                _bitmapBuffer?.Dispose();
                _bitmapBuffer = new Bitmap(size.Width, size.Height);
            }

            if (_graphicsBuffer == null)
            {
                _graphicsBuffer?.Dispose();
                _graphicsBuffer = Graphics.FromImage(_bitmapBuffer);
            }

            _width = size.Width;
            _height = size.Height;
            var curSize = new Size(32, 32);

            var nextFrameTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
            {
                var now = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                if (now >= nextFrameTime)
                {
                    nextFrameTime += _delay;
                    _graphicsBuffer!.CopyFromScreen(Screen.Bounds.Left, Screen.Bounds.Top, 0, 0, size);
                    if (ShowCursor)
                    {
                        var cursorPosition = Cursor.Position;
                        cursorPosition.X -= Screen.Bounds.Left;
                        cursorPosition.Y -= Screen.Bounds.Top;
                        Cursors.Default.Draw(_graphicsBuffer, new Rectangle(cursorPosition, curSize));
                    }

                    ImageCaptured(_bitmapBuffer!);
                }
                else
                    await Task.Delay(FrameCheckDelayMs, _cancellationTokenSourceCameraGrabber.Token);
            }
            // Note: _bitmapBuffer and _graphicsBuffer are reused and disposed in Dispose()
        }, _cancellationTokenSourceCameraGrabber.Token);

        IsRunning = true;
        CameraConnectedEvent?.Invoke(this);

        return true;
    }

    private void ImageCaptured(Bitmap image)
    {
        if (Monitor.IsEntered(_getPictureThreadLock))
            return;

        try
        {
            lock (_getPictureThreadLock)
            {
                _frame = BitmapToMap(image);
                if (_frame == null || _frame.Empty())
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

    private static Mat BitmapToMap(Bitmap image)
    {
        if (image.PixelFormat != PixelFormat.Format24bppRgb)
            image = BitmapTo24Bpp(image);

        return image.ToMat();
    }

    public static Bitmap BitmapTo24Bpp(Image image)
    {
        var bp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
        using (var gr = Graphics.FromImage(bp))
            gr.DrawImage(image, new Rectangle(0, 0, bp.Width, bp.Height));

        return bp;
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

        var size = new Size(Screen.Bounds.Width, Screen.Bounds.Height);
        var srcImage = new Bitmap(size.Width, size.Height);
        var srcGraphics = Graphics.FromImage(srcImage);
        _width = size.Width;
        _height = size.Height;
        var curSize = new Size(32, 32);
        srcGraphics.CopyFromScreen(Screen.Bounds.Left, Screen.Bounds.Top, 0, 0, size);
        if (ShowCursor)
            Cursors.Default.Draw(srcGraphics, new Rectangle(Cursor.Position, curSize));

        var image = BitmapToMap(srcImage);
        srcImage.Dispose();
        srcGraphics.Dispose();

        return image;
    }

    public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);

        while (!token.IsCancellationRequested)
        {
            var image = await GrabFrameAsync(token);
            if (image == null)
                await Task.Delay(FrameCheckDelayMs, token);
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
                _keepAliveTimer.Close();
                _keepAliveTimer.Dispose();
                _cancellationTokenSourceCameraGrabber?.Dispose();
                _cancellationTokenSource?.Dispose();
                _frame?.Dispose();
                _graphicsBuffer?.Dispose();
                _bitmapBuffer?.Dispose();
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
