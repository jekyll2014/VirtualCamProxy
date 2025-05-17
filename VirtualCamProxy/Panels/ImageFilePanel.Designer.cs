namespace VirtualCamProxy.Panels
{
    partial class ImageFilePanel
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
            textBox_imageDelay = new TextBox();
            button_selectImageFiles = new Button();
            button_selectImageFolder = new Button();
            textBox_selectedImageFiles = new TextBox();
            label3 = new Label();
            label1 = new Label();
            textBox_selectedImageFolder = new TextBox();
            radioButton_imageFilesChecked = new RadioButton();
            radioButton_imageFolderChecked = new RadioButton();
            openFileDialog1 = new OpenFileDialog();
            folderBrowserDialog1 = new FolderBrowserDialog();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(textBox_imageDelay);
            groupBox1.Controls.Add(button_selectImageFiles);
            groupBox1.Controls.Add(button_selectImageFolder);
            groupBox1.Controls.Add(textBox_selectedImageFiles);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBox_selectedImageFolder);
            groupBox1.Controls.Add(radioButton_imageFilesChecked);
            groupBox1.Controls.Add(radioButton_imageFolderChecked);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(506, 406);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Image file";
            // 
            // textBox_imageDelay
            // 
            textBox_imageDelay.Location = new Point(52, 16);
            textBox_imageDelay.MaxLength = 4;
            textBox_imageDelay.Name = "textBox_imageDelay";
            textBox_imageDelay.Size = new Size(44, 23);
            textBox_imageDelay.TabIndex = 16;
            textBox_imageDelay.Text = "3";
            textBox_imageDelay.TextChanged += TextBox_imageDelay_TextChanged;
            // 
            // button_selectImageFiles
            // 
            button_selectImageFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_selectImageFiles.Location = new Point(407, 73);
            button_selectImageFiles.Name = "button_selectImageFiles";
            button_selectImageFiles.Size = new Size(93, 23);
            button_selectImageFiles.TabIndex = 12;
            button_selectImageFiles.Text = "Select files";
            button_selectImageFiles.UseVisualStyleBackColor = true;
            button_selectImageFiles.Click += Button_selectImageFiles_Click;
            // 
            // button_selectImageFolder
            // 
            button_selectImageFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_selectImageFolder.Location = new Point(407, 44);
            button_selectImageFolder.Name = "button_selectImageFolder";
            button_selectImageFolder.Size = new Size(93, 23);
            button_selectImageFolder.TabIndex = 13;
            button_selectImageFolder.Text = "Select folder";
            button_selectImageFolder.UseVisualStyleBackColor = true;
            button_selectImageFolder.Click += Button_selectImageFolder_Click;
            // 
            // textBox_selectedImageFiles
            // 
            textBox_selectedImageFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox_selectedImageFiles.Location = new Point(85, 73);
            textBox_selectedImageFiles.Multiline = true;
            textBox_selectedImageFiles.Name = "textBox_selectedImageFiles";
            textBox_selectedImageFiles.ReadOnly = true;
            textBox_selectedImageFiles.Size = new Size(316, 327);
            textBox_selectedImageFiles.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(102, 19);
            label3.Name = "label3";
            label3.Size = new Size(28, 15);
            label3.TabIndex = 14;
            label3.Text = "sec.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 15;
            label1.Text = "Delay";
            // 
            // textBox_selectedImageFolder
            // 
            textBox_selectedImageFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_selectedImageFolder.Location = new Point(85, 44);
            textBox_selectedImageFolder.Name = "textBox_selectedImageFolder";
            textBox_selectedImageFolder.ReadOnly = true;
            textBox_selectedImageFolder.Size = new Size(316, 23);
            textBox_selectedImageFolder.TabIndex = 11;
            // 
            // radioButton_imageFilesChecked
            // 
            radioButton_imageFilesChecked.AutoSize = true;
            radioButton_imageFilesChecked.Checked = true;
            radioButton_imageFilesChecked.Location = new Point(4, 74);
            radioButton_imageFilesChecked.Name = "radioButton_imageFilesChecked";
            radioButton_imageFilesChecked.Size = new Size(49, 19);
            radioButton_imageFilesChecked.TabIndex = 8;
            radioButton_imageFilesChecked.TabStop = true;
            radioButton_imageFilesChecked.Text = "Files";
            radioButton_imageFilesChecked.UseVisualStyleBackColor = true;
            radioButton_imageFilesChecked.CheckedChanged += RadioButton_imageFilesChecked_CheckedChanged;
            // 
            // radioButton_imageFolderChecked
            // 
            radioButton_imageFolderChecked.AutoSize = true;
            radioButton_imageFolderChecked.Location = new Point(4, 45);
            radioButton_imageFolderChecked.Name = "radioButton_imageFolderChecked";
            radioButton_imageFolderChecked.Size = new Size(60, 19);
            radioButton_imageFolderChecked.TabIndex = 9;
            radioButton_imageFolderChecked.Text = "Folder";
            radioButton_imageFolderChecked.UseVisualStyleBackColor = true;
            radioButton_imageFolderChecked.CheckedChanged += RadioButton_imageFolderChecked_CheckedChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // ImageFilePanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(groupBox1);
            Name = "ImageFilePanel";
            Size = new Size(506, 406);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox textBox_imageDelay;
        private Button button_selectImageFiles;
        private Button button_selectImageFolder;
        private TextBox textBox_selectedImageFiles;
        private Label label3;
        private Label label1;
        private TextBox textBox_selectedImageFolder;
        private RadioButton radioButton_imageFilesChecked;
        private RadioButton radioButton_imageFolderChecked;
        private OpenFileDialog openFileDialog1;
        private FolderBrowserDialog folderBrowserDialog1;
    }
}
