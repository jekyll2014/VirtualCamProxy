using CameraExtension;

using VirtualCamProxy.Settings;

namespace VirtualCamProxy;

public partial class DesktopCameraSettingsPanel : UserControl
{
    private readonly DesktopCameraSettings _settings;
    private readonly CameraHubService _cameraHub;
    public DesktopCameraSettingsPanel(DesktopCameraSettings settings, CameraHubService cameraHub)
    {
        InitializeComponent();
        _settings = settings;
        _cameraHub = cameraHub;

        checkBox_showCursor.Checked = _settings.ShowCursor;
        checkBox_showClicks.Checked = _settings.ShowClicks;
    }

    private void CheckBox_showCursor_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ShowCursor = checkBox_showCursor.Checked;
        if (_cameraHub.CurrentCamera is ScreenCamera scam)
            scam.ShowCursor = _settings.ShowCursor;
    }

    private void checkBox_showClicks_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ShowClicks = checkBox_showClicks.Checked;
        if (_cameraHub.CurrentCamera is ScreenCamera scam)
            scam.ShowClicks = _settings.ShowClicks;
    }
}
