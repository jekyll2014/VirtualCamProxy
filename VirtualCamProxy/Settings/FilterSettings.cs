using OpenCvSharp;

namespace VirtualCamProxy.Settings;

public class FilterSettings
{
    public bool FlipHorizontal { get; set; } = false;
    public bool FlipVertical { get; set; } = false;
    public RotateFlags? Rotate { get; set; } = null;
}