namespace iConnect.UserControls
{
    partial class UC_Conversation
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
            this.UserAvatar = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.UserName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)(this.UserAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // UserAvatar
            // 
            this.UserAvatar.BackColor = System.Drawing.Color.Transparent;
            this.UserAvatar.Image = global::iConnect.Properties.Resources.profile;
            this.UserAvatar.ImageRotate = 0F;
            this.UserAvatar.Location = new System.Drawing.Point(20, 20);
            this.UserAvatar.Name = "UserAvatar";
            this.UserAvatar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.UserAvatar.Size = new System.Drawing.Size(80, 80);
            this.UserAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.UserAvatar.TabIndex = 0;
            this.UserAvatar.TabStop = false;
            this.UserAvatar.UseTransparentBackground = true;
            // 
            // UserName
            // 
            this.UserName.BackColor = System.Drawing.Color.Transparent;
            this.UserName.Font = new System.Drawing.Font("Segoe UI Semibold", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserName.Location = new System.Drawing.Point(120, 40);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(238, 39);
            this.UserName.TabIndex = 1;
            this.UserName.Text = "Trần Thế Hữu Phúc";
            // 
            // UC_Conversation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.UserAvatar);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "UC_Conversation";
            this.Size = new System.Drawing.Size(460, 120);
            ((System.ComponentModel.ISupportInitialize)(this.UserAvatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Guna.UI2.WinForms.Guna2HtmlLabel UserName;
        private Guna.UI2.WinForms.Guna2CirclePictureBox UserAvatar;
    }
}
