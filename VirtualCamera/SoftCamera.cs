using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace VirtualCamera
{
    public partial class SoftCamera : IDisposable
    {
#if DEBUG
        const string dllName = "softcamd.dll";
#else
        const string dllName = "softcam.dll";
#endif

        public int ResolutionX;
        public int ResolutionY;
        public bool AppIsConnected
        {
            get => _cam != 0 && scIsConnected(_cam);
        }

        // First, create a virtual camera instance with scCreateCamera().
        // A virtual camera is a source of a video image stream.
        // The dimension width and height can be any positive number
        // that is a multiple of 4.
        // The third argument framerate is used to make sending frames at regular intervals.
        // This framerate argument can be omitted, and the default framerate is 60.
        // If you want to send every frame immediately without the frame rate regulator,
        // specify 0 to the framerate argument, then it will be a variable frame rate.
        [LibraryImport(dllName, EntryPoint = "scCreateCamera")]
        private static partial IntPtr scCreateCamera(UInt16 width, UInt16 height, float framerate);

        // Here, we wait for an application to connect to this camera.
        // You can comment out this line to start sending frames immediately
        // no matter there is a receiver or not.
        [LibraryImport(dllName, EntryPoint = "scWaitForConnection")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool scWaitForConnection(IntPtr ptr, float timeout);

        // Here we can check if the application is connected to the camera
        [LibraryImport(dllName, EntryPoint = "scIsConnected")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool scIsConnected(IntPtr ptr);

        // Our canvas is a simple array of RGB pixels.
        // Note that the color component order is BGR, not RGB.
        // This is due to the convention of DirectShow.
        [LibraryImport(dllName, EntryPoint = "scSendFrame")]
        private static partial void scSendFrame(IntPtr camera, IntPtr imageBits);

        // Delete the camera instance.
        // The receiver application will no longer receive new frames from this camera.
        [LibraryImport(dllName, EntryPoint = "scDeleteCamera")]
        private static partial void scDeleteCamera(IntPtr ptr);

        private readonly DllReg _dllReg;
        private readonly IntPtr _cam;
        private byte[] _buffer;
        private bool _disposedValue;

        public SoftCamera(int xResolution, int yResolution, float frameRate = 0f)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(xResolution, nameof(xResolution));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(yResolution, nameof(yResolution));

            if (xResolution % 4 != 0)
                throw new ArgumentException("Resolution width must be a multiple of 4", nameof(xResolution));

            if (yResolution % 4 != 0)
                throw new ArgumentException("Resolution height must be a multiple of 4", nameof(yResolution));

            if (frameRate < 0)
                throw new ArgumentOutOfRangeException(nameof(frameRate), frameRate, "Frame rate cannot be negative");

            ResolutionX = xResolution;
            ResolutionY = yResolution;
            _dllReg = new DllReg(dllName);
            var registered = _dllReg.RegisterComDLL();
            _cam = scCreateCamera((UInt16)ResolutionX, (UInt16)ResolutionY, frameRate);
            _buffer = new byte[ResolutionX * ResolutionY * 3];
        }

        public bool PushFrame(byte[] frame)
        {
            if (frame.Length != _buffer.Length)
                return false;

            frame.CopyTo(_buffer, 0);
            PushFrameInternal();

            return true;
        }

        public bool PushFrame(Bitmap frame)
        {
            if (frame.Width < ResolutionX || frame.Height < ResolutionY || frame.PixelFormat != PixelFormat.Format24bppRgb)
                return false;

            BitmapData? bmpdata = null;

            try
            {
                bmpdata = frame.LockBits(new Rectangle(0, 0, ResolutionX, ResolutionY), ImageLockMode.ReadOnly, frame.PixelFormat);
                var numbytes = bmpdata.Stride * ResolutionY;
                var ptr = bmpdata.Scan0;
                Marshal.Copy(ptr, _buffer, 0, numbytes);
                PushFrameInternal();

                return true;
            }
            finally
            {
                if (bmpdata != null)
                    frame.UnlockBits(bmpdata);
            }
        }

        private void PushFrameInternal()
        {
            unsafe
            {
                fixed (byte* p = _buffer)
                {
                    var ptr = (IntPtr)p;
                    scSendFrame(_cam, ptr);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                scDeleteCamera(_cam);
                if (disposing)
                {
                    _dllReg.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _buffer = null!;
                _disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SoftCamera()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
