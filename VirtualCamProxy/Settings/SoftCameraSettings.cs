namespace VirtualCamProxy.Settings;

public class SoftCameraSettings
{
    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;
    public bool ShowStream { get; set; } = false;
    public CameraSettings Cameras { get; set; } = new CameraSettings();
    public DesktopCameraSettings DesktopCamera { get; set; } = new DesktopCameraSettings();
    public VideoFileCameraSettings VideoFileCamera { get; set; } = new VideoFileCameraSettings();
    public ImageFileCameraSettings ImageFileCamera { get; set; } = new ImageFileCameraSettings();
    public FilterSettings Filters { get; set; } = new FilterSettings();
}