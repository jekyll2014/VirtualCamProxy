using OpenCvSharp;

using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Timers;
using OpenCvSharp.Extensions;
using Timer = System.Timers.Timer;
using Size = System.Drawing.Size;

namespace CameraLib.Screen
{
    public class ScreenCamera : ICamera, IDisposable
    {
        public CameraDescription Description { get; set; }
        public bool IsRunning { get; private set; }
        public FrameFormat? CurrentFrameFormat { get; private set; }
        public double CurrentFps { get; private set; }
        public int FrameTimeout { get; set; } = 30000;

        public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;

        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

        public bool ShowCursor = true;

        private const string NamePrefix = "Desktop#";
        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationTokenSource? _cancellationTokenSourceCameraGrabber;

        private System.Windows.Forms.Screen Screen;
        private readonly object _getPictureThreadLock = new();
        private Task? _captureTask;
        private Mat? _frame;
        private readonly Stopwatch _fpsTimer = new();
        private byte _frameCount;

        private readonly Timer _keepAliveTimer = new Timer();
        private int _width = 0;
        private int _height = 0;
        private string _format = string.Empty;
        private CancellationToken _token = CancellationToken.None;

        private bool _disposedValue;

        public ScreenCamera(string path, string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = path.Replace("\\", "");

            Screen = System.Windows.Forms.Screen.AllScreens.FirstOrDefault(n => n.DeviceName == path)
                         ?? throw new ArgumentException("Can not find camera", nameof(path));

            Description = new CameraDescription(CameraType.Screen, path, name, GetAllAvailableResolution(path));
            CurrentFps = Description.FrameFormats.FirstOrDefault()?.Fps ?? 15;

            _keepAliveTimer.Elapsed += CameraDisconnected;
        }

        private void CameraDisconnected(object? sender, ElapsedEventArgs e)
        {
            if (_fpsTimer.ElapsedMilliseconds > FrameTimeout)
            {
                Console.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} Camera connection restarted ({_fpsTimer.ElapsedMilliseconds} timeout)");
                Stop(false);
                Start(_width, _height, _format, _token);
            }
        }

        public List<CameraDescription> DiscoverCamerasAsync(int discoveryTimeout, CancellationToken token)
        {
            return DiscoverScreenCameras();
        }

        public static List<CameraDescription> DiscoverScreenCameras()
        {
            var result = new List<CameraDescription>();
            var n = 0;
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                result.Add(new CameraDescription(CameraType.Screen, $"{screen.DeviceName}", $"{NamePrefix}{(n).ToString()}",
                    new List<FrameFormat>()
                    {
                        new FrameFormat(screen.Bounds.Width, screen.Bounds.Height, $"{screen.BitsPerPixel}bpp", 15)
                    }));

                n++;
            }

            return result;
        }

        private List<FrameFormat> GetAllAvailableResolution(string path)
        {
            return new List<FrameFormat>()
                {
                    new FrameFormat(Screen.Bounds.Width, Screen.Bounds.Height, $"{Screen.BitsPerPixel}bpp", 15)
                };
        }

        public async Task<bool> Start(int width, int height, string format, CancellationToken token)
        {
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
            _captureTask = Task.Run(() =>
            {
                var size = new Size(Screen.Bounds.Width, Screen.Bounds.Height);
                var srcImage = new Bitmap(size.Width, size.Height);
                var srcGraphics = Graphics.FromImage(srcImage);
                _width = size.Width;
                _height = size.Height;
                var dstImage = srcImage;
                var dstGraphics = srcGraphics;
                var curSize = new Size(32, 32);

                while (!_cancellationTokenSourceCameraGrabber.Token.IsCancellationRequested)
                {
                    srcGraphics.CopyFromScreen(Screen.Bounds.Left,
                        Screen.Bounds.Top,
                        0,
                        0,
                        size);

                    if (ShowCursor)
                    {
                        var cursorPosition = Cursor.Position;
                        cursorPosition.X -= Screen.Bounds.Left;
                        cursorPosition.Y -= Screen.Bounds.Top;
                        Cursors.Default.Draw(srcGraphics, new Rectangle(cursorPosition, curSize));
                    }

                    ImageCaptured(dstImage);
                    Task.Delay(50, _cancellationTokenSourceCameraGrabber.Token);
                }

                srcImage.Dispose();
                dstImage.Dispose();
                dstGraphics.Dispose();
                srcGraphics.Dispose();
            }, _cancellationTokenSourceCameraGrabber.Token);

            IsRunning = true;

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
                    _frame?.Dispose();
                    _frame = BitmapToMap(image);

                    if (CurrentFrameFormat == null)
                    {
                        CurrentFrameFormat = new FrameFormat(_frame.Width, _frame.Height);
                    }

                    ImageCapturedEvent?.Invoke(this, _frame.Clone());
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
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
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
                image = BitmapTo24bpp(image);

            //return image.ToMat().CvtColor(ColorConversionCodes.RGBA2RGB);
            return image.ToMat();
        }

        public static Bitmap BitmapTo24bpp(Image image)
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
                while (IsRunning && _frame == null && !token.IsCancellationRequested)
                    await Task.Delay(10, token);

                lock (_getPictureThreadLock)
                {
                    return _frame?.Clone();
                }
            }

            Mat? image = null;
            var size = new Size(Screen.Bounds.Width, Screen.Bounds.Height);
            var srcImage = new Bitmap(size.Width, size.Height);
            var srcGraphics = Graphics.FromImage(srcImage);
            _width = size.Width;
            _height = size.Height;
            var curSize = new Size(32, 32);
            srcGraphics.CopyFromScreen(Screen.Bounds.Left, Screen.Bounds.Top, 0, 0, size);
            if (ShowCursor)
                Cursors.Default.Draw(srcGraphics, new Rectangle(Cursor.Position, curSize));

            image = BitmapToMap(srcImage);
            srcImage.Dispose();
            srcGraphics.Dispose();
            srcImage.Dispose();
            srcGraphics.Dispose();

            return image;
        }

        public async IAsyncEnumerable<Mat> GrabFrames([EnumeratorCancellation] CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var image = await GrabFrame(token);
                if (image == null)
                {
                    await Task.Delay(100, token);
                }
                else
                {
                    yield return image.Clone();
                    image.Dispose();
                }
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
