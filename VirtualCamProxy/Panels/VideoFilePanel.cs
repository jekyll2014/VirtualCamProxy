using CameraExtension;

using System.Collections.Generic;

using VirtualCamProxy.Settings;

namespace VirtualCamProxy;
public partial class VideoFilePanel : UserControl
{
    private readonly VideoFileCameraSettings _settings;
    private readonly CameraHubService _cameraHub;

    public VideoFilePanel(VideoFileCameraSettings settings, CameraHubService cameraHub)
    {
        InitializeComponent();
        _settings = settings;
        _cameraHub = cameraHub;

        textBox_selectedVideoFiles.Text = string.Join("\r\n", _settings.FileNames);
        textBox_selectedVideoFolder.Text = _settings.FolderName;
        checkBox_repeatFile.Checked = _settings.Repeat;
        radioButton_videoFolderChecked.Checked = _settings.Container == FileContainer.Folder;
        radioButton_videoFilesChecked.Checked = _settings.Container == FileContainer.FileList;
    }

    private void Button_selectVideoFiles_Click(object sender, EventArgs e)
    {
        openFileDialog1.FileName = "";
        openFileDialog1.Title = "Open video file";
        openFileDialog1.DefaultExt = "avi";
        openFileDialog1.Filter = "AVI files|*.avi|MP4 files|*.mp4|MKV files|*.mkv|All files|*.*";
        openFileDialog1.Multiselect = true;

        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            _settings.FileNames.Clear();
            _settings.FileNames.AddRange(openFileDialog1.FileNames);
            textBox_selectedVideoFiles.Text = string.Join("\r\n", _settings.FileNames);
        }
    }

    private void Button_selectVideoFolder_Click(object sender, EventArgs e)
    {
        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
        {
            var folderName = folderBrowserDialog1.SelectedPath;
            textBox_selectedVideoFolder.Text = folderName;
            _settings.FolderName = folderName;
        }
    }

    private void CheckBox_repeatFile_CheckedChanged(object sender, EventArgs e)
    {
        _settings.Repeat = checkBox_repeatFile.Checked;
        if (_cameraHub.CurrentCamera is VideoFileCamera vcam)
            vcam.RepeatFile = _settings.Repeat;
    }

    private void RadioButton_videoFolderChecked_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButton_videoFolderChecked.Checked)
        {
            _settings.Container = FileContainer.Folder;
            if (_cameraHub.CurrentCamera is VideoFileCamera vcam)
                vcam.SetFile(_settings.Files);
        }
    }

    private void RadioButton_videoFilesChecked_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButton_videoFilesChecked.Checked)
        {
            _settings.Container = FileContainer.FileList;
            if (_cameraHub.CurrentCamera is VideoFileCamera vcam)
                vcam.SetFile(_settings.Files);
        }
    }
}
