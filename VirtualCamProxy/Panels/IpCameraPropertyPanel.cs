using CameraLib.IP;

namespace VirtualCamProxy.Panels
{
    public partial class IpCameraPropertyPanel : UserControl
    {
        private readonly IpCamera _camera;

        public IpCameraPropertyPanel(IpCamera camera)
        {
            InitializeComponent();
            _camera = camera;

            textBox_path.Text = _camera.Description.Path;
            textBox_name.Text = _camera.Description.Name;
            textBox_auth.Text = _camera.AuthenicationType.ToString();
            textBox_login.Text = _camera.Login;
            textBox_password.Text = _camera.Password;
        }
    }
}
