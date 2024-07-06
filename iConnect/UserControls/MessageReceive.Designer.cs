namespace iConnect.UserControls
{
    partial class MessageReceive
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
            this.MessageTime = new System.Windows.Forms.Label();
            this.MessageContent = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // MessageTime
            // 
            this.MessageTime.AutoSize = true;
            this.MessageTime.Location = new System.Drawing.Point(15, 73);
            this.MessageTime.Name = "MessageTime";
            this.MessageTime.Size = new System.Drawing.Size(66, 25);
            this.MessageTime.TabIndex = 3;
            this.MessageTime.Text = "12:30";
            // 
            // MessageContent
            // 
            this.MessageContent.BorderRadius = 30;
            this.MessageContent.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MessageContent.DefaultText = "Example";
            this.MessageContent.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.MessageContent.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.MessageContent.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.MessageContent.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.MessageContent.FillColor = System.Drawing.Color.Gainsboro;
            this.MessageContent.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.MessageContent.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessageContent.ForeColor = System.Drawing.Color.Black;
            this.MessageContent.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.MessageContent.Location = new System.Drawing.Point(20, 0);
            this.MessageContent.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.MessageContent.Name = "MessageContent";
            this.MessageContent.PasswordChar = '\0';
            this.MessageContent.PlaceholderText = "";
            this.MessageContent.ReadOnly = true;
            this.MessageContent.SelectedText = "";
            this.MessageContent.Size = new System.Drawing.Size(500, 70);
            this.MessageContent.TabIndex = 2;
            this.MessageContent.TextOffset = new System.Drawing.Point(20, 0);
            // 
            // MessageReceive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.MessageTime);
            this.Controls.Add(this.MessageContent);
            this.Name = "MessageReceive";
            this.Size = new System.Drawing.Size(970, 110);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MessageTime;
        private Guna.UI2.WinForms.Guna2TextBox MessageContent;
    }
}
