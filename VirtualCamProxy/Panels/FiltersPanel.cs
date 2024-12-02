using OpenCvSharp;

using VirtualCamProxy.Settings;

namespace VirtualCamProxy
{
    public partial class FiltersPanel : UserControl
    {
        private readonly FilterSettings _settings;
        public FiltersPanel(FilterSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            checkBox_flipHorizontal.Checked = _settings.FlipHorizontal;
            checkBox_flipVertical.Checked = _settings.FlipVertical;
            radioButton_rotateNone.Checked = _settings.Rotate == null;
            radioButton_rotate90.Checked = _settings.Rotate == RotateFlags.Rotate90Clockwise;
            radioButton_rotate180.Checked = _settings.Rotate == RotateFlags.Rotate180;
            radioButton_rotate270.Checked = _settings.Rotate == RotateFlags.Rotate90Counterclockwise;
        }

        private void CheckBox_flipHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlipHorizontal = checkBox_flipHorizontal.Checked;
        }

        private void CheckBox_flipVertical_CheckedChanged(object sender, EventArgs e)
        {
            _settings.FlipVertical = checkBox_flipVertical.Checked;
        }

        private void RadioButton_rotateNone_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Rotate = null;
        }

        private void RadioButton_rotate90_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Rotate = RotateFlags.Rotate90Clockwise;
        }

        private void RadioButton_rotate180_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Rotate = RotateFlags.Rotate180;
        }

        private void RadioButton_rotate270_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Rotate = RotateFlags.Rotate90Counterclockwise;
        }
    }
}
