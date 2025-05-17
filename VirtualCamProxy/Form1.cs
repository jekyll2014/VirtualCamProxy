using CameraExtension;

using CameraLib;
using CameraLib.IP;
using CameraLib.MJPEG;

using OpenCvSharp;

using System.Security.Principal;

using VirtualCamera;

using VirtualCamProxy.Panels;
using VirtualCamProxy.Settings;

using Size = OpenCvSharp.Size;

namespace VirtualCamProxy;

public partial class Form1 : Form
{
    private const string ConfigFileName = "appsettings.json";
    private const string SoftCamName = "DirectShow Softcam";
    private readonly JsonStorage<SoftCameraSettings> _configuration = new(ConfigFileName, true);
    private readonly SoftCameraSettings _settings;
    private Task? _cameraFeed;
    private CancellationToken? _cancellationToken;
    private readonly CameraHubService _cameraHub;
    private bool _cameraStarted = false;
    private SoftCamera? _softCamera;
    private ICamera? _currentSource;

    public Form1()
    {
        InitializeComponent();
        _settings = _configuration.Storage;
        _cameraHub = new CameraHubService(_settings.Cameras);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        var filterPAnel = new FiltersPanel(_settings.Filters)
        {
            Dock = DockStyle.Fill
        };

        tabControl2.TabPages[1].Controls.Add(filterPAnel);

        textBox_x.Text = _settings.Width.ToString();
        textBox_y.Text = _settings.Height.ToString();
        checkBox_showStream.Checked = _settings.ShowStream;
        RefreshUi();
        //Button_refreshAll_Click(this, EventArgs.Empty);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!(_cameraFeed?.IsCompleted ?? true))
        {
            Button_camStop_Click(this, EventArgs.Empty);
            Button_softCamStop_Click(this, EventArgs.Empty);
        }

        _cameraHub.Dispose();
        _configuration.Save();
        if (_cameraFeed?.IsCompleted ?? false)
            _cameraFeed?.Dispose();

        _softCamera?.Dispose();
    }

    private async Task CameraFeed()
    {
        _cameraStarted = true;
        var softCamWidth = _settings.Width;
        var softCamHeight = _settings.Height;
        while (!(_cancellationToken?.IsCancellationRequested ?? true))
        {
            if (_cameraHub.ImageQueue.TryDequeue(out var image) && _softCamera != null)
            {
                if (_softCamera.AppIsConnected)
                {
                    var filtered = ApplyFilters(image, _settings.Filters, softCamWidth, softCamHeight);
                    var bitmap = MatToBitmap(filtered);
                    filtered?.Dispose();
                    if (bitmap != null)
                    {
                        _softCamera.PushFrame(bitmap);
                        if (_settings.ShowStream)
                        {
                            var bmp = (Bitmap)bitmap.Clone();
                            this.Invoke(() => { pictureBox_cam.Image = bmp; });
                        }

                        bitmap.Dispose();
                    }
                }

                image.Dispose();
            }
            else
            {
                await Task.Delay(10);
            }
        }

        _cameraStarted = false;
    }

    #region Camera tab
    private void Button_softCamStart_Click(object sender, EventArgs e)
    {
        if (_settings.Width <= 0 || _settings.Height <= 0)
            return;

        if (!IsAdministrator())
            MessageBox.Show("Application is not running with Administator rights so please register the required .dll prior to starting the soft-camera:\r\n'regsvr32 softcam.dll'");

        DisableUi();
        var started = _cameraStarted;
        if (started)
            Button_camStop_Click(this, EventArgs.Empty);

        _softCamera = new SoftCamera(_settings.Width, _settings.Height);
        if (started)
            Button_camStart_Click(this, EventArgs.Empty);

        RefreshUi();
    }

    private void Button_softCamStop_Click(object sender, EventArgs e)
    {
        DisableUi();
        if (_cameraStarted)
            Button_camStop_Click(this, EventArgs.Empty);

        _softCamera?.Dispose();
        _softCamera = null;
        RefreshUi();
    }

    private async void Button_camStart_Click(object sender, EventArgs e)
    {
        if (comboBox_cameras.SelectedIndex < 0 || comboBox_camResolution.SelectedIndex < 0)
            return;

        DisableUi();
        textBox_currentSource.Clear();
        _currentSource = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (_currentSource == null)
        {
            RefreshUi();

            return;
        }

        var frameFormat = _currentSource.Description.FrameFormats.ToArray()[comboBox_camResolution.SelectedIndex];
        var w = frameFormat?.Width ?? 0;
        var h = frameFormat?.Height ?? 0;
        var currentCam = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (currentCam is ScreenCamera scam)
        {
            scam.ShowCursor = _settings.DesktopCamera.ShowCursor;
        }
        else if (currentCam is VideoFileCamera vcam)
        {
            vcam.RepeatFile = _settings.VideoFileCamera.Repeat;
            vcam.SetFile(_settings.VideoFileCamera.Files);
        }
        else if (currentCam is ImageFileCamera icam)
        {
            icam.RepeatFile = _settings.ImageFileCamera.Repeat;
            icam.SetFiles(_settings.ImageFileCamera.Files);
        }

        _cancellationToken = await _cameraHub.HookCamera(comboBox_cameras.SelectedIndex, w, h, "");
        if (_cancellationToken == CancellationToken.None)
        {
            _currentSource.Stop();
            RefreshUi();

            return;
        }

        _cameraFeed = Task.Run(CameraFeed, (CancellationToken)_cancellationToken);
        RefreshUi();
    }

