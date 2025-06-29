namespace VirtualCamProxy
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_camStart = new Button();
            comboBox_cameras = new ComboBox();
            pictureBox_cam = new PictureBox();
            button_refreshAll = new Button();
            button_camGetImage = new Button();
            splitContainer1 = new SplitContainer();
            checkBox_showStream = new CheckBox();
            comboBox_camResolution = new ComboBox();
            button_camStop = new Button();
            button_refresh = new Button();
            tabControl2 = new TabControl();
            tabPage_cameraProperties = new TabPage();
            tabPage_filters2 = new TabPage();
            button_softCamStop = new Button();
            button_softCamStart = new Button();
            label_x = new Label();
            textBox_x = new TextBox();
            label2 = new Label();
            label_y = new Label();
            textBox_y = new TextBox();
            label_currentSource = new Label();
            textBox_currentSource = new TextBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            openFileDialog1 = new OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)pictureBox_cam).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl2.SuspendLayout();
            SuspendLayout();
            // 
            // button_camStart
            // 
            button_camStart.Location = new Point(3, 61);
            button_camStart.Name = "button_camStart";
            button_camStart.Size = new Size(75, 23);
            button_camStart.TabIndex = 0;
            button_camStart.Text = "Start";
            button_camStart.UseVisualStyleBackColor = true;
            button_camStart.Click += Button_camStart_Click;
            // 
            // comboBox_cameras
            // 
            comboBox_cameras.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox_cameras.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_cameras.FormattingEnabled = true;
            comboBox_cameras.Location = new Point(3, 3);
            comboBox_cameras.Name = "comboBox_cameras";
            comboBox_cameras.Size = new Size(272, 23);
            comboBox_cameras.TabIndex = 1;
            comboBox_cameras.SelectedIndexChanged += ComboBox_cameras_SelectedIndexChanged;
            // 
            // pictureBox_cam
            // 
            pictureBox_cam.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox_cam.Location = new Point(3, 90);
            pictureBox_cam.Name = "pictureBox_cam";
            pictureBox_cam.Size = new Size(364, 205);
            pictureBox_cam.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_cam.TabIndex = 2;
            pictureBox_cam.TabStop = false;
            // 
            // button_refreshAll
            // 
            button_refreshAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_refreshAll.Location = new Point(281, 3);
            button_refreshAll.Name = "button_refreshAll";
            button_refreshAll.Size = new Size(86, 23);
            button_refreshAll.TabIndex = 0;
            button_refreshAll.Text = "Refresh all";
            button_refreshAll.UseVisualStyleBackColor = true;
            button_refreshAll.Click += Button_refreshAll_Click;
            // 
            // button_camGetImage
            // 
            button_camGetImage.Location = new Point(165, 61);
            button_camGetImage.Name = "button_camGetImage";
            button_camGetImage.Size = new Size(75, 23);
            button_camGetImage.TabIndex = 0;
            button_camGetImage.Text = "Get image";
            button_camGetImage.UseVisualStyleBackColor = true;
            button_camGetImage.Click += Button_camGetImage_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Location = new Point(12, 56);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(comboBox_cameras);
            splitContainer1.Panel1.Controls.Add(checkBox_showStream);
            splitContainer1.Panel1.Controls.Add(button_camStart);
            splitContainer1.Panel1.Controls.Add(comboBox_camResolution);
            splitContainer1.Panel1.Controls.Add(button_camStop);
            splitContainer1.Panel1.Controls.Add(button_refresh);
            splitContainer1.Panel1.Controls.Add(button_refreshAll);
            splitContainer1.Panel1.Controls.Add(pictureBox_cam);
            splitContainer1.Panel1.Controls.Add(button_camGetImage);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl2);
            splitContainer1.Size = new Size(760, 302);
            splitContainer1.SplitterDistance = 374;
            splitContainer1.TabIndex = 4;
            // 
            // checkBox_showStream
            // 
            checkBox_showStream.AutoSize = true;
            checkBox_showStream.Location = new Point(246, 64);
            checkBox_showStream.Name = "checkBox_showStream";
            checkBox_showStream.Size = new Size(94, 19);
            checkBox_showStream.TabIndex = 3;
            checkBox_showStream.Text = "Show stream";
            checkBox_showStream.UseVisualStyleBackColor = true;
            checkBox_showStream.CheckedChanged += CheckBox_showStream_CheckedChanged;
            // 
            // comboBox_camResolution
            // 
            comboBox_camResolution.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox_camResolution.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_camResolution.FormattingEnabled = true;
            comboBox_camResolution.Location = new Point(3, 32);
            comboBox_camResolution.Name = "comboBox_camResolution";
            comboBox_camResolution.Size = new Size(272, 23);
            comboBox_camResolution.TabIndex = 1;
            comboBox_camResolution.SelectedIndexChanged += ComboBox_camResolution_SelectedIndexChanged;
            // 
            // button_camStop
            // 
            button_camStop.Location = new Point(84, 61);
            button_camStop.Name = "button_camStop";
            button_camStop.Size = new Size(75, 23);
            button_camStop.TabIndex = 0;
            button_camStop.Text = "Stop";
            button_camStop.UseVisualStyleBackColor = true;
            button_camStop.Click += Button_camStop_Click;
            // 
            // button_refresh
            // 
            button_refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_refresh.Location = new Point(281, 31);
            button_refresh.Name = "button_refresh";
            button_refresh.Size = new Size(86, 23);
            button_refresh.TabIndex = 0;
            button_refresh.Text = "Refresh";
            button_refresh.UseVisualStyleBackColor = true;
            button_refresh.Click += Button_refresh_Click;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPage_cameraProperties);
            tabControl2.Controls.Add(tabPage_filters2);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new Point(0, 0);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(378, 298);
            tabControl2.TabIndex = 0;
            // 
            // tabPage_cameraProperties
            // 
            tabPage_cameraProperties.Location = new Point(4, 24);
            tabPage_cameraProperties.Name = "tabPage_cameraProperties";
            tabPage_cameraProperties.Padding = new Padding(3);
            tabPage_cameraProperties.Size = new Size(370, 270);
            tabPage_cameraProperties.TabIndex = 0;
            tabPage_cameraProperties.Text = "Camera properties";
            tabPage_cameraProperties.UseVisualStyleBackColor = true;
            // 
            // tabPage_filters2
            // 
            tabPage_filters2.Location = new Point(4, 24);
            tabPage_filters2.Name = "tabPage_filters2";
            tabPage_filters2.Padding = new Padding(3);
            tabPage_filters2.Size = new Size(370, 270);
            tabPage_filters2.TabIndex = 1;
            tabPage_filters2.Text = "Filters";
            tabPage_filters2.UseVisualStyleBackColor = true;
            // 
            // button_softCamStop
            // 
            button_softCamStop.Location = new Point(251, 12);
            button_softCamStop.Name = "button_softCamStop";
            button_softCamStop.Size = new Size(75, 38);
            button_softCamStop.TabIndex = 4;
            button_softCamStop.Text = "Stop";
            button_softCamStop.UseVisualStyleBackColor = true;
            button_softCamStop.Click += Button_softCamStop_Click;
            // 
            // button_softCamStart
            // 
            button_softCamStart.Location = new Point(170, 12);
            button_softCamStart.Name = "button_softCamStart";
            button_softCamStart.Size = new Size(75, 38);
            button_softCamStart.TabIndex = 5;
            button_softCamStart.Text = "Start";
            button_softCamStart.UseVisualStyleBackColor = true;
            button_softCamStart.Click += Button_softCamStart_Click;
            // 
            // label_x
            // 
            label_x.AutoSize = true;
            label_x.Location = new Point(13, 30);
            label_x.Name = "label_x";
            label_x.Size = new Size(22, 15);
            label_x.TabIndex = 6;
            label_x.Text = "X=";
            // 
            // textBox_x
            // 
            textBox_x.Location = new Point(42, 27);
            textBox_x.MaxLength = 4;
            textBox_x.Name = "textBox_x";
            textBox_x.Size = new Size(44, 23);
            textBox_x.TabIndex = 7;
            textBox_x.Text = "1920";
            textBox_x.Leave += TextBox_x_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
            label2.TabIndex = 6;
            label2.Text = "SoftCam resolution";
            // 
            // label_y
            // 
            label_y.AutoSize = true;
            label_y.Location = new Point(92, 30);
            label_y.Name = "label_y";
            label_y.Size = new Size(22, 15);
            label_y.TabIndex = 6;
            label_y.Text = "Y=";
            // 
            // textBox_y
            // 
            textBox_y.Location = new Point(120, 27);
            textBox_y.MaxLength = 4;
            textBox_y.Name = "textBox_y";
            textBox_y.Size = new Size(44, 23);
            textBox_y.TabIndex = 7;
            textBox_y.Text = "1080";
            textBox_y.Leave += TextBox_y_Leave;
            // 
            // label_currentSource
            // 
            label_currentSource.AutoSize = true;
            label_currentSource.Location = new Point(332, 9);
            label_currentSource.Name = "label_currentSource";
            label_currentSource.Size = new Size(85, 15);
            label_currentSource.TabIndex = 6;
            label_currentSource.Text = "Current source";
            // 
            // textBox_currentSource
            // 
            textBox_currentSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_currentSource.Location = new Point(332, 27);
            textBox_currentSource.MaxLength = 4;
            textBox_currentSource.Name = "textBox_currentSource";
            textBox_currentSource.ReadOnly = true;
            textBox_currentSource.Size = new Size(440, 23);
            textBox_currentSource.TabIndex = 7;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 370);
            Controls.Add(splitContainer1);
            Controls.Add(textBox_y);
            Controls.Add(textBox_currentSource);
            Controls.Add(textBox_x);
            Controls.Add(label_y);
            Controls.Add(label_currentSource);
            Controls.Add(label2);
            Controls.Add(label_x);
            Controls.Add(button_softCamStop);
            Controls.Add(button_softCamStart);
            Name = "Form1";
            Text = "VirtualCamProxy";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox_cam).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_camStart;
        private ComboBox comboBox_cameras;
        private PictureBox pictureBox_cam;
        private Button button_refreshAll;
        private Button button_camGetImage;
        private Button button_camStop;
        private Button button_softCamStop;
        private Button button_softCamStart;
        private Label label_x;
        private TextBox textBox_x;
        private Label label2;
        private Label label_y;
        private TextBox textBox_y;
        private Label label_currentSource;
        private TextBox textBox_currentSource;
        private ComboBox comboBox_camResolution;
        private FolderBrowserDialog folderBrowserDialog1;
        private OpenFileDialog openFileDialog1;
        private CheckBox checkBox_showStream;
        private SplitContainer splitContainer1;
        private TabControl tabControl2;
        private TabPage tabPage_cameraProperties;
        private TabPage tabPage_filters2;
        private Button button_refresh;
    }
}
