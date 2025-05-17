using CameraExtension;

using VirtualCamProxy.Settings;

namespace VirtualCamProxy.Panels;

public partial class DesktopCameraSettingsPanel : UserControl
{
    private readonly ScreenCamera _camera;

    private readonly DesktopCameraSettings _settings;
    private readonly CameraHubService _cameraHub;
    public DesktopCameraSettingsPanel(ScreenCamera camera, DesktopCameraSettings settings, CameraHubService cameraHub)
    {
        InitializeComponent();
        _camera = camera;
        _settings = settings;
        _cameraHub = cameraHub;

        checkBox_showCursor.Checked = _settings.ShowCursor;
        checkBox_showClicks.Checked = _settings.ShowClicks;
    }

    private void CheckBox_showCursor_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ShowCursor = checkBox_showCursor.Checked;
        if (_cameraHub.CurrentCamera is ScreenCamera scam && scam.Description.Path == _camera.Description.Path)
            scam.ShowCursor = _settings.ShowCursor;
    }

    private void checkBox_showClicks_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ShowClicks = checkBox_showClicks.Checked;
        if (_cameraHub.CurrentCamera is ScreenCamera scam && scam.Description.Path == _camera.Description.Path)
            scam.ShowClicks = _settings.ShowClicks;
    }
}
