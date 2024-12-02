namespace VirtualCamProxy
{
    partial class FiltersPanel
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
            radioButton_rotateNone = new RadioButton();
            radioButton_rotate270 = new RadioButton();
            radioButton_rotate180 = new RadioButton();
            radioButton_rotate90 = new RadioButton();
            checkBox_flipVertical = new CheckBox();
            checkBox_flipHorizontal = new CheckBox();
            groupBox2 = new GroupBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(radioButton_rotateNone);
            groupBox1.Controls.Add(radioButton_rotate270);
            groupBox1.Controls.Add(radioButton_rotate180);
            groupBox1.Controls.Add(radioButton_rotate90);
            groupBox1.Location = new Point(6, 72);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(142, 128);
            groupBox1.TabIndex = 5;
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
            checkBox_flipVertical.Location = new Point(6, 47);
            checkBox_flipVertical.Name = "checkBox_flipVertical";
            checkBox_flipVertical.Size = new Size(90, 19);
            checkBox_flipVertical.TabIndex = 3;
            checkBox_flipVertical.Text = "Flip vertical";
            checkBox_flipVertical.UseVisualStyleBackColor = true;
            checkBox_flipVertical.CheckedChanged += CheckBox_flipVertical_CheckedChanged;
            // 
            // checkBox_flipHorizontal
            // 
            checkBox_flipHorizontal.AutoSize = true;
            checkBox_flipHorizontal.Location = new Point(6, 22);
            checkBox_flipHorizontal.Name = "checkBox_flipHorizontal";
            checkBox_flipHorizontal.Size = new Size(104, 19);
            checkBox_flipHorizontal.TabIndex = 4;
            checkBox_flipHorizontal.Text = "Flip horizontal";
            checkBox_flipHorizontal.UseVisualStyleBackColor = true;
            checkBox_flipHorizontal.CheckedChanged += CheckBox_flipHorizontal_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.AutoSize = true;
            groupBox2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox2.Controls.Add(checkBox_flipHorizontal);
            groupBox2.Controls.Add(groupBox1);
            groupBox2.Controls.Add(checkBox_flipVertical);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(151, 220);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Filters";
            // 
            // FiltersPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(groupBox2);
            Name = "FiltersPanel";
            Size = new Size(151, 220);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private RadioButton radioButton_rotateNone;
        private RadioButton radioButton_rotate270;
        private RadioButton radioButton_rotate180;
        private RadioButton radioButton_rotate90;
        private CheckBox checkBox_flipVertical;
        private CheckBox checkBox_flipHorizontal;
        private GroupBox groupBox2;
    }
}
