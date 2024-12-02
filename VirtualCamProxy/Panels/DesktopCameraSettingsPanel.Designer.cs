
namespace VirtualCamProxy
{
    partial class DesktopCameraSettingsPanel
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
            checkBox_showCursor = new CheckBox();
            groupBox1 = new GroupBox();
            checkBox_showClicks = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // checkBox_showCursor
            // 
            checkBox_showCursor.AutoSize = true;
            checkBox_showCursor.Location = new Point(6, 22);
            checkBox_showCursor.Name = "checkBox_showCursor";
            checkBox_showCursor.Size = new Size(95, 19);
            checkBox_showCursor.TabIndex = 5;
            checkBox_showCursor.Text = "Show cursor";
            checkBox_showCursor.UseVisualStyleBackColor = true;
            checkBox_showCursor.CheckedChanged += CheckBox_showCursor_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(checkBox_showClicks);
            groupBox1.Controls.Add(checkBox_showCursor);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(149, 122);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Desktop";
            // 
            // checkBox_showClicks
            // 
            checkBox_showClicks.AutoSize = true;
            checkBox_showClicks.Location = new Point(6, 47);
            checkBox_showClicks.Name = "checkBox_showClicks";
            checkBox_showClicks.Size = new Size(90, 19);
            checkBox_showClicks.TabIndex = 5;
            checkBox_showClicks.Text = "Show clicks";
            checkBox_showClicks.UseVisualStyleBackColor = true;
            checkBox_showClicks.CheckedChanged += checkBox_showClicks_CheckedChanged;
            // 
            // DesktopCameraSettingsPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(groupBox1);
            Name = "DesktopCameraSettingsPanel";
            Size = new Size(149, 122);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox_showCursor;
        private GroupBox groupBox1;
        private CheckBox checkBox_showClicks;
    }
}
