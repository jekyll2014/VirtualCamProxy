namespace VirtualCamProxy.Panels
{
    partial class MjpegCameraPropertyPanel
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
            textBox_password = new TextBox();
            textBox_login = new TextBox();
            label5 = new Label();
            textBox_auth = new TextBox();
            label4 = new Label();
            textBox_name = new TextBox();
            label3 = new Label();
            textBox_path = new TextBox();
            label2 = new Label();
            label1 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(textBox_password);
            groupBox1.Controls.Add(textBox_login);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(textBox_auth);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(textBox_name);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(textBox_path);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(404, 409);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "MJPEG Camera properties";
            // 
            // textBox_password
            // 
            textBox_password.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_password.Location = new Point(66, 132);
            textBox_password.Name = "textBox_password";
            textBox_password.ReadOnly = true;
            textBox_password.Size = new Size(332, 23);
            textBox_password.TabIndex = 4;
            // 
            // textBox_login
            // 
            textBox_login.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_login.Location = new Point(66, 103);
            textBox_login.Name = "textBox_login";
            textBox_login.ReadOnly = true;
            textBox_login.Size = new Size(332, 23);
            textBox_login.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 135);
            label5.Name = "label5";
            label5.Size = new Size(57, 15);
            label5.TabIndex = 2;
            label5.Text = "Password";
            // 
            // textBox_auth
            // 
            textBox_auth.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_auth.Location = new Point(66, 74);
            textBox_auth.Name = "textBox_auth";
            textBox_auth.ReadOnly = true;
            textBox_auth.Size = new Size(332, 23);
            textBox_auth.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 106);
            label4.Name = "label4";
            label4.Size = new Size(37, 15);
            label4.TabIndex = 2;
            label4.Text = "Login";
            // 
            // textBox_name
            // 
            textBox_name.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_name.Location = new Point(66, 45);
            textBox_name.Name = "textBox_name";
            textBox_name.ReadOnly = true;
            textBox_name.Size = new Size(332, 23);
            textBox_name.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 77);
            label3.Name = "label3";
            label3.Size = new Size(33, 15);
            label3.TabIndex = 2;
            label3.Text = "Auth";
            // 
            // textBox_path
            // 
            textBox_path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox_path.Location = new Point(66, 16);
            textBox_path.Name = "textBox_path";
            textBox_path.ReadOnly = true;
            textBox_path.Size = new Size(332, 23);
            textBox_path.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 48);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 2;
            label2.Text = "Name";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 3;
            label1.Text = "Path";
            // 
            // MjpegCameraPropertyPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(groupBox1);
            Name = "MjpegCameraPropertyPanel";
            Size = new Size(404, 409);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox textBox_name;
        private TextBox textBox_path;
        private Label label2;
        private Label label1;
        private TextBox textBox_auth;
        private Label label3;
        private TextBox textBox_password;
        private TextBox textBox_login;
        private Label label5;
        private Label label4;
    }
}
