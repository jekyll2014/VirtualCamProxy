using CameraExtension;

using CameraLib;
using CameraLib.FlashCap;
using CameraLib.IP;
using CameraLib.MJPEG;
using CameraLib.USB;

using OpenCvSharp;

using System.Diagnostics;

namespace VirtualCamProxy;

public class CameraHubService : IDisposable
{
    public event ICamera.ImageCapturedEventHandler? ImageCapturedEvent;

    public readonly List<ICamera> Cameras = [];
    public ICamera? CurrentCamera { get; private set; }

    private readonly CameraSettings _cameraSettings;
    private bool _disposedValue;

    public CameraHubService(CameraSettings settings)
    {
        _cameraSettings = settings;
    }

    public async Task RefreshCameraCollectionAsync()
    {
        // remove predefined cameras from collection
        Cameras.Clear();

        // add custom cameras
        Debug.WriteLine("Adding predefined cameras...");

        var customCameraTasks = _cameraSettings.CustomCameras.Select(async cam =>
        {
            Debug.WriteLine($"\t{cam.Name}");
            ICamera? serverCamera = null;

            try
            {
                if (cam.Type == CameraType.IP)
                {
                    serverCamera = new IpCamera(path: cam.Path,
                            name: cam.Name,
                            authenticationType: cam.AuthenticationType,
                            login: cam.Login,
                            password: cam.Password);

                    if (cam.ForceConnect)
                    {
                        await serverCamera.GetImageDataAsync(_cameraSettings.DiscoveryTimeOut);
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
                        await serverCamera.GetImageDataAsync(_cameraSettings.DiscoveryTimeOut);
                    else
                        serverCamera.Description.FrameFormats = new[] { new FrameFormat(0, 0, "MJPG") };
                }
                else if (cam.Type == CameraType.USB)
                {
                    serverCamera = new UsbCamera(cam.Path, cam.Name);
                }
                else if (cam.Type == CameraType.USB_FC)
                {
                    serverCamera = new UsbCameraFc(cam.Path, cam.Name);
                }

                if (serverCamera != null)
                {
                    serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing camera {cam.Name}: {ex}");
                serverCamera = null;
            }

            return serverCamera;
        });

        var customCameras = await Task.WhenAll(customCameraTasks);

        foreach (var camera in customCameras.Where(c => c != null))
        {
            Cameras.Add(camera!);
        }

        if (_cameraSettings.AutoSearchUsb)
        {
            Debug.WriteLine("Autodetecting USB cameras...");
            var usbCameras = UsbCamera.DiscoverUsbCameras();
            foreach (var c in usbCameras)
                Debug.WriteLine($"USB-CameraStream: {c.Name} - [{c.Path}]");

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
            Debug.WriteLine("Autodetecting USB_FC cameras...");
            var usbFcCameras = UsbCameraFc.DiscoverUsbCameras();
            foreach (var c in usbFcCameras)
                Debug.WriteLine($"USB_FC-CameraStream: {c.Name} - [{c.Path}]");

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
            Debug.WriteLine("Autodetecting IP cameras...");
            var ipCameras = await IpCamera.DiscoverOnvifCamerasAsync(_cameraSettings.DiscoveryTimeOut);
            foreach (var c in ipCameras)
                Debug.WriteLine($"IP-Camera: {c.Name} - [{c.Path}]");

            // add newly discovered cameras
            foreach (var c in ipCameras
                         .Where(c => Cameras
                             .All(n => n.Description.Path != c.Path)))
            {
                Debug.WriteLine($"Adding IP-Camera: {c.Name} - [{c.Path}]");
                var serverCamera = new IpCamera(c.Path)
                {
                    FrameTimeout = _cameraSettings.FrameTimeout
                };

                Cameras.Add(serverCamera);
            }
        }

        if (_cameraSettings.AutoSearchDesktop)
        {
            Debug.WriteLine("Autodetecting desktop screens...");
            var screenCameras = ScreenCamera.DiscoverScreenCameras();
            foreach (var c in screenCameras)
                Debug.WriteLine($"Screen-CameraStream: {c.Name} - [{c.Path}]");

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

        Debug.WriteLine("Done.");
    }

    public async Task<CancellationToken> HookCameraAsync(int cameraIndex, int width, int height, string imageFormat = "")
    {
        var cameraDescription = GetCamera(cameraIndex)?.Description;
        var cameraId = cameraDescription?.Path;

        if (cameraDescription == null || cameraId == null)
            return CancellationToken.None;

        return await HookCameraAsync(cameraDescription.Type, cameraId, width, height, imageFormat);
    }

    public async Task<CancellationToken> HookCameraAsync(CameraType cameraType, string cameraId, int width, int height, string imageFormat = "")
    {
        if (Cameras.All(n => n.Description.Type == cameraType && n.Description.Path != cameraId))
            return CancellationToken.None;

        CurrentCamera = Cameras
        .FirstOrDefault(n => n.Description.Type == cameraType && n.Description.Path == cameraId);

        if (CurrentCamera == null)
            return CancellationToken.None;

        CurrentCamera.ImageCapturedEvent += GetImageFromCameraStream;
        if (!await CurrentCamera.StartAsync(width,
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
        ImageCapturedEvent?.Invoke(camera, image);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                UnHookCamera();
                CurrentCamera?.Dispose();
                foreach (var cam in Cameras)
                    cam.Dispose();
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
