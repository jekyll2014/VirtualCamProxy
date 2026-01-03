using VirtualCamProxy.Settings;

namespace VirtualCamProxy;

public class CameraSettings
{
    public bool AutoSearchIp { get; set; } = true;
    public bool AutoSearchUsb { get; set; } = true;
    public bool AutoSearchUsbFC { get; set; } = true;
    public bool AutoSearchDesktop { get; set; } = true;
    public int DiscoveryTimeOut { get; set; } = 1000;
    public bool ForceCameraConnect { get; set; } = false;
    public int MaxFrameBuffer { get; set; } = 10;
    public List<CustomCameraDto> CustomCameras { get; set; } = new();
    public int FrameTimeout { get; set; } = 30000;
}