namespace VirtualCamProxy
{
    partial class VideoFilePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            radioButton_videoFilesChecked = new RadioButton();
            radioButton_videoFolderChecked = new RadioButton();
            checkBox_repeatFile = new CheckBox();
            textBox_selectedVideoFolder = new TextBox();
            textBox_selectedVideoFiles = new TextBox();
            button_selectVideoFolder = new Button();
            button_selectVideoFiles = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            openFileDialog1 = new OpenFileDialog();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(radioButton_videoFilesChecked);
            groupBox1.Controls.Add(radioButton_videoFolderChecked);
            groupBox1.Controls.Add(checkBox_repeatFile);
            groupBox1.Controls.Add(textBox_selectedVideoFolder);
            groupBox1.Controls.Add(textBox_selectedVideoFiles);
            groupBox1.Controls.Add(button_selectVideoFolder);
            groupBox1.Controls.Add(button_selectVideoFiles);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(448, 406);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Video file";
            // 
            // radioButton_videoFilesChecked
            // 
            radioButton_videoFilesChecked.AutoSize = true;
            radioButton_videoFilesChecked.Checked = true;
            radioButton_videoFilesChecked.Location = new Point(6, 77);
            radioButton_videoFilesChecked.Name = "radioButton_videoFilesChecked";
            radioButton_videoFilesChecked.Size = new Size(49, 19);
            radioButton_videoFilesChecked.TabIndex = 17;
            radioButton_videoFilesChecked.TabStop = true;
            radioButton_videoFilesChecked.Text = "Files";
            radioButton_videoFilesChecked.UseVisualStyleBackColor = true;
            radioButton_videoFilesChecked.CheckedChanged += RadioButton_videoFilesChecked_CheckedChanged;
            // 
            // radioButton_videoFolderChecked
            // 
            radioButton_videoFolderChecked.AutoSize = true;
            radioButton_videoFolderChecked.Location = new Point(6, 48);
            radioButton_videoFolderChecked.Name = "radioButton_videoFolderChecked";
            radioButton_videoFolderChecked.Size = new Size(60, 19);
            radioButton_videoFolderChecked.TabIndex = 18;
            radioButton_videoFolderChecked.Text = "Folder";
            radioButton_videoFolderChecked.UseVisualStyleBackColor = true;
            radioButton_videoFolderChecked.CheckedChanged += RadioButton_videoFolderChecked_CheckedChanged;
            // 
            // checkBox_repeatFile
            // 
            checkBox_repeatFile.AutoSize = true;
            checkBox_repeatFile.Location = new Point(6, 22);
            checkBox_repeatFile.Name = "checkBox_repeatFile";
            checkBox_repeatFile.Size = new Size(88, 19);
            checkBox_repeatFile.TabIndex = 16;
            checkBox_repeatFile.Text = "Repeat File";
            checkBox_repeatFile.UseVisualStyleBackColor = true;
            checkBox_repeatFile.CheckedChanged += CheckBox_repeatFile_CheckedChanged;
            // 
            // textBox_selectedVideoFolder
            // 
            textBox_selectedVideoFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_selectedVideoFolder.Location = new Point(72, 47);
            textBox_selectedVideoFolder.MaxLength = 4;
            textBox_selectedVideoFolder.Name = "textBox_selectedVideoFolder";
            textBox_selectedVideoFolder.ReadOnly = true;
            textBox_selectedVideoFolder.Size = new Size(257, 23);
            textBox_selectedVideoFolder.TabIndex = 14;
            // 
            // textBox_selectedVideoFiles
            // 
            textBox_selectedVideoFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox_selectedVideoFiles.Location = new Point(72, 77);
            textBox_selectedVideoFiles.MaxLength = 4;
            textBox_selectedVideoFiles.Multiline = true;
            textBox_selectedVideoFiles.Name = "textBox_selectedVideoFiles";
            textBox_selectedVideoFiles.ReadOnly = true;
            textBox_selectedVideoFiles.Size = new Size(257, 323);
            textBox_selectedVideoFiles.TabIndex = 15;
            // 
            // button_selectVideoFolder
            // 
            button_selectVideoFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_selectVideoFolder.Location = new Point(335, 47);
            button_selectVideoFolder.Name = "button_selectVideoFolder";
            button_selectVideoFolder.Size = new Size(107, 23);
            button_selectVideoFolder.TabIndex = 12;
            button_selectVideoFolder.Text = "Select folder ...";
            button_selectVideoFolder.UseVisualStyleBackColor = true;
            button_selectVideoFolder.Click += Button_selectVideoFolder_Click;
            // 
            // button_selectVideoFiles
            // 
            button_selectVideoFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_selectVideoFiles.Location = new Point(335, 76);
            button_selectVideoFiles.Name = "button_selectVideoFiles";
            button_selectVideoFiles.Size = new Size(107, 23);
            button_selectVideoFiles.TabIndex = 13;
            button_selectVideoFiles.Text = "Select file ...";
            button_selectVideoFiles.UseVisualStyleBackColor = true;
            button_selectVideoFiles.Click += Button_selectVideoFiles_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // VideoFilePanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(groupBox1);
            Name = "VideoFilePanel";
            Size = new Size(448, 406);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private FolderBrowserDialog folderBrowserDialog1;
        private OpenFileDialog openFileDialog1;
        private RadioButton radioButton_videoFilesChecked;
        private RadioButton radioButton_videoFolderChecked;
        private CheckBox checkBox_repeatFile;
        private TextBox textBox_selectedVideoFolder;
        private TextBox textBox_selectedVideoFiles;
        private Button button_selectVideoFolder;
        private Button button_selectVideoFiles;
    }
}
