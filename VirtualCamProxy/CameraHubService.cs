using CameraLib;
using CameraLib.FlashCap;
using CameraLib.IP;
using CameraLib.MJPEG;
using CameraLib.USB;

using OpenCvSharp;

using System.Collections.Concurrent;
using CameraExtension;

namespace VirtualCamProxy;
public class CameraHubService
{
    private readonly CameraSettings _cameraSettings;
    private readonly int _maxBuffer;
    public readonly List<ICamera> Cameras = new List<ICamera>();
    public readonly ConcurrentQueue<Mat> ImageQueue = new ConcurrentQueue<Mat>();
    public ICamera? CurrentCamera { get; private set; }

    public CameraHubService(CameraSettings settings)
    {
        _cameraSettings = settings;
        _maxBuffer = _cameraSettings.MaxFrameBuffer;
    }

    public async Task RefreshCameraCollection(CancellationToken cancellationToken)
    {
        // remove predefined cameras from collection
        Cameras.Clear();

        // add custom cameras
        Console.WriteLine("Adding predefined cameras...");
        Parallel.ForEach(_cameraSettings.CustomCameras, (c) =>
        {
            Console.WriteLine($"\t{c.Name}");
            ICamera serverCamera;
            if (c.Type == CameraType.IP)
            {
                serverCamera =
                    new IpCamera(path: c.Path,
                        name: c.Name,
                        authenicationType: c.AuthenicationType,
                        login: c.Login,
                        password: c.Password,
                        discoveryTimeout: _cameraSettings.DiscoveryTimeOut,
                        forceCameraConnect: _cameraSettings.ForceCameraConnect);
            }
            else if (c.Type == CameraType.MJPEG)
            {
                serverCamera = new MjpegCamera(path: c.Path,
                        name: c.Name,
                        authenicationType: c.AuthenicationType,
                        login: c.Login,
                        password: c.Password,
                        discoveryTimeout: _cameraSettings.DiscoveryTimeOut,
                        forceCameraConnect: _cameraSettings.ForceCameraConnect);
            }
            else if (c.Type == CameraType.USB)
            {
                try
                {
                    serverCamera = new UsbCamera(c.Path, c.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
            else if (c.Type == CameraType.USB_FC)
            {
                try
                {
                    serverCamera = new UsbCameraFc(c.Path, c.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
            else
                return;

            serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
            Cameras.Add(serverCamera);
        });

        List<CameraDescription> ipCameras = new();
        if (_cameraSettings.AutoSearchIp)
        {
            Console.WriteLine("Detecting IP cameras...");
            ipCameras = await IpCamera.DiscoverOnvifCamerasAsync(_cameraSettings.DiscoveryTimeOut);
        }

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
                var serverCamera = new UsbCamera(c.Path);
                serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
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
                var serverCamera = new UsbCameraFc(c.Path);
                serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
                Cameras.Add(serverCamera);
            }
        }

        if (_cameraSettings.AutoSearchIp)
        {
            Console.WriteLine("Autodetecting IP cameras...");
            foreach (var c in ipCameras)
                Console.WriteLine($"IP-Camera: {c.Name} - [{c.Path}]");

            // add newly discovered cameras
            foreach (var c in ipCameras
                         .Where(c => Cameras
                             .All(n => n.Description.Path != c.Path)))
            {
                Console.WriteLine($"Adding IP-Camera: {c.Name} - [{c.Path}]");
                var serverCamera = new IpCamera(c.Path);
                serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
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
                var serverCamera = new ScreenCamera(c.Path);
                serverCamera.FrameTimeout = _cameraSettings.FrameTimeout;
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
        if (cameraNumber < 0 || cameraNumber >= Cameras.Count())
            //throw new ArgumentOutOfRangeException($"No cameraStream available: \"{cameraNumber}\"");\
            return null;

        var camera = Cameras.ToArray()[cameraNumber];

        return camera;
    }

    public ICamera? GetCamera(string cameraId)
    {
        var camera = Cameras.FirstOrDefault(n => n.Description.Path == cameraId);
        if (camera == null)
            //throw new ArgumentOutOfRangeException($"No cameraStream available: \"{cameraId}\"");
            return null;

        return camera;
    }

    private void GetImageFromCameraStream(ICamera camera, Mat image)
    {
        if (ImageQueue.Count >= _maxBuffer)
        {
            if (ImageQueue.TryDequeue(out var frame))
                frame?.Dispose();
        }

        ImageQueue.Enqueue(image);
    }
}