    private async void Button_camStop_Click(object sender, EventArgs e)
    {
        DisableUi();

        _cameraStarted = false;
        _cameraHub.UnHookCamera();
        try
        {
            if (_cameraFeed != null && !_cameraFeed.IsCompleted)
                await _cameraFeed.WaitAsync(CancellationToken.None);
        }
        catch
        {
            MessageBox.Show("Can't stop camera");
        }

        RefreshUi();
    }

    private async void Button_refreshAll_Click(object sender, EventArgs e)
    {
        DisableUi();
        button_refreshAll.Text = "Refreshing...";

        comboBox_cameras.Items.Clear();
        await _cameraHub.RefreshCameraCollection();
        _cameraHub.Cameras.RemoveAll(n => n.Description.Name == SoftCamName);
        var i = 1;
        foreach (var cam in _cameraHub.Cameras)
            comboBox_cameras.Items.Add($"{i++} {cam.Description.Name} [{cam.GetType().Name}] [{cam.Description.FrameFormats.Count()}]");

        if (_cameraHub.Cameras.Count > 0)
            comboBox_cameras.SelectedIndex = 0;

        button_refreshAll.Text = "Refresh all";
        RefreshUi();
    }

    private async void Button_refresh_Click(object sender, EventArgs e)
    {
        var camera = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (camera == null)
            return;

        DisableUi();
        button_refresh.Text = "Refreshing...";

        var image = await camera.GetImageData();
        ComboBox_cameras_SelectedIndexChanged(this, EventArgs.Empty);

        button_refresh.Text = "Refresh";
        RefreshUi();
    }

    private void DisableUi()
    {
        button_softCamStart.Enabled = false;
        button_softCamStop.Enabled = false;
        textBox_x.Enabled = false;
        textBox_y.Enabled = false;

        button_camStart.Enabled = false;
        button_camStop.Enabled = false;

        button_camGetImage.Enabled = false;
        comboBox_cameras.Enabled = false;
        comboBox_camResolution.Enabled = false;

        button_refreshAll.Enabled = false;
        button_refresh.Enabled = false;
    }

    private void RefreshUi()
    {
        button_softCamStart.Enabled = (_softCamera == null);
        button_softCamStop.Enabled = !button_softCamStart.Enabled;
        textBox_x.Enabled = button_softCamStart.Enabled;
        textBox_y.Enabled = button_softCamStart.Enabled;

        comboBox_cameras.Enabled = (comboBox_cameras.Items.Count > 0);
        comboBox_camResolution.Enabled = (comboBox_camResolution.Items.Count > 0);

        button_camStart.Enabled = (_softCamera != null)
                                  && (comboBox_cameras.SelectedIndex >= 0)
                                  && (comboBox_camResolution.SelectedIndex >= 0)
                                  && (!(_currentSource?.IsRunning ?? false));

        button_camStop.Enabled = (_softCamera != null)
                                 && (comboBox_cameras.SelectedIndex >= 0)
                                 && (comboBox_camResolution.SelectedIndex >= 0)
                                 && (_currentSource?.IsRunning ?? false);

        if (_currentSource?.IsRunning ?? false)
            textBox_currentSource.Text = _currentSource.Description.Path;
        else
            textBox_currentSource.Clear();

        button_camGetImage.Enabled = (comboBox_cameras.SelectedIndex >= 0) && (comboBox_camResolution.SelectedIndex >= 0);
        button_refreshAll.Enabled = !(_currentSource?.IsRunning ?? false);
        button_refresh.Enabled = !(_currentSource?.IsRunning ?? false) && (comboBox_cameras.SelectedIndex >= 0);
    }

    private async void Button_camGetImage_Click(object sender, EventArgs e)
    {
        var camera = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (camera == null)
            return;

        DisableUi();

        var image = await camera.GrabFrame(CancellationToken.None);
        if (image != null)
        {
            var bitmap = MatToBitmap(image);
            pictureBox_cam.Image = bitmap;
        }

        RefreshUi();
    }
    
