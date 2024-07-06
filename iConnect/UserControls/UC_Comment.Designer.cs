namespace iConnect.UserControls
{
    partial class UC_Comment
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            this.panelWrapperAvatar = new Guna.UI2.WinForms.Guna2Panel();
            this.postAvatarAuthor = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.buttonDeleteComment = new System.Windows.Forms.Button();
            this.btnReply = new Guna.UI2.WinForms.Guna2TileButton();
            this.labelComment = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.labelAuthorName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.labelDotHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.labelCreatedAt = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panelWrapperAvatar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.postAvatarAuthor)).BeginInit();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWrapperAvatar
            // 
            this.panelWrapperAvatar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelWrapperAvatar.BorderRadius = 22;
            this.panelWrapperAvatar.BorderThickness = 4;
            this.panelWrapperAvatar.Controls.Add(this.postAvatarAuthor);
            this.panelWrapperAvatar.CustomizableEdges = customizableEdges3;
            this.panelWrapperAvatar.Enabled = false;
            this.panelWrapperAvatar.Location = new System.Drawing.Point(3, 3);
            this.panelWrapperAvatar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelWrapperAvatar.Name = "panelWrapperAvatar";
            this.panelWrapperAvatar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panelWrapperAvatar.ShadowDecoration.CustomizableEdges = customizableEdges4;
            this.panelWrapperAvatar.Size = new System.Drawing.Size(49, 49);
            this.panelWrapperAvatar.TabIndex = 5;
            // 
            // postAvatarAuthor
            // 
            this.postAvatarAuthor.BackColor = System.Drawing.Color.Transparent;
            this.postAvatarAuthor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.postAvatarAuthor.BorderRadius = 20;
            this.postAvatarAuthor.CustomizableEdges = customizableEdges1;
            this.postAvatarAuthor.ErrorImage = null;
            this.postAvatarAuthor.Image = global::iConnect.Properties.Resources.profile;
            this.postAvatarAuthor.ImageRotate = 0F;
            this.postAvatarAuthor.InitialImage = global::iConnect.Properties.Resources.profile;
            this.postAvatarAuthor.Location = new System.Drawing.Point(1, 1);
            this.postAvatarAuthor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.postAvatarAuthor.Name = "postAvatarAuthor";
            this.postAvatarAuthor.ShadowDecoration.CustomizableEdges = customizableEdges2;
            this.postAvatarAuthor.Size = new System.Drawing.Size(47, 47);
            this.postAvatarAuthor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.postAvatarAuthor.TabIndex = 0;
            this.postAvatarAuthor.TabStop = false;
            this.postAvatarAuthor.WaitOnLoad = true;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.AutoSize = true;
            this.guna2Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.guna2Panel1.Controls.Add(this.buttonDeleteComment);
            this.guna2Panel1.Controls.Add(this.panelWrapperAvatar);
            this.guna2Panel1.Controls.Add(this.btnReply);
            this.guna2Panel1.Controls.Add(this.labelComment);
            this.guna2Panel1.Controls.Add(this.labelAuthorName);
            this.guna2Panel1.Controls.Add(this.labelDotHeader);
            this.guna2Panel1.Controls.Add(this.labelCreatedAt);
            this.guna2Panel1.CustomizableEdges = customizableEdges7;
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges8;
            this.guna2Panel1.Size = new System.Drawing.Size(533, 54);
            this.guna2Panel1.TabIndex = 6;
            // 
            // buttonDeleteComment
            // 
            this.buttonDeleteComment.BackColor = System.Drawing.Color.Transparent;
            this.buttonDeleteComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDeleteComment.ForeColor = System.Drawing.Color.Red;
            this.buttonDeleteComment.Location = new System.Drawing.Point(435, 8);
            this.buttonDeleteComment.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDeleteComment.Name = "buttonDeleteComment";
            this.buttonDeleteComment.Size = new System.Drawing.Size(45, 23);
            this.buttonDeleteComment.TabIndex = 7;
            this.buttonDeleteComment.Text = "Xóa";
            this.buttonDeleteComment.UseVisualStyleBackColor = false;
            this.buttonDeleteComment.Visible = false;
            // 
            // btnReply
            // 
            this.btnReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReply.CustomizableEdges = customizableEdges5;
            this.btnReply.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReply.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReply.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReply.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReply.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnReply.ForeColor = System.Drawing.Color.White;
            this.btnReply.Location = new System.Drawing.Point(349, 6);
            this.btnReply.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReply.MaximumSize = new System.Drawing.Size(107, 48);
            this.btnReply.Name = "btnReply";
            this.btnReply.ShadowDecoration.CustomizableEdges = customizableEdges6;
            this.btnReply.Size = new System.Drawing.Size(76, 26);
            this.btnReply.TabIndex = 6;
            this.btnReply.Text = "Trả lời";
            this.btnReply.Click += new System.EventHandler(this.btnReply_Click);
            // 
            // labelComment
            // 
            this.labelComment.BackColor = System.Drawing.Color.Transparent;
            this.labelComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComment.Location = new System.Drawing.Point(55, 28);
            this.labelComment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelComment.MaximumSize = new System.Drawing.Size(356, 0);
            this.labelComment.MinimumSize = new System.Drawing.Size(17, 0);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(92, 18);
            this.labelComment.TabIndex = 5;
            this.labelComment.Text = "coment của tôi!";
            this.labelComment.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAuthorName
            // 
            this.labelAuthorName.BackColor = System.Drawing.Color.Transparent;
            this.labelAuthorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAuthorName.Location = new System.Drawing.Point(55, 6);
            this.labelAuthorName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelAuthorName.MinimumSize = new System.Drawing.Size(17, 0);
            this.labelAuthorName.Name = "labelAuthorName";
            this.labelAuthorName.Size = new System.Drawing.Size(74, 19);
            this.labelAuthorName.TabIndex = 1;
            this.labelAuthorName.Text = "username";
            this.labelAuthorName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDotHeader
            // 
            this.labelDotHeader.BackColor = System.Drawing.Color.Transparent;
            this.labelDotHeader.Location = new System.Drawing.Point(116, 7);
            this.labelDotHeader.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelDotHeader.Name = "labelDotHeader";
            this.labelDotHeader.Size = new System.Drawing.Size(10, 18);
            this.labelDotHeader.TabIndex = 2;
            this.labelDotHeader.Text = "•";
            // 
            // labelCreatedAt
            // 
            this.labelCreatedAt.BackColor = System.Drawing.Color.Transparent;
            this.labelCreatedAt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreatedAt.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelCreatedAt.Location = new System.Drawing.Point(201, 6);
            this.labelCreatedAt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelCreatedAt.MaximumSize = new System.Drawing.Size(223, 0);
            this.labelCreatedAt.Name = "labelCreatedAt";
            this.labelCreatedAt.Size = new System.Drawing.Size(33, 22);
            this.labelCreatedAt.TabIndex = 3;
            this.labelCreatedAt.Text = "time";
            // 
            // UC_Comment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.guna2Panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximumSize = new System.Drawing.Size(533, 0);
            this.MinimumSize = new System.Drawing.Size(0, 25);
            this.Name = "UC_Comment";
            this.Size = new System.Drawing.Size(533, 54);
            this.panelWrapperAvatar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.postAvatarAuthor)).EndInit();
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel panelWrapperAvatar;
        private Guna.UI2.WinForms.Guna2PictureBox postAvatarAuthor;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelAuthorName;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelDotHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelCreatedAt;
        private Guna.UI2.WinForms.Guna2HtmlLabel labelComment;
        private Guna.UI2.WinForms.Guna2TileButton btnReply;
        private System.Windows.Forms.Button buttonDeleteComment;
    }
}
