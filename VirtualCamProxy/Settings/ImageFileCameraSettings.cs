namespace VirtualCamProxy.Settings;

public class ImageFileCameraSettings : FileCameraSettings
{
    public bool Repeat { get; set; } = true;
    public int Delay { get; set; } = 3000;
}