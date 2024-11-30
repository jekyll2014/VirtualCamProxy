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
            button_refresh = new Button();
            button_camGetImage = new Button();
            tabControl1 = new TabControl();
            tabPage_cam = new TabPage();
            comboBox_camResolution = new ComboBox();
            button_camStop = new Button();
            tabPage_images = new TabPage();
            tabPage_video = new TabPage();
            button_softCamStop = new Button();
            button_softCamStart = new Button();
            label_x = new Label();
            textBox_x = new TextBox();
            label2 = new Label();
            label_y = new Label();
            textBox_y = new TextBox();
            label_currentSource = new Label();
            textBox_currentSource = new TextBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            textBox1 = new TextBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            button1 = new Button();
            textBox2 = new TextBox();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            openFileDialog1 = new OpenFileDialog();
            label1 = new Label();
            textBox3 = new TextBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox_cam).BeginInit();
            tabControl1.SuspendLayout();
            tabPage_cam.SuspendLayout();
            tabPage_images.SuspendLayout();
            SuspendLayout();
            // 
            // button_camStart
            // 
            button_camStart.Location = new Point(3, 35);
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
            comboBox_cameras.Location = new Point(6, 6);
            comboBox_cameras.Name = "comboBox_cameras";
            comboBox_cameras.Size = new Size(577, 23);
            comboBox_cameras.TabIndex = 1;
            comboBox_cameras.SelectedIndexChanged += ComboBox_cameras_SelectedIndexChanged;
            // 
            // pictureBox_cam
            // 
            pictureBox_cam.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox_cam.Location = new Point(8, 64);
            pictureBox_cam.Name = "pictureBox_cam";
            pictureBox_cam.Size = new Size(752, 282);
            pictureBox_cam.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_cam.TabIndex = 2;
            pictureBox_cam.TabStop = false;
            // 
            // button_refresh
            // 
            button_refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_refresh.Location = new Point(685, 35);
            button_refresh.Name = "button_refresh";
            button_refresh.Size = new Size(75, 23);
            button_refresh.TabIndex = 0;
            button_refresh.Text = "Refresh";
            button_refresh.UseVisualStyleBackColor = true;
            button_refresh.Click += Button_refresh_Click;
            // 
            // button_camGetImage
            // 
            button_camGetImage.Location = new Point(165, 35);
            button_camGetImage.Name = "button_camGetImage";
            button_camGetImage.Size = new Size(75, 23);
            button_camGetImage.TabIndex = 0;
            button_camGetImage.Text = "Get image";
            button_camGetImage.UseVisualStyleBackColor = true;
            button_camGetImage.Click += Button_camGetImage_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage_cam);
            tabControl1.Controls.Add(tabPage_images);
            tabControl1.Controls.Add(tabPage_video);
            tabControl1.Location = new Point(12, 56);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 382);
            tabControl1.TabIndex = 3;
            // 
            // tabPage_cam
            // 
            tabPage_cam.Controls.Add(comboBox_camResolution);
            tabPage_cam.Controls.Add(comboBox_cameras);
            tabPage_cam.Controls.Add(pictureBox_cam);
            tabPage_cam.Controls.Add(button_camGetImage);
            tabPage_cam.Controls.Add(button_refresh);
            tabPage_cam.Controls.Add(button_camStop);
            tabPage_cam.Controls.Add(button_camStart);
            tabPage_cam.Location = new Point(4, 24);
            tabPage_cam.Name = "tabPage_cam";
            tabPage_cam.Padding = new Padding(3);
            tabPage_cam.Size = new Size(768, 354);
            tabPage_cam.TabIndex = 0;
            tabPage_cam.Text = "Camera";
            tabPage_cam.UseVisualStyleBackColor = true;
            // 
            // comboBox_camResolution
            // 
            comboBox_camResolution.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox_camResolution.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_camResolution.FormattingEnabled = true;
            comboBox_camResolution.Location = new Point(589, 6);
            comboBox_camResolution.Name = "comboBox_camResolution";
            comboBox_camResolution.Size = new Size(173, 23);
            comboBox_camResolution.TabIndex = 1;
            // 
            // button_camStop
            // 
            button_camStop.Location = new Point(84, 35);
            button_camStop.Name = "button_camStop";
            button_camStop.Size = new Size(75, 23);
            button_camStop.TabIndex = 0;
            button_camStop.Text = "Stop";
            button_camStop.UseVisualStyleBackColor = true;
            button_camStop.Click += Button_camStop_Click;
            // 
            // tabPage_images
            // 
            tabPage_images.Controls.Add(button3);
            tabPage_images.Controls.Add(button4);
            tabPage_images.Controls.Add(textBox3);
            tabPage_images.Controls.Add(button2);
            tabPage_images.Controls.Add(button1);
            tabPage_images.Controls.Add(textBox2);
            tabPage_images.Controls.Add(label3);
            tabPage_images.Controls.Add(label1);
            tabPage_images.Controls.Add(textBox1);
            tabPage_images.Controls.Add(radioButton2);
            tabPage_images.Controls.Add(radioButton1);
            tabPage_images.Location = new Point(4, 24);
            tabPage_images.Name = "tabPage_images";
            tabPage_images.Padding = new Padding(3);
            tabPage_images.Size = new Size(768, 354);
            tabPage_images.TabIndex = 1;
            tabPage_images.Text = "Images";
            tabPage_images.UseVisualStyleBackColor = true;
            // 
            // tabPage_video
            // 
            tabPage_video.Location = new Point(4, 24);
            tabPage_video.Name = "tabPage_video";
            tabPage_video.Size = new Size(768, 354);
            tabPage_video.TabIndex = 2;
            tabPage_video.Text = "Video";
            tabPage_video.UseVisualStyleBackColor = true;
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
            label_x.Size = new Size(23, 15);
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
            textBox_x.TextChanged += TextBox_x_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(114, 15);
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
            textBox_y.TextChanged += TextBox_y_TextChanged;
            // 
            // label_currentSource
            // 
            label_currentSource.AutoSize = true;
            label_currentSource.Location = new Point(332, 9);
            label_currentSource.Name = "label_currentSource";
            label_currentSource.Size = new Size(90, 15);
            label_currentSource.TabIndex = 6;
            label_currentSource.Text = "Current source";
            // 
            // textBox_currentSource
            // 
            textBox_currentSource.Location = new Point(332, 27);
            textBox_currentSource.MaxLength = 4;
            textBox_currentSource.Name = "textBox_currentSource";
            textBox_currentSource.ReadOnly = true;
            textBox_currentSource.Size = new Size(456, 23);
            textBox_currentSource.TabIndex = 7;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(6, 7);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(60, 19);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Folder";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(6, 36);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(49, 19);
            radioButton2.TabIndex = 0;
            radioButton2.TabStop = true;
            radioButton2.Text = "Files";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(87, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(576, 23);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(669, 6);
            button1.Name = "button1";
            button1.Size = new Size(93, 23);
            button1.TabIndex = 2;
            button1.Text = "Select folder";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Location = new Point(87, 35);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(576, 313);
            textBox2.TabIndex = 1;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(669, 35);
            button2.Name = "button2";
            button2.Size = new Size(93, 23);
            button2.TabIndex = 2;
            button2.Text = "Select files";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(6, 134);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 3;
            button3.Text = "Stop";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(6, 105);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 4;
            button4.Text = "Start";
            button4.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 58);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 6;
            label1.Text = "Delay";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(6, 76);
            textBox3.MaxLength = 4;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(44, 23);
            textBox3.TabIndex = 7;
            textBox3.Text = "3";
            textBox3.TextChanged += TextBox_x_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(56, 79);
            label3.Name = "label3";
            label3.Size = new Size(28, 15);
            label3.TabIndex = 6;
            label3.Text = "sec.";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox_y);
            Controls.Add(textBox_currentSource);
            Controls.Add(textBox_x);
            Controls.Add(label_y);
            Controls.Add(label_currentSource);
            Controls.Add(label2);
            Controls.Add(label_x);
            Controls.Add(button_softCamStop);
            Controls.Add(button_softCamStart);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "VirtualCamProxy";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox_cam).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage_cam.ResumeLayout(false);
            tabPage_images.ResumeLayout(false);
            tabPage_images.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_camStart;
        private ComboBox comboBox_cameras;
        private PictureBox pictureBox_cam;
        private Button button_refresh;
        private Button button_camGetImage;
        private TabControl tabControl1;
        private TabPage tabPage_cam;
        private Button button_camStop;
        private TabPage tabPage_images;
        private TabPage tabPage_video;
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
        private Button button2;
        private Button button1;
        private TextBox textBox2;
        private TextBox textBox1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button3;
        private Button button4;
        private OpenFileDialog openFileDialog1;
        private TextBox textBox3;
        private Label label3;
        private Label label1;
    }
}
