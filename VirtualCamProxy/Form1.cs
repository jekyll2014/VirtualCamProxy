using CameraExtension;

using CameraLib;
using CameraLib.IP;
using CameraLib.MJPEG;

using OpenCvSharp;

using VirtualCamera;

using VirtualCamProxy.Panels;
using VirtualCamProxy.Settings;

using Size = OpenCvSharp.Size;

namespace VirtualCamProxy;

public partial class Form1 : Form
{
    private const string ConfigFileName = "appsettings.json";
    private const string SoftCamName = "DirectShow Softcam";
    private readonly JsonStorage<SoftCameraSettings> _configuration = new JsonStorage<SoftCameraSettings>(ConfigFileName, true);
    private SoftCameraSettings _settings = new SoftCameraSettings();
    private Task? _cameraFeed;
    private CancellationToken? _cancellationToken;
    private CameraHubService _cameraHub;
    private bool _softCameraStarted = false;
    private bool _cameraStarted = false;
    private SoftCamera? _softCamera;
    private ICamera? _currentSource;

    public Form1()
    {
        InitializeComponent();
        _settings = _configuration.Storage;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        _cameraHub = new CameraHubService(_settings.Cameras);
        textBox_x.Text = _settings.Width.ToString();
        textBox_y.Text = _settings.Height.ToString();
        checkBox_showStream.Checked = _settings.ShowStream;
        Button_refresh_Click(this, EventArgs.Empty);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {

        if (!(_cameraFeed?.IsCompleted ?? true))
        {
            Button_camStop_Click(this, EventArgs.Empty);
            Button_softCamStop_Click(this, EventArgs.Empty);
        }

        _cameraHub.ImageQueue.Clear();
        _configuration.Save();
        if (_cameraFeed?.IsCompleted ?? false)
            _cameraFeed?.Dispose();

        _softCamera?.Dispose();
    }

    #region Camera tab
    private void Button_softCamStart_Click(object sender, EventArgs e)
    {
        if (_settings.Width <= 0 || _settings.Height <= 0)
            return;

        var started = _cameraStarted;
        if (started)
            Button_camStop_Click(this, EventArgs.Empty);

        button_softCamStart.Enabled = false;
        textBox_x.Enabled = false;
        textBox_y.Enabled = false;
        _softCamera = new SoftCamera(_settings.Width, _settings.Height);
        _softCameraStarted = true;

        if (started)
            Button_camStart_Click(this, EventArgs.Empty);

        button_softCamStop.Enabled = true;
    }

    private void Button_softCamStop_Click(object sender, EventArgs e)
    {
        if (_cameraStarted)
            Button_camStop_Click(this, EventArgs.Empty);

        button_softCamStop.Enabled = false;
        _softCamera?.Dispose();
        _softCameraStarted = false;
        textBox_x.Enabled = true;
        textBox_y.Enabled = true;
        button_softCamStart.Enabled = true;
    }

    private async void Button_camStart_Click(object sender, EventArgs e)
    {
        button_camStart.Enabled = false;
        button_camStop.Enabled = false;
        button_camGetImage.Enabled = false;
        button_refresh.Enabled = false;
        comboBox_cameras.Enabled = false;
        comboBox_camResolution.Enabled = false;
        textBox_currentSource.Clear();

        _currentSource = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (_currentSource == null)
        {
            button_camStart.Enabled = true;
            button_camStop.Enabled = false;
            button_camGetImage.Enabled = false;
            button_refresh.Enabled = true;
            comboBox_cameras.Enabled = true;
            comboBox_camResolution.Enabled = true;

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
            icam.SetFile(_settings.ImageFileCamera.Files);
        }

        _cancellationToken = await _cameraHub.HookCamera(comboBox_cameras.SelectedIndex, w, h, "");
        if (_cancellationToken == CancellationToken.None)
        {
            _currentSource.Stop();

            button_camStart.Enabled = false;
            button_camStop.Enabled = true;
            button_camGetImage.Enabled = true;
            button_refresh.Enabled = false;
            comboBox_cameras.Enabled = false;
            comboBox_camResolution.Enabled = false;

            return;
        }


        textBox_currentSource.Text = _currentSource.Description.Name;
        _cameraFeed = Task.Run(CameraFeed, (CancellationToken)_cancellationToken);

        button_camStart.Enabled = false;
        button_camStop.Enabled = true;
        button_camGetImage.Enabled = true;
        button_refresh.Enabled = false;
        comboBox_cameras.Enabled = false;
        comboBox_camResolution.Enabled = false;
    }

    private async void Button_camStop_Click(object sender, EventArgs e)
    {
        button_camStart.Enabled = false;
        button_camStop.Enabled = false;
        button_camGetImage.Enabled = false;
        button_refresh.Enabled = false;
        comboBox_cameras.Enabled = false;
        comboBox_camResolution.Enabled = false;
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

        textBox_currentSource.Clear();
        button_camStart.Enabled = true;
        button_camStop.Enabled = false;
        button_camGetImage.Enabled = false;
        button_refresh.Enabled = true;
        comboBox_cameras.Enabled = true;
        comboBox_camResolution.Enabled = true;
    }

    private async void Button_refresh_Click(object sender, EventArgs e)
    {
        button_refresh.Enabled = false;
        button_camStart.Enabled = false;
        button_camStop.Enabled = false;
        button_camGetImage.Enabled = false;
        comboBox_cameras.Enabled = false;
        comboBox_camResolution.Enabled = false;
        comboBox_cameras.Items.Clear();
        await _cameraHub.RefreshCameraCollection(CancellationToken.None);
        _cameraHub.Cameras.RemoveAll(n => n.Description.Name == SoftCamName);
        var i = 1;
        foreach (var cam in _cameraHub.Cameras)
            comboBox_cameras.Items.Add($"{i++} {cam.Description.Name} [{cam.GetType().Name}]");

        if (_cameraHub.Cameras.Count > 0)
            comboBox_cameras.SelectedIndex = 0;

        button_refresh.Enabled = true;
        button_camStart.Enabled = true;
        button_camStop.Enabled = false;
        button_camGetImage.Enabled = true;
        comboBox_cameras.Enabled = true;
        comboBox_camResolution.Enabled = true;
    }

    private async void Button_camGetImage_Click(object sender, EventArgs e)
    {
        var camera = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex);
        if (camera == null)
            return;

        var image = await camera.GrabFrame(CancellationToken.None);

        if (image != null)
        {
            var bitmap = MatToBitmap(image);
            pictureBox_cam.Image = bitmap;
        }
    }

    private async Task CameraFeed()
    {
        _cameraStarted = true;
        var softCamWidth = _settings.Width;
        var softCamHeigth = _settings.Height;
        while (!(_cancellationToken?.IsCancellationRequested ?? true))
        {
            //if ((_softCamera?.AppIsConnected ?? false) && _cameraHub.ImageQueue.TryDequeue(out var image))
            if (_cameraHub.ImageQueue.TryDequeue(out var image) && _softCamera != null)
            {
                var rotated = new Mat();
                if (_settings.Filters.Rotate != null)
                    Cv2.Rotate(image, rotated, (RotateFlags)_settings.Filters.Rotate);
                else
                    rotated = image;

                var resizedImage = ResizeFit(rotated, softCamWidth, softCamHeigth);
                if (resizedImage == null)
                    continue;

                if (_settings.Filters.FlipHorizontal)
                {
                    resizedImage = resizedImage.Flip(FlipMode.Y);
                }

                if (_settings.Filters.FlipVertical)
                {

                    resizedImage = resizedImage.Flip(FlipMode.X);
                }

                /*
                var buffer = Mat_to_array(image);
                _softCamera.PushFrame(buffer);
                */

                var bitmap = MatToBitmap(resizedImage);
                if (bitmap != null)
                {
                    _softCamera.PushFrame(bitmap);

                    if (_settings.ShowStream)
                    {
                        var bmp = (Bitmap)bitmap.Clone();
                        this.Invoke(() =>
                        {
                            pictureBox_cam.Image = bmp;
                        });
                    }

                    bitmap.Dispose();
                }

                resizedImage.Dispose();
                image.Dispose();
            }
            else
            {
                await Task.Delay(20);
            }
        }

        _cameraStarted = false;
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
        }

        return image;
    }

    private static Bitmap MatToBitmap(Mat mat)
    {
        using (var ms = mat.ToMemoryStream())
        {
            return (Bitmap)Image.FromStream(ms);
        }
    }

    private void TextBox_x_TextChanged(object sender, EventArgs e)
    {
        if (int.TryParse(textBox_x.Text, out var softCamWidth))
            _settings.Width = softCamWidth;
        else
            textBox_x.Text = _settings.Width.ToString();
    }

    private void TextBox_y_TextChanged(object sender, EventArgs e)
    {
        if (int.TryParse(textBox_y.Text, out var softCamHeigth))
            _settings.Height = softCamHeigth;
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
        {
            comboBox_camResolution.SelectedIndex = 0;
            button_camStart.Enabled = true;
            button_camStop.Enabled = false;
            button_camGetImage.Enabled = true;
            button_refresh.Enabled = true;
            comboBox_cameras.Enabled = true;
            UserControl uiControl;
            if (camera is ScreenCamera)
                uiControl = new DesktopCameraSettingsPanel(_settings.DesktopCamera, _cameraHub);
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

            uiControl.AutoSize = true;
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(uiControl);
        }
        else
        {
            button_camStart.Enabled = false;
            button_camStop.Enabled = false;
            button_camGetImage.Enabled = false;
            button_refresh.Enabled = true;
            comboBox_cameras.Enabled = true;
            textBox_currentSource.Clear();
        }
    }
    #endregion

    #region Filters tab
    private void CheckBox_flipHorizontal_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Filters.FlipHorizontal = checkBox_flipHorizontal.Checked;
    }

    private void CheckBox_flipVertical_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Filters.FlipVertical = checkBox_flipVertical.Checked;
    }

    private void RadioButton_rotateNone_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Filters.Rotate = null;
    }

    private void RadioButton_rotate90_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Filters.Rotate = RotateFlags.Rotate90Clockwise;
    }

    private void RadioButton_rotate180_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Filters.Rotate = RotateFlags.Rotate180;
    }

    private void RadioButton_rotate270_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Filters.Rotate = RotateFlags.Rotate90Counterclockwise;
    }
    #endregion
}