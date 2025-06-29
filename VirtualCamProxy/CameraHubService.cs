using CameraExtension;

using CameraLib;
using CameraLib.FlashCap;
using CameraLib.IP;
using CameraLib.MJPEG;
using CameraLib.USB;

using OpenCvSharp;

namespace VirtualCamProxy;
public class CameraHubService : IDisposable
{
    public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;

    public readonly List<ICamera> Cameras = [];
    public ICamera? CurrentCamera { get; private set; }

    private readonly CameraSettings _cameraSettings;
    private bool disposedValue;

    public CameraHubService(CameraSettings settings)
    {
        _cameraSettings = settings;
    }

    public async Task RefreshCameraCollection()
    {
        // remove predefined cameras from collection
        Cameras.Clear();

        // add custom cameras
        Console.WriteLine("Adding predefined cameras...");
        Parallel.ForEach(_cameraSettings.CustomCameras, async (cam) =>
        {
            Console.WriteLine($"\t{cam.Name}");
            ICamera? serverCamera = null;
            if (cam.Type == CameraType.IP)
            {
                serverCamera = new IpCamera(path: cam.Path,
                        name: cam.Name,
                        authenticationType: cam.AuthenticationType,
                        login: cam.Login,
                        password: cam.Password);

                if (cam.ForceConnect)
                {
                    await serverCamera.GetImageData(_cameraSettings.DiscoveryTimeOut);
                }
                else
                    serverCamera.Description.FrameFormats = new[] { new FrameFormat(0, 0, "") };
            }
            else if (cam.Type == CameraType.MJPEG)
            {
                serverCamera = new MjpegCamera(path: cam.Path,
                    name: cam.Name,
                    authenticationType: cam.AuthenticationType,
                    login: cam.Login,
                    password: cam.Password);

                if (cam.ForceConnect)
                    await serverCamera.GetImageData(_cameraSettings.DiscoveryTimeOut);
                else
                    serverCamera.Description.FrameFormats = new[] { new FrameFormat(0, 0, "MJPG") };
            }
            else if (cam.Type == CameraType.USB)
            {
                try
                {
                    serverCamera = new UsbCamera(cam.Path, cam.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return;
                }
            }
            else if (cam.Type == CameraType.USB_FC)
            {
                try
                {
                    serverCamera = new UsbCameraFc(cam.Path, cam.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return;
                }
            }

            if (serverCamera != null)
            {
                serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
                Cameras.Add(serverCamera);
            }
        });

        if (_cameraSettings.AutoSearchUsb)
        {
            Console.WriteLine("Autodetecting USB cameras...");
            var usbCameras = UsbCamera.DiscoverUsbCameras();
            foreach (var c in usbCameras)
                Console.WriteLine($"USB-CameraStream: {c.Name} - [{c.Path}]");

            // add newly discovered cameras
            foreach (var c in usbCameras
                         .Where(c => Cameras
                             .All(n => n.Description.Path != c.Path)))
            {
                var serverCamera = new UsbCamera(c.Path)
                {
                    FrameTimeout = _cameraSettings.FrameTimeout
                };

                Cameras.Add(serverCamera);
            }
        }

        if (_cameraSettings.AutoSearchUsbFC)
        {
            Console.WriteLine("Autodetecting USB_FC cameras...");
            var usbFcCameras = UsbCameraFc.DiscoverUsbCameras();
            foreach (var c in usbFcCameras)
                Console.WriteLine($"USB_FC-CameraStream: {c.Name} - [{c.Path}]");

            // add newly discovered cameras
            foreach (var c in usbFcCameras
                         .Where(c => Cameras
                             .All(n => n.Description.Path != c.Path)))
            {
                var serverCamera = new UsbCameraFc(c.Path)
                {
                    FrameTimeout = _cameraSettings.FrameTimeout
                };

                Cameras.Add(serverCamera);
            }
        }

        if (_cameraSettings.AutoSearchIp)
        {
            Console.WriteLine("Autodetecting IP cameras...");
            var ipCameras = await IpCamera.DiscoverOnvifCamerasAsync(_cameraSettings.DiscoveryTimeOut);
            foreach (var c in ipCameras)
                Console.WriteLine($"IP-Camera: {c.Name} - [{c.Path}]");

            // add newly discovered cameras
            foreach (var c in ipCameras
                         .Where(c => Cameras
                             .All(n => n.Description.Path != c.Path)))
            {
                Console.WriteLine($"Adding IP-Camera: {c.Name} - [{c.Path}]");
                var serverCamera = new IpCamera(c.Path)
                {
                    FrameTimeout = _cameraSettings.FrameTimeout
                };

                Cameras.Add(serverCamera);
            }
        }

        if (_cameraSettings.AutoSearchDektop)
        {
            Console.WriteLine("Autodetecting desktop screens...");
            var screenCameras = ScreenCamera.DiscoverScreenCameras();
            foreach (var c in screenCameras)
                Console.WriteLine($"Screen-CameraStream: {c.Name} - [{c.Path}]");

            // add newly discovered cameras
            foreach (var c in screenCameras
                         .Where(c => Cameras
                             .All(n => n.Description.Path != c.Path)))
            {
                var serverCamera = new ScreenCamera(c.Path)
                {
                    FrameTimeout = _cameraSettings.FrameTimeout
                };

                Cameras.Add(serverCamera);
            }
        }

        Cameras.Add(new VideoFileCamera(""));
        Cameras.Add(new ImageFileCamera(""));

        Console.WriteLine("Done.");
    }

    public async Task<CancellationToken> HookCamera(int cameraIndex, int width, int height, string imageFormat = "")
    {
        var cameraId = GetCamera(cameraIndex)?.Description.Path;
        if (cameraId == null)
            return CancellationToken.None;

        return await HookCamera(cameraId, width, height, imageFormat);
    }

    public async Task<CancellationToken> HookCamera(string cameraId, int width, int height, string imageFormat = "")
    {
        if (Cameras.All(n => n.Description.Path != cameraId))
            return CancellationToken.None;

        CurrentCamera = Cameras
        .FirstOrDefault(n => n.Description.Path == cameraId);

        if (CurrentCamera == null)
            return CancellationToken.None;

        CurrentCamera.ImageCapturedEvent += GetImageFromCameraStream;
        if (!await CurrentCamera.Start(width,
                height,
                imageFormat,
                CancellationToken.None))
            return CancellationToken.None;

        return CurrentCamera.CancellationToken;
    }

    public bool UnHookCamera()
    {
        if (CurrentCamera != null)
        {
            CurrentCamera.ImageCapturedEvent -= GetImageFromCameraStream;
            CurrentCamera.Stop();
            CurrentCamera = null;
        }

        return true;
    }

    public ICamera? GetCamera(int cameraNumber)
    {
        if (cameraNumber < 0 || cameraNumber >= Cameras.Count)
            return null;

        var camera = Cameras.ToArray()[cameraNumber];

        return camera;
    }

    public ICamera? GetCamera(string cameraId)
    {
        var camera = Cameras.FirstOrDefault(n => n.Description.Path == cameraId);
        return camera ?? null;
    }

    private void GetImageFromCameraStream(ICamera camera, Mat image)
    {
        if (!image.Empty())
            ImageCapturedEvent?.Invoke(CurrentCamera, image);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                UnHookCamera();
                CurrentCamera?.Dispose();
                foreach (var cam in Cameras)
                    cam.Dispose();

                //Image?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
