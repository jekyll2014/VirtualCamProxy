using CameraLib.MJPEG;

namespace VirtualCamProxy.Panels
{
    public partial class MjpegCameraPropertyPanel : UserControl
    {
        private readonly MjpegCamera _camera;

        public MjpegCameraPropertyPanel(MjpegCamera camera)
        {
            InitializeComponent();
            _camera = camera;

            textBox_path.Text = _camera.Description.Path;
            textBox_name.Text = _camera.Description.Name;
            textBox_auth.Text = _camera.AuthenticationType.ToString();
            textBox_login.Text = _camera.Login;
            textBox_password.Text = _camera.Password;
        }
    }
}
