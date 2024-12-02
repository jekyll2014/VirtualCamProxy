using CameraExtension;

using VirtualCamProxy.Settings;

namespace VirtualCamProxy;
public partial class ImageFilePanel : UserControl
{
    private readonly ImageFileCameraSettings _settings;
    private readonly CameraHubService _cameraHub;

    public ImageFilePanel(ImageFileCameraSettings settings, CameraHubService cameraHub)
    {
        InitializeComponent();
        _settings = settings;
        _cameraHub = cameraHub;

        textBox_imageDelay.Text = _settings.Delay.ToString();
        textBox_selectedImageFolder.Text = _settings.FolderName;

        radioButton_imageFolderChecked.Checked = _settings.Container == FileContainer.Folder;
        radioButton_imageFilesChecked.Checked = _settings.Container == FileContainer.FileList;
        textBox_selectedImageFiles.Text = string.Join("\r\n", _settings.FileNames);
    }

    private void Button_selectImageFolder_Click(object sender, EventArgs e)
    {
        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            var folderName = folderBrowserDialog1.SelectedPath;
            textBox_selectedImageFolder.Text = folderName;
            _settings.FolderName = folderName;
        }
    }

    private void Button_selectImageFiles_Click(object sender, EventArgs e)
    {
        openFileDialog1.FileName = "";
        openFileDialog1.Title = "Open image files";
        openFileDialog1.DefaultExt = "jpg";
        openFileDialog1.Filter = "JPG files|*.jpg|BMP files|*.bmp|PNG files|*.png|All files|*.*";
        openFileDialog1.Multiselect = true;

        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            _settings.FileNames.Clear();
            _settings.FileNames.AddRange(openFileDialog1.FileNames);
            textBox_selectedImageFiles.Text = string.Join("\r\n", _settings.FileNames);
        }
    }

    private void TextBox_imageDelay_TextChanged(object sender, EventArgs e)
    {
        if (int.TryParse(textBox_imageDelay.Text, out var delay))
        {
            _settings.Delay = delay;
            if (_cameraHub.CurrentCamera is ImageFileCamera icam)
                icam.Delay = _settings.Delay;
        }
        else
            textBox_imageDelay.Text = _settings.Delay.ToString();
    }

    private void RadioButton_imageFolderChecked_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButton_imageFolderChecked.Checked)
        {
            _settings.Container = FileContainer.Folder;
            if (_cameraHub.CurrentCamera is ImageFileCamera iCam)
                iCam.SetFile(_settings.Files);
        }
    }

    private void RadioButton_imageFilesChecked_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButton_imageFilesChecked.Checked)
        {
            _settings.Container = FileContainer.FileList;
            if (_cameraHub.CurrentCamera is ImageFileCamera iCam)
                iCam.SetFile(_settings.Files);
        }
    }
}