    private void textBox_x_Leave(object sender, EventArgs e)
    {
        if (int.TryParse(textBox_x.Text, out var softCamWidth))
        {
            _settings.Width = MakeDivisible(softCamWidth);
            textBox_x.Text = _settings.Width.ToString();
        }
        else
            textBox_x.Text = _settings.Width.ToString();
    }

    private void textBox_y_Leave(object sender, EventArgs e)
    {
        if (int.TryParse(textBox_y.Text, out var softCamHeight))
        {
            _settings.Height = MakeDivisible(softCamHeight);
            textBox_y.Text = _settings.Height.ToString();
        }
        else
            textBox_y.Text = _settings.Height.ToString();
    }

    private void CheckBox_showStream_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ShowStream = checkBox_showStream.Checked;
    }

    private void ComboBox_cameras_SelectedIndexChanged(object sender, EventArgs e)
    {
        comboBox_camResolution.Items.Clear();
        var camera = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (camera == null)
            return;

        var cameraFormats = camera.Description.FrameFormats;
        foreach (var format in cameraFormats)
            comboBox_camResolution.Items.Add($"{format.Width}x{format.Height} {format.Format} {format.Fps}fps");

        if (comboBox_camResolution.Items.Count > 0)
            comboBox_camResolution.SelectedIndex = 0;

        RefreshUi();

        UserControl uiControl;
        if (camera is ScreenCamera scrCam)
            uiControl = new DesktopCameraSettingsPanel(scrCam, _settings.DesktopCamera, _cameraHub);
        else if (camera is VideoFileCamera)
            uiControl = new VideoFilePanel(_settings.VideoFileCamera, _cameraHub);
        else if (camera is ImageFileCamera)
            uiControl = new ImageFilePanel(_settings.ImageFileCamera, _cameraHub);
        else if (camera is IpCamera ipCam)
            uiControl = new IpCameraPropertyPanel(ipCam);
        else if (camera is MjpegCamera mCam)
            uiControl = new MjpegCameraPropertyPanel(mCam);
        else
            uiControl = new CommonCameraPropertyPanel(camera);

        uiControl.Dock = DockStyle.Fill;
        tabControl2.TabPages[0].Controls.Clear();
        tabControl2.TabPages[0].Controls.Add(uiControl);
    }

    private void comboBox_camResolution_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshUi();
    }
    #endregion

    #region Utilities
    public static bool IsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    private static Mat? ApplyFilters(Mat? image, FilterSettings filters, int softCamWidth, int softCamHeight)
    {
        if (image == null)
            return null;

        if (filters.Grayscale)
        {
            image = image.CvtColor(ColorConversionCodes.BGR2GRAY);
            image = image.CvtColor(ColorConversionCodes.GRAY2RGB);
        }

        var rotated = new Mat();
        if (filters.Rotate != null)
            Cv2.Rotate(image, rotated, (RotateFlags)filters.Rotate);
        else
            rotated = image;

        var resizedImage = ResizeFit(rotated, softCamWidth, softCamHeight);
        if (resizedImage == null)
            return null;

        if (filters.FlipHorizontal)
            resizedImage = resizedImage.Flip(FlipMode.Y);

        if (filters.FlipVertical)
            resizedImage = resizedImage.Flip(FlipMode.X);

        return resizedImage;
    }

    private static Mat? ResizeFit(Mat? image, int maxWidth, int maxHeight)
    {
        if (image == null)
            return image;

        var width = image.Width;
        var height = image.Height;

        if (maxHeight != height || maxWidth != width)
        {
            var scalingFactor = (double)maxHeight / (double)height;
            if ((double)maxWidth / (double)width < scalingFactor)
                scalingFactor = (double)maxWidth / (double)width;

            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);
            if (width > maxWidth)
                width = maxWidth;

            if (height > maxHeight)
                height = maxHeight;

            var result = new Mat();
            Cv2.Resize(image, result, new Size(width, height), 0, 0, InterpolationFlags.Cubic);

            var top = (maxHeight - result.Height) / 2;
            var bottom = maxHeight - (result.Height + top);
            var left = (maxWidth - result.Width) / 2;
            var right = maxWidth - (result.Width + left);
            if (top == 0 && bottom == 0 && left == 0 && right == 0)
                return result;

            Cv2.CopyMakeBorder(result, image, top, bottom, left, right, BorderTypes.Constant, Scalar.Gray);
            result.Dispose();
        }

        return image;
    }

    private static Bitmap? MatToBitmap(Mat? mat)
    {
        if (mat == null)
            return null;

        using (var ms = mat.ToMemoryStream())
        {
            return (Bitmap)Image.FromStream(ms);
        }
    }

    private static int MakeDivisible(int number)
    {
        int div = number / 4;
        if (number % 4 > 0)
            number = div * 4;

        return number;
    }

    #endregion
}