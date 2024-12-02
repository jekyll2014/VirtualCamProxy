using CameraLib;

namespace VirtualCamProxy.Settings;

public class CustomCameraDto
{
    public CameraType Type { get; set; } = CameraType.Unknown;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public AuthType AuthenicationType { get; set; } = AuthType.None;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool ForceConnect { get; set; } = false;
}