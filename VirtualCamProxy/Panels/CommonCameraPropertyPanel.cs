using CameraLib;

namespace VirtualCamProxy.Panels
{

    public partial class CommonCameraPropertyPanel : UserControl
    {
        private readonly ICamera _camera;

        public CommonCameraPropertyPanel(ICamera camera)
        {
            InitializeComponent();
            _camera = camera;

            textBox_path.Text = _camera.Description.Path;
            textBox_name.Text = _camera.Description.Name;
        }
    }
}
