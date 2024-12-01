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
            tabPage_desktop = new TabPage();
            checkBox_showCursor = new CheckBox();
            tabPage_images = new TabPage();
            textBox3 = new TextBox();
            button2 = new Button();
            button1 = new Button();
            textBox2 = new TextBox();
            label3 = new Label();
            label1 = new Label();
            textBox1 = new TextBox();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            tabPage_video = new TabPage();
            radioButton_videoFilesChecked = new RadioButton();
            radioButton_videoFolderChecked = new RadioButton();
            checkBox_repeatFile = new CheckBox();
            textBox_selectedVideoFolder = new TextBox();
            textBox_selectedVideoFile = new TextBox();
            button_selectVideoFolder = new Button();
            button_selectVideoFile = new Button();
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
            tabPage_filters.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage_desktop.SuspendLayout();
            tabPage_images.SuspendLayout();
            tabPage_video.SuspendLayout();
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
            tabControl1.Controls.Add(tabPage_filters);
            tabControl1.Controls.Add(tabPage_desktop);
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
            tabPage_cam.Controls.Add(checkBox_showStream);
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
            // checkBox_showStream
            // 
            checkBox_showStream.AutoSize = true;
            checkBox_showStream.Location = new Point(246, 38);
            checkBox_showStream.Name = "checkBox_showStream";
            checkBox_showStream.Size = new Size(99, 19);
            checkBox_showStream.TabIndex = 3;
            checkBox_showStream.Text = "Show stream";
            checkBox_showStream.UseVisualStyleBackColor = true;
            checkBox_showStream.CheckedChanged += checkBox_showStream_CheckedChanged;
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
            radioButton_rotateNone.CheckedChanged += radioButton_rotateNone_CheckedChanged;
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
            radioButton_rotate270.CheckedChanged += radioButton_rotate270_CheckedChanged;
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
            radioButton_rotate180.CheckedChanged += radioButton_rotate180_CheckedChanged;
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
            radioButton_rotate90.CheckedChanged += radioButton_rotate90_CheckedChanged;
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
            checkBox_flipVertical.CheckedChanged += checkBox_flipVertical_CheckedChanged;
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
            checkBox_flipHorizontal.CheckedChanged += checkBox_flipHorizontal_CheckedChanged;
            // 
            // tabPage_desktop
            // 
            tabPage_desktop.Controls.Add(checkBox_showCursor);
            tabPage_desktop.Location = new Point(4, 24);
            tabPage_desktop.Name = "tabPage_desktop";
            tabPage_desktop.Size = new Size(768, 354);
            tabPage_desktop.TabIndex = 3;
            tabPage_desktop.Text = "Desktop";
            tabPage_desktop.UseVisualStyleBackColor = true;
            // 
            // checkBox_showCursor
            // 
            checkBox_showCursor.AutoSize = true;
            checkBox_showCursor.Location = new Point(3, 3);
            checkBox_showCursor.Name = "checkBox_showCursor";
            checkBox_showCursor.Size = new Size(95, 19);
            checkBox_showCursor.TabIndex = 4;
            checkBox_showCursor.Text = "Show cursor";
            checkBox_showCursor.UseVisualStyleBackColor = true;
            checkBox_showCursor.CheckedChanged += checkBox_showCursor_CheckedChanged;
            // 
            // tabPage_images
            // 
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
            // textBox3
            // 
            textBox3.Location = new Point(54, 6);
            textBox3.MaxLength = 4;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(44, 23);
            textBox3.TabIndex = 7;
            textBox3.Text = "3";
            textBox3.TextChanged += TextBox_x_TextChanged;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(669, 63);
            button2.Name = "button2";
            button2.Size = new Size(93, 23);
            button2.TabIndex = 2;
            button2.Text = "Select files";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(669, 34);
            button1.Name = "button1";
            button1.Size = new Size(93, 23);
            button1.TabIndex = 2;
            button1.Text = "Select folder";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Location = new Point(87, 63);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(576, 285);
            textBox2.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(104, 9);
            label3.Name = "label3";
            label3.Size = new Size(28, 15);
            label3.TabIndex = 6;
            label3.Text = "sec.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 9);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 6;
            label1.Text = "Delay";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(87, 34);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(576, 23);
            textBox1.TabIndex = 1;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Checked = true;
            radioButton2.Location = new Point(6, 64);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(49, 19);
            radioButton2.TabIndex = 0;
            radioButton2.TabStop = true;
            radioButton2.Text = "Files";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(6, 35);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(60, 19);
            radioButton1.TabIndex = 0;
            radioButton1.Text = "Folder";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // tabPage_video
            // 
            tabPage_video.Controls.Add(radioButton_videoFilesChecked);
            tabPage_video.Controls.Add(radioButton_videoFolderChecked);
            tabPage_video.Controls.Add(checkBox_repeatFile);
            tabPage_video.Controls.Add(textBox_selectedVideoFolder);
            tabPage_video.Controls.Add(textBox_selectedVideoFile);
            tabPage_video.Controls.Add(button_selectVideoFolder);
            tabPage_video.Controls.Add(button_selectVideoFile);
            tabPage_video.Location = new Point(4, 24);
            tabPage_video.Name = "tabPage_video";
            tabPage_video.Size = new Size(768, 354);
            tabPage_video.TabIndex = 2;
            tabPage_video.Text = "Video";
            tabPage_video.UseVisualStyleBackColor = true;
            // 
            // radioButton_videoFilesChecked
            // 
            radioButton_videoFilesChecked.AutoSize = true;
            radioButton_videoFilesChecked.Checked = true;
            radioButton_videoFilesChecked.Location = new Point(3, 58);
            radioButton_videoFilesChecked.Name = "radioButton_videoFilesChecked";
            radioButton_videoFilesChecked.Size = new Size(49, 19);
            radioButton_videoFilesChecked.TabIndex = 10;
            radioButton_videoFilesChecked.TabStop = true;
            radioButton_videoFilesChecked.Text = "Files";
            radioButton_videoFilesChecked.UseVisualStyleBackColor = true;
            radioButton_videoFilesChecked.CheckedChanged += radioButton_videoFilesChecked_CheckedChanged;
            // 
            // radioButton_videoFolderChecked
            // 
            radioButton_videoFolderChecked.AutoSize = true;
            radioButton_videoFolderChecked.Location = new Point(3, 29);
            radioButton_videoFolderChecked.Name = "radioButton_videoFolderChecked";
            radioButton_videoFolderChecked.Size = new Size(60, 19);
            radioButton_videoFolderChecked.TabIndex = 11;
            radioButton_videoFolderChecked.Text = "Folder";
            radioButton_videoFolderChecked.UseVisualStyleBackColor = true;
            radioButton_videoFolderChecked.CheckedChanged += radioButton_videoFolderChecked_CheckedChanged;
            // 
            // checkBox_repeatFile
            // 
            checkBox_repeatFile.AutoSize = true;
            checkBox_repeatFile.Location = new Point(3, 3);
            checkBox_repeatFile.Name = "checkBox_repeatFile";
            checkBox_repeatFile.Size = new Size(88, 19);
            checkBox_repeatFile.TabIndex = 9;
            checkBox_repeatFile.Text = "Repeat File";
            checkBox_repeatFile.UseVisualStyleBackColor = true;
            checkBox_repeatFile.CheckedChanged += checkBox_repeatFile_CheckedChanged;
            // 
            // textBox_selectedVideoFolder
            // 
            textBox_selectedVideoFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_selectedVideoFolder.Location = new Point(69, 28);
            textBox_selectedVideoFolder.MaxLength = 4;
            textBox_selectedVideoFolder.Name = "textBox_selectedVideoFolder";
            textBox_selectedVideoFolder.ReadOnly = true;
            textBox_selectedVideoFolder.Size = new Size(583, 23);
            textBox_selectedVideoFolder.TabIndex = 8;
            // 
            // textBox_selectedVideoFile
            // 
            textBox_selectedVideoFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_selectedVideoFile.Location = new Point(69, 58);
            textBox_selectedVideoFile.MaxLength = 4;
            textBox_selectedVideoFile.Multiline = true;
            textBox_selectedVideoFile.Name = "textBox_selectedVideoFile";
            textBox_selectedVideoFile.ReadOnly = true;
            textBox_selectedVideoFile.Size = new Size(583, 293);
            textBox_selectedVideoFile.TabIndex = 8;
            // 
            // button_selectVideoFolder
            // 
            button_selectVideoFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_selectVideoFolder.Location = new Point(658, 28);
            button_selectVideoFolder.Name = "button_selectVideoFolder";
            button_selectVideoFolder.Size = new Size(107, 23);
            button_selectVideoFolder.TabIndex = 0;
            button_selectVideoFolder.Text = "Select folder ...";
            button_selectVideoFolder.UseVisualStyleBackColor = true;
            button_selectVideoFolder.Click += button_selectVideoFolder_Click;
            // 
            // button_selectVideoFile
            // 
            button_selectVideoFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_selectVideoFile.Location = new Point(658, 57);
            button_selectVideoFile.Name = "button_selectVideoFile";
            button_selectVideoFile.Size = new Size(107, 23);
            button_selectVideoFile.TabIndex = 0;
            button_selectVideoFile.Text = "Select file ...";
            button_selectVideoFile.UseVisualStyleBackColor = true;
            button_selectVideoFile.Click += button_selectVideoFile_Click;
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
            tabPage_cam.PerformLayout();
            tabPage_filters.ResumeLayout(false);
            tabPage_filters.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage_desktop.ResumeLayout(false);
            tabPage_desktop.PerformLayout();
            tabPage_images.ResumeLayout(false);
            tabPage_images.PerformLayout();
            tabPage_video.ResumeLayout(false);
            tabPage_video.PerformLayout();
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
        private OpenFileDialog openFileDialog1;
        private TextBox textBox3;
        private Label label3;
        private Label label1;
        private CheckBox checkBox_showStream;
        private TabPage tabPage_desktop;
        private Button button_selectVideoFile;
        private Button button_selectVideoFolder;
        private CheckBox checkBox_showCursor;
        private TextBox textBox_selectedVideoFile;
        private CheckBox checkBox_repeatFile;
        private TextBox textBox_selectedVideoFolder;
        private RadioButton radioButton_videoFilesChecked;
        private RadioButton radioButton_videoFolderChecked;
        private TabPage tabPage_filters;
        private CheckBox checkBox_flipVertical;
        private CheckBox checkBox_flipHorizontal;
        private GroupBox groupBox1;
        private RadioButton radioButton_rotateNone;
        private RadioButton radioButton_rotate270;
        private RadioButton radioButton_rotate180;
        private RadioButton radioButton_rotate90;
    }
}
