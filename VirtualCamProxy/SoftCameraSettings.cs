namespace VirtualCamProxy;

public class SoftCameraSettings
{
    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;

    public CameraSettings Cameras { get; set; } = new CameraSettings();
}