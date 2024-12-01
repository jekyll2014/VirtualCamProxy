using CameraLib;
using CameraLib.Screen;

using OpenCvSharp;

using VirtualCamera;

using Size = OpenCvSharp.Size;

namespace VirtualCamProxy
{
    public partial class Form1 : Form
    {
        private const string ConfigFileName = "appsettings.json";
        private const string SoftCamName = "DirectShow Softcam";
        private readonly JsonStorage<SoftCameraSettings> _configuration = new JsonStorage<SoftCameraSettings>(ConfigFileName, true);
        private Task? _cameraFeed;
        private CancellationToken? _cancellationToken;
        private readonly CameraHubService _cameraHub = new CameraHubService(new CameraSettings());
        private bool _softCameStarted = false;
        private bool _cameraStarted = false;
        private SoftCamera? _softCamera;
        private ICamera? _currentSource;
        private bool _showStream = false;
        private bool _showCursor = false;
        private bool _repeatFile = false;
        private List<string> _videoFiles = new List<string>();
        private List<string> _videoFileNames = new List<string>();
        private string _videoFolderName = string.Empty;
        private bool _flipHorizontal = false;
        private bool _flipVertical = false;

        private RotateFlags? _rotate = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_x.Text = _configuration.Storage.Width.ToString();
            textBox_y.Text = _configuration.Storage.Height.ToString();
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
            if (_configuration.Storage.Width <= 0 || _configuration.Storage.Height <= 0)
                return;

            var started = _cameraStarted;
            if (started)
                Button_camStop_Click(this, EventArgs.Empty);

            button_softCamStart.Enabled = false;
            textBox_x.Enabled = false;
            textBox_y.Enabled = false;
            _softCamera = new SoftCamera(_configuration.Storage.Width, _configuration.Storage.Height);
            _softCameStarted = true;

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
            _softCameStarted = false;
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
                scam.ShowCursor = _showCursor;
            }
            else if (currentCam is VideoFileCamera vcam)
            {
                vcam.RepeatFile = _repeatFile;
                vcam.SetFile(_videoFiles);
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
            var softCamWidth = _configuration.Storage.Width;
            var softCamHeigth = _configuration.Storage.Height;
            while (!(_cancellationToken?.IsCancellationRequested ?? true))
            {
                //if ((_softCamera?.AppIsConnected ?? false) && _cameraHub.ImageQueue.TryDequeue(out var image))
                if (_cameraHub.ImageQueue.TryDequeue(out var image) && _softCamera != null)
                {
                    var rotated = new Mat();
                    if (_rotate != null)
                        Cv2.Rotate(image, rotated, (RotateFlags)_rotate);
                    else
                        rotated = image;

                    var resizedImage = ResizeFit(rotated, softCamWidth, softCamHeigth);
                    if (resizedImage == null)
                        continue;

                    if (_flipHorizontal)
                    {
                        resizedImage = resizedImage.Flip(FlipMode.Y);
                    }

                    if (_flipVertical)
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

                        if (_showStream)
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

        /*
        private static byte[] _buffer;
        private static int _bufferWidth;
        static unsafe void Operation(Vec3b* pixel, int* position)
        {
            var row = position[0];
            var col = position[1];
            _buffer[col + row * _bufferWidth] = pixel->Item0;
            _buffer[col + row * _bufferWidth + 1] = pixel->Item1;
            _buffer[col + row * _bufferWidth + 2] = pixel->Item2;
        }

        private static byte[] Mat_to_array(Mat input)
        {
            var height = input.Height;
            var width = input.Width;
            _bufferWidth = width;
            _buffer = new byte[height * width * 3];

            unsafe
            {
                input.ForEachAsVec3b(Operation);
            }

            return _buffer;
        }
        */

        private void TextBox_x_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox_x.Text, out var softCamWidth))
                _configuration.Storage.Width = softCamWidth;
            else
                textBox_x.Text = _configuration.Storage.Width.ToString();
        }

        private void TextBox_y_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox_y.Text, out var softCamHeigth))
                _configuration.Storage.Height = softCamHeigth;
            else
                textBox_y.Text = _configuration.Storage.Height.ToString();
        }

        private void checkBox_showStream_CheckedChanged(object sender, EventArgs e)
        {
            _showStream = checkBox_showStream.Checked;
        }

        private void ComboBox_cameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_camResolution.Items.Clear();
            var cameraFormats = _cameraHub.GetCamera(comboBox_cameras.SelectedIndex)?.Description.FrameFormats;
            if (cameraFormats == null)
                return;

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
            }
            else
            {
                button_camStart.Enabled = false;
                button_camStop.Enabled = false;
                button_camGetImage.Enabled = false;
                button_refresh.Enabled = true;
                comboBox_cameras.Enabled = false;
                textBox_currentSource.Clear();
            }
        }
        #endregion

        #region Desktop tab
        private void checkBox_showCursor_CheckedChanged(object sender, EventArgs e)
        {
            _showCursor = checkBox_showCursor.Checked;
            if (_cameraHub.CurrentCamera is ScreenCamera scam)
                scam.ShowCursor = _showCursor;
        }
        #endregion

        #region Image tab

        #endregion

        #region Video tab
        private async void button_selectVideoFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Open video file";
            openFileDialog1.DefaultExt = "avi";
            openFileDialog1.Filter = "AVI files|*.avi|MP4 files|*.mp4|MKV files|*.mkv|All files|*.*";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _videoFileNames.Clear();
                _videoFileNames.AddRange(openFileDialog1.FileNames);
                textBox_selectedVideoFile.Text = string.Join("\r\n", _videoFileNames);
            }
        }

        private void button_selectVideoFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var folderName = folderBrowserDialog1.SelectedPath;
                textBox_selectedVideoFolder.Text = folderName;
                _videoFolderName = folderName;
            }
        }

        private void checkBox_repeatFile_CheckedChanged(object sender, EventArgs e)
        {
            _repeatFile = checkBox_repeatFile.Checked;
            if (_cameraHub.CurrentCamera is VideoFileCamera vcam)
                vcam.RepeatFile = _repeatFile;
        }

        private void radioButton_videoFolderChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_videoFolderChecked.Checked)
            {
                _videoFiles.Clear();
                _videoFiles.AddRange(Directory.GetFiles(_videoFolderName));
                if (_cameraHub.CurrentCamera is VideoFileCamera vcam)
                    vcam.SetFile(_videoFiles);
            }
        }

        private void radioButton_videoFilesChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_videoFilesChecked.Checked)
            {
                _videoFiles.Clear();
                _videoFiles.AddRange(_videoFileNames);
                if (_cameraHub.CurrentCamera is VideoFileCamera vcam)
                    vcam.SetFile(_videoFiles);
            }
        }
        #endregion

        #region Filters tab
        private void checkBox_flipHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            _flipHorizontal = checkBox_flipHorizontal.Checked;
        }

        private void checkBox_flipVertical_CheckedChanged(object sender, EventArgs e)
        {
            _flipVertical = checkBox_flipVertical.Checked;
        }

        private void radioButton_rotateNone_CheckedChanged(object sender, EventArgs e)
        {
            _rotate = null;
        }

        private void radioButton_rotate90_CheckedChanged(object sender, EventArgs e)
        {
            _rotate = RotateFlags.Rotate90Clockwise;
        }

        private void radioButton_rotate180_CheckedChanged(object sender, EventArgs e)
        {
            _rotate = RotateFlags.Rotate180;
        }

        private void radioButton_rotate270_CheckedChanged(object sender, EventArgs e)
        {
            _rotate = RotateFlags.Rotate90Counterclockwise;
        }
        #endregion
    }
}
