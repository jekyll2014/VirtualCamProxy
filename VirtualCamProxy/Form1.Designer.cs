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
            splitContainer1 = new SplitContainer();
            checkBox_showStream = new CheckBox();
            comboBox_camResolution = new ComboBox();
            button_camStop = new Button();
            tabPage_filters = new TabPage();
            groupBox1 = new GroupBox();
            radioButton_rotateNone = new RadioButton();
            radioButton_rotate270 = new RadioButton();
            radioButton_rotate180 = new RadioButton();
            radioButton_rotate90 = new RadioButton();
            checkBox_flipVertical = new CheckBox();
            checkBox_flipHorizontal = new CheckBox();
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
            tabControl1.SuspendLayout();
            tabPage_cam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabPage_filters.SuspendLayout();
            groupBox1.SuspendLayout();
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
            comboBox_cameras.Size = new Size(483, 23);
            comboBox_cameras.TabIndex = 1;
            comboBox_cameras.SelectedIndexChanged += ComboBox_cameras_SelectedIndexChanged;
            // 
            // pictureBox_cam
            // 
            pictureBox_cam.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox_cam.Location = new Point(3, 90);
            pictureBox_cam.Name = "pictureBox_cam";
            pictureBox_cam.Size = new Size(483, 251);
            pictureBox_cam.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_cam.TabIndex = 2;
            pictureBox_cam.TabStop = false;
            // 
            // button_refresh
            // 
            button_refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_refresh.Location = new Point(411, 32);
            button_refresh.Name = "button_refresh";
            button_refresh.Size = new Size(75, 23);
            button_refresh.TabIndex = 0;
            button_refresh.Text = "Refresh";
            button_refresh.UseVisualStyleBackColor = true;
            button_refresh.Click += Button_refresh_Click;
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
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage_cam);
            tabControl1.Controls.Add(tabPage_filters);
            tabControl1.Location = new Point(12, 56);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 382);
            tabControl1.TabIndex = 3;
            // 
            // tabPage_cam
            // 
            tabPage_cam.Controls.Add(splitContainer1);
            tabPage_cam.Location = new Point(4, 24);
            tabPage_cam.Name = "tabPage_cam";
            tabPage_cam.Padding = new Padding(3);
            tabPage_cam.Size = new Size(768, 354);
            tabPage_cam.TabIndex = 0;
            tabPage_cam.Text = "Camera";
            tabPage_cam.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
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
            splitContainer1.Panel1.Controls.Add(pictureBox_cam);
            splitContainer1.Panel1.Controls.Add(button_camGetImage);
            splitContainer1.Size = new Size(762, 348);
            splitContainer1.SplitterDistance = 493;
            splitContainer1.TabIndex = 4;
            // 
            // checkBox_showStream
            // 
            checkBox_showStream.AutoSize = true;
            checkBox_showStream.Location = new Point(246, 64);
            checkBox_showStream.Name = "checkBox_showStream";
            checkBox_showStream.Size = new Size(99, 19);
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
            comboBox_camResolution.Size = new Size(402, 23);
            comboBox_camResolution.TabIndex = 1;
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
            // tabPage_filters
            // 
            tabPage_filters.Controls.Add(groupBox1);
            tabPage_filters.Controls.Add(checkBox_flipVertical);
            tabPage_filters.Controls.Add(checkBox_flipHorizontal);
            tabPage_filters.Location = new Point(4, 24);
            tabPage_filters.Name = "tabPage_filters";
            tabPage_filters.Size = new Size(768, 354);
            tabPage_filters.TabIndex = 4;
            tabPage_filters.Text = "Filters";
            tabPage_filters.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(radioButton_rotateNone);
            groupBox1.Controls.Add(radioButton_rotate270);
            groupBox1.Controls.Add(radioButton_rotate180);
            groupBox1.Controls.Add(radioButton_rotate90);
            groupBox1.Location = new Point(3, 53);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(762, 128);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Rotate";
            // 
            // radioButton_rotateNone
            // 
            radioButton_rotateNone.AutoSize = true;
            radioButton_rotateNone.Checked = true;
            radioButton_rotateNone.Location = new Point(6, 22);
            radioButton_rotateNone.Name = "radioButton_rotateNone";
            radioButton_rotateNone.Size = new Size(55, 19);
            radioButton_rotateNone.TabIndex = 1;
            radioButton_rotateNone.TabStop = true;
            radioButton_rotateNone.Text = "None";
            radioButton_rotateNone.UseVisualStyleBackColor = true;
            radioButton_rotateNone.CheckedChanged += RadioButton_rotateNone_CheckedChanged;
            // 
            // radioButton_rotate270
            // 
            radioButton_rotate270.AutoSize = true;
            radioButton_rotate270.Location = new Point(6, 97);
            radioButton_rotate270.Name = "radioButton_rotate270";
            radioButton_rotate270.Size = new Size(49, 19);
            radioButton_rotate270.TabIndex = 1;
            radioButton_rotate270.Text = "-90°";
            radioButton_rotate270.UseVisualStyleBackColor = true;
            radioButton_rotate270.CheckedChanged += RadioButton_rotate270_CheckedChanged;
            // 
            // radioButton_rotate180
            // 
            radioButton_rotate180.AutoSize = true;
            radioButton_rotate180.Location = new Point(6, 72);
            radioButton_rotate180.Name = "radioButton_rotate180";
            radioButton_rotate180.Size = new Size(51, 19);
            radioButton_rotate180.TabIndex = 1;
            radioButton_rotate180.Text = "180°";
            radioButton_rotate180.UseVisualStyleBackColor = true;
            radioButton_rotate180.CheckedChanged += RadioButton_rotate180_CheckedChanged;
            // 
            // radioButton_rotate90
            // 
            radioButton_rotate90.AutoSize = true;
            radioButton_rotate90.Location = new Point(6, 47);
            radioButton_rotate90.Name = "radioButton_rotate90";
            radioButton_rotate90.Size = new Size(44, 19);
            radioButton_rotate90.TabIndex = 1;
            radioButton_rotate90.Text = "90°";
            radioButton_rotate90.UseVisualStyleBackColor = true;
            radioButton_rotate90.CheckedChanged += RadioButton_rotate90_CheckedChanged;
            // 
            // checkBox_flipVertical
            // 
            checkBox_flipVertical.AutoSize = true;
            checkBox_flipVertical.Location = new Point(3, 28);
            checkBox_flipVertical.Name = "checkBox_flipVertical";
            checkBox_flipVertical.Size = new Size(90, 19);
            checkBox_flipVertical.TabIndex = 0;
            checkBox_flipVertical.Text = "Flip vertical";
            checkBox_flipVertical.UseVisualStyleBackColor = true;
            checkBox_flipVertical.CheckedChanged += CheckBox_flipVertical_CheckedChanged;
            // 
            // checkBox_flipHorizontal
            // 
            checkBox_flipHorizontal.AutoSize = true;
            checkBox_flipHorizontal.Location = new Point(3, 3);
            checkBox_flipHorizontal.Name = "checkBox_flipHorizontal";
            checkBox_flipHorizontal.Size = new Size(104, 19);
            checkBox_flipHorizontal.TabIndex = 0;
            checkBox_flipHorizontal.Text = "Flip horizontal";
            checkBox_flipHorizontal.UseVisualStyleBackColor = true;
            checkBox_flipHorizontal.CheckedChanged += CheckBox_flipHorizontal_CheckedChanged;
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
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
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
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabPage_filters.ResumeLayout(false);
            tabPage_filters.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
        private TabPage tabPage_filters;
        private CheckBox checkBox_flipVertical;
        private CheckBox checkBox_flipHorizontal;
        private GroupBox groupBox1;
        private RadioButton radioButton_rotateNone;
        private RadioButton radioButton_rotate270;
        private RadioButton radioButton_rotate180;
        private RadioButton radioButton_rotate90;
        private SplitContainer splitContainer1;
    }
}
