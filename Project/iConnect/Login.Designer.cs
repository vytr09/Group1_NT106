namespace iConnect
{
    partial class Login
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


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.loginBtn = new Guna.UI2.WinForms.Guna2Button();
            this.passwdTxt = new Guna.UI2.WinForms.Guna2TextBox();
            this.usernameTxt = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2CircleButton1 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.minimizeBtn = new Guna.UI2.WinForms.Guna2CircleButton();
            this.closeAppBtn = new Guna.UI2.WinForms.Guna2CircleButton();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.usernameLbl = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.passwdLbl = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.hidePasswd = new Guna.UI2.WinForms.Guna2GradientButton();
            this.showPasswd = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2PictureBox2 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.forgetPasswdBtn = new Guna.UI2.WinForms.Guna2Button();
            this.forgetPasswdPnl = new Guna.UI2.WinForms.Guna2Panel();
            this.forgetEmailTxt = new Guna.UI2.WinForms.Guna2TextBox();
            this.forgetPasswdCodeBtn = new Guna.UI2.WinForms.Guna2Button();
            this.forgetPasswdCodeLbl = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox2)).BeginInit();
            this.forgetPasswdPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(251, 498);
            this.guna2HtmlLabel1.Margin = new System.Windows.Forms.Padding(2);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(185, 27);
            this.guna2HtmlLabel1.TabIndex = 12;
            this.guna2HtmlLabel1.Text = "Bạn chưa có tài khoản?";
            // 
            // loginBtn
            // 
            this.loginBtn.AnimatedGIF = true;
            this.loginBtn.BorderRadius = 30;
            this.loginBtn.CustomImages.CheckedImage = global::iConnect.Properties.Resources.loading;
            this.loginBtn.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.loginBtn.CustomImages.ImageSize = new System.Drawing.Size(30, 30);
            this.loginBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.loginBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.loginBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.loginBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.loginBtn.FillColor = System.Drawing.Color.MediumAquamarine;
            this.loginBtn.Font = new System.Drawing.Font("Segoe UI", 13.875F, System.Drawing.FontStyle.Bold);
            this.loginBtn.ForeColor = System.Drawing.Color.White;
            this.loginBtn.Location = new System.Drawing.Point(308, 410);
            this.loginBtn.Margin = new System.Windows.Forms.Padding(2);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.PressedDepth = 0;
            this.loginBtn.Size = new System.Drawing.Size(183, 57);
            this.loginBtn.TabIndex = 11;
            this.loginBtn.Text = "Đăng nhập";
            this.loginBtn.Click += new System.EventHandler(this.loginBtn_Click);
            // 
            // passwdTxt
            // 
            this.passwdTxt.BorderColor = System.Drawing.Color.Black;
            this.passwdTxt.BorderRadius = 15;
            this.passwdTxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.passwdTxt.DefaultText = "";
            this.passwdTxt.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.passwdTxt.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.passwdTxt.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.passwdTxt.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.passwdTxt.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.passwdTxt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwdTxt.ForeColor = System.Drawing.Color.Black;
            this.passwdTxt.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.passwdTxt.Location = new System.Drawing.Point(211, 318);
            this.passwdTxt.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.passwdTxt.Name = "passwdTxt";
            this.passwdTxt.PasswordChar = '●';
            this.passwdTxt.PlaceholderText = "Mật khẩu";
            this.passwdTxt.SelectedText = "";
            this.passwdTxt.Size = new System.Drawing.Size(373, 57);
            this.passwdTxt.TabIndex = 10;
            this.passwdTxt.TextOffset = new System.Drawing.Point(20, 0);
            this.passwdTxt.UseSystemPasswordChar = true;
            this.passwdTxt.TextChanged += new System.EventHandler(this.passwdTxt_TextChanged);
            // 
            // usernameTxt
            // 
            this.usernameTxt.BorderColor = System.Drawing.Color.Black;
            this.usernameTxt.BorderRadius = 15;
            this.usernameTxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.usernameTxt.DefaultText = "";
            this.usernameTxt.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.usernameTxt.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.usernameTxt.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.usernameTxt.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.usernameTxt.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.usernameTxt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTxt.ForeColor = System.Drawing.Color.Black;
            this.usernameTxt.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.usernameTxt.Location = new System.Drawing.Point(211, 231);
            this.usernameTxt.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.usernameTxt.Name = "usernameTxt";
            this.usernameTxt.PasswordChar = '\0';
            this.usernameTxt.PlaceholderText = "Tên đăng nhập";
            this.usernameTxt.SelectedText = "";
            this.usernameTxt.Size = new System.Drawing.Size(373, 57);
            this.usernameTxt.TabIndex = 9;
            this.usernameTxt.TextOffset = new System.Drawing.Point(20, 0);
            this.usernameTxt.TextChanged += new System.EventHandler(this.usernameTxt_TextChanged);
            // 
            // guna2CircleButton1
            // 
            this.guna2CircleButton1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2CircleButton1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2CircleButton1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2CircleButton1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2CircleButton1.FillColor = System.Drawing.Color.LimeGreen;
            this.guna2CircleButton1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2CircleButton1.ForeColor = System.Drawing.Color.White;
            this.guna2CircleButton1.Location = new System.Drawing.Point(57, 10);
            this.guna2CircleButton1.Margin = new System.Windows.Forms.Padding(2);
            this.guna2CircleButton1.Name = "guna2CircleButton1";
            this.guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton1.Size = new System.Drawing.Size(17, 16);
            this.guna2CircleButton1.TabIndex = 57;
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.minimizeBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.minimizeBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.minimizeBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.minimizeBtn.FillColor = System.Drawing.Color.Orange;
            this.minimizeBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.minimizeBtn.ForeColor = System.Drawing.Color.White;
            this.minimizeBtn.Location = new System.Drawing.Point(33, 10);
            this.minimizeBtn.Margin = new System.Windows.Forms.Padding(2);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.minimizeBtn.Size = new System.Drawing.Size(17, 16);
            this.minimizeBtn.TabIndex = 56;
            this.minimizeBtn.Click += new System.EventHandler(this.minimizeBtn_Click);
            // 
            // closeAppBtn
            // 
            this.closeAppBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.closeAppBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.closeAppBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.closeAppBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.closeAppBtn.FillColor = System.Drawing.Color.OrangeRed;
            this.closeAppBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.closeAppBtn.ForeColor = System.Drawing.Color.White;
            this.closeAppBtn.Location = new System.Drawing.Point(10, 10);
            this.closeAppBtn.Margin = new System.Windows.Forms.Padding(2);
            this.closeAppBtn.Name = "closeAppBtn";
            this.closeAppBtn.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.closeAppBtn.Size = new System.Drawing.Size(17, 16);
            this.closeAppBtn.TabIndex = 55;
            this.closeAppBtn.Click += new System.EventHandler(this.closeAppBtn_Click);
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 25;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2Button1
            // 
            this.guna2Button1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button1.BorderRadius = 15;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.FillColor = System.Drawing.Color.Transparent;
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.guna2Button1.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.guna2Button1.HoverState.FillColor = System.Drawing.Color.White;
            this.guna2Button1.HoverState.Font = new System.Drawing.Font("Segoe UI Black", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.guna2Button1.HoverState.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.guna2Button1.Location = new System.Drawing.Point(437, 495);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.PressedDepth = 0;
            this.guna2Button1.Size = new System.Drawing.Size(127, 34);
            this.guna2Button1.TabIndex = 58;
            this.guna2Button1.Text = "Đăng ký";
            this.guna2Button1.UseTransparentBackground = true;
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // usernameLbl
            // 
            this.usernameLbl.BackColor = System.Drawing.Color.Transparent;
            this.usernameLbl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.usernameLbl.ForeColor = System.Drawing.Color.Red;
            this.usernameLbl.Location = new System.Drawing.Point(215, 292);
            this.usernameLbl.Name = "usernameLbl";
            this.usernameLbl.Size = new System.Drawing.Size(3, 2);
            this.usernameLbl.TabIndex = 59;
            this.usernameLbl.Text = " ";
            this.usernameLbl.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // passwdLbl
            // 
            this.passwdLbl.BackColor = System.Drawing.Color.Transparent;
            this.passwdLbl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.passwdLbl.ForeColor = System.Drawing.Color.Red;
            this.passwdLbl.Location = new System.Drawing.Point(215, 383);
            this.passwdLbl.Name = "passwdLbl";
            this.passwdLbl.Size = new System.Drawing.Size(3, 2);
            this.passwdLbl.TabIndex = 60;
            this.passwdLbl.Text = " ";
            this.passwdLbl.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hidePasswd
            // 
            this.hidePasswd.BackColor = System.Drawing.Color.Transparent;
            this.hidePasswd.Cursor = System.Windows.Forms.Cursors.Default;
            this.hidePasswd.CustomImages.Image = global::iConnect.Properties.Resources.hide;
            this.hidePasswd.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.hidePasswd.CustomImages.ImageOffset = new System.Drawing.Point(0, -5);
            this.hidePasswd.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.hidePasswd.DisabledState.BorderColor = System.Drawing.Color.Black;
            this.hidePasswd.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.hidePasswd.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.hidePasswd.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.hidePasswd.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.hidePasswd.FillColor = System.Drawing.Color.Transparent;
            this.hidePasswd.FillColor2 = System.Drawing.Color.Transparent;
            this.hidePasswd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hidePasswd.ForeColor = System.Drawing.Color.White;
            this.hidePasswd.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.hidePasswd.HoverState.FillColor2 = System.Drawing.Color.Transparent;
            this.hidePasswd.Location = new System.Drawing.Point(535, 340);
            this.hidePasswd.Margin = new System.Windows.Forms.Padding(2);
            this.hidePasswd.Name = "hidePasswd";
            this.hidePasswd.PressedColor = System.Drawing.Color.Transparent;
            this.hidePasswd.PressedDepth = 0;
            this.hidePasswd.Size = new System.Drawing.Size(29, 23);
            this.hidePasswd.TabIndex = 62;
            this.hidePasswd.UseTransparentBackground = true;
            this.hidePasswd.Visible = false;
            this.hidePasswd.Click += new System.EventHandler(this.hidePasswd_Click);
            // 
            // showPasswd
            // 
            this.showPasswd.BackColor = System.Drawing.Color.Transparent;
            this.showPasswd.Cursor = System.Windows.Forms.Cursors.Default;
            this.showPasswd.CustomImages.Image = global::iConnect.Properties.Resources.eye;
            this.showPasswd.CustomImages.ImageAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.showPasswd.CustomImages.ImageOffset = new System.Drawing.Point(0, -5);
            this.showPasswd.CustomImages.ImageSize = new System.Drawing.Size(25, 25);
            this.showPasswd.DisabledState.BorderColor = System.Drawing.Color.Black;
            this.showPasswd.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.showPasswd.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.showPasswd.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.showPasswd.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.showPasswd.FillColor = System.Drawing.Color.Transparent;
            this.showPasswd.FillColor2 = System.Drawing.Color.Transparent;
            this.showPasswd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.showPasswd.ForeColor = System.Drawing.Color.White;
            this.showPasswd.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.showPasswd.HoverState.FillColor2 = System.Drawing.Color.Transparent;
            this.showPasswd.Location = new System.Drawing.Point(535, 340);
            this.showPasswd.Margin = new System.Windows.Forms.Padding(2);
            this.showPasswd.Name = "showPasswd";
            this.showPasswd.PressedColor = System.Drawing.Color.Transparent;
            this.showPasswd.PressedDepth = 0;
            this.showPasswd.Size = new System.Drawing.Size(29, 23);
            this.showPasswd.TabIndex = 61;
            this.showPasswd.UseTransparentBackground = true;
            this.showPasswd.Click += new System.EventHandler(this.showPasswd_Click);
            // 
            // guna2PictureBox2
            // 
            this.guna2PictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.guna2PictureBox2.Image = global::iConnect.Properties.Resources.Screenshot_2024_04_20_232604;
            this.guna2PictureBox2.ImageRotate = 0F;
            this.guna2PictureBox2.Location = new System.Drawing.Point(206, 94);
            this.guna2PictureBox2.Margin = new System.Windows.Forms.Padding(1);
            this.guna2PictureBox2.Name = "guna2PictureBox2";
            this.guna2PictureBox2.Size = new System.Drawing.Size(388, 120);
            this.guna2PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2PictureBox2.TabIndex = 63;
            this.guna2PictureBox2.TabStop = false;
            this.guna2PictureBox2.UseTransparentBackground = true;
            // 
            // forgetPasswdBtn
            // 
            this.forgetPasswdBtn.BackColor = System.Drawing.Color.Transparent;
            this.forgetPasswdBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.forgetPasswdBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.forgetPasswdBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.forgetPasswdBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.forgetPasswdBtn.FillColor = System.Drawing.Color.Transparent;
            this.forgetPasswdBtn.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.forgetPasswdBtn.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.forgetPasswdBtn.HoverState.FillColor = System.Drawing.Color.White;
            this.forgetPasswdBtn.HoverState.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.forgetPasswdBtn.Location = new System.Drawing.Point(311, 519);
            this.forgetPasswdBtn.Name = "forgetPasswdBtn";
            this.forgetPasswdBtn.PressedDepth = 0;
            this.forgetPasswdBtn.Size = new System.Drawing.Size(180, 45);
            this.forgetPasswdBtn.TabIndex = 64;
            this.forgetPasswdBtn.Text = "Quên mật khẩu?";
            this.forgetPasswdBtn.UseTransparentBackground = true;
            this.forgetPasswdBtn.Click += new System.EventHandler(this.forgetPasswdBtn_Click);
            // 
            // forgetPasswdPnl
            // 
            this.forgetPasswdPnl.Controls.Add(this.forgetPasswdCodeLbl);
            this.forgetPasswdPnl.Controls.Add(this.forgetPasswdCodeBtn);
            this.forgetPasswdPnl.Controls.Add(this.forgetEmailTxt);
            this.forgetPasswdPnl.Location = new System.Drawing.Point(190, 211);
            this.forgetPasswdPnl.Name = "forgetPasswdPnl";
            this.forgetPasswdPnl.Size = new System.Drawing.Size(419, 353);
            this.forgetPasswdPnl.TabIndex = 65;
            this.forgetPasswdPnl.Visible = false;
            // 
            // forgetEmailTxt
            // 
            this.forgetEmailTxt.BorderRadius = 25;
            this.forgetEmailTxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.forgetEmailTxt.DefaultText = "";
            this.forgetEmailTxt.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.forgetEmailTxt.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.forgetEmailTxt.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.forgetEmailTxt.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.forgetEmailTxt.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.forgetEmailTxt.Font = new System.Drawing.Font("Segoe UI", 10.8F);
            this.forgetEmailTxt.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.forgetEmailTxt.Location = new System.Drawing.Point(25, 51);
            this.forgetEmailTxt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.forgetEmailTxt.Name = "forgetEmailTxt";
            this.forgetEmailTxt.PasswordChar = '\0';
            this.forgetEmailTxt.PlaceholderText = "Email đăng ký";
            this.forgetEmailTxt.SelectedText = "";
            this.forgetEmailTxt.Size = new System.Drawing.Size(365, 60);
            this.forgetEmailTxt.TabIndex = 66;
            this.forgetEmailTxt.TextChanged += new System.EventHandler(this.forgetEmailTxt_TextChanged);
            // 
            // forgetPasswdCodeBtn
            // 
            this.forgetPasswdCodeBtn.BorderRadius = 20;
            this.forgetPasswdCodeBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.forgetPasswdCodeBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.forgetPasswdCodeBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.forgetPasswdCodeBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.forgetPasswdCodeBtn.FillColor = System.Drawing.Color.MediumAquamarine;
            this.forgetPasswdCodeBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.forgetPasswdCodeBtn.ForeColor = System.Drawing.Color.White;
            this.forgetPasswdCodeBtn.Location = new System.Drawing.Point(118, 129);
            this.forgetPasswdCodeBtn.Name = "forgetPasswdCodeBtn";
            this.forgetPasswdCodeBtn.Size = new System.Drawing.Size(180, 45);
            this.forgetPasswdCodeBtn.TabIndex = 66;
            this.forgetPasswdCodeBtn.Text = "Gửi mã";
            // 
            // forgetPasswdCodeLbl
            // 
            this.forgetPasswdCodeLbl.BackColor = System.Drawing.Color.Transparent;
            this.forgetPasswdCodeLbl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.forgetPasswdCodeLbl.ForeColor = System.Drawing.Color.Crimson;
            this.forgetPasswdCodeLbl.Location = new System.Drawing.Point(32, 119);
            this.forgetPasswdCodeLbl.Name = "forgetPasswdCodeLbl";
            this.forgetPasswdCodeLbl.Size = new System.Drawing.Size(3, 2);
            this.forgetPasswdCodeLbl.TabIndex = 67;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 576);
            this.Controls.Add(this.forgetPasswdPnl);
            this.Controls.Add(this.guna2PictureBox2);
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.guna2HtmlLabel1);
            this.Controls.Add(this.passwdLbl);
            this.Controls.Add(this.usernameLbl);
            this.Controls.Add(this.guna2CircleButton1);
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.closeAppBtn);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.usernameTxt);
            this.Controls.Add(this.showPasswd);
            this.Controls.Add(this.hidePasswd);
            this.Controls.Add(this.passwdTxt);
            this.Controls.Add(this.forgetPasswdBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox2)).EndInit();
            this.forgetPasswdPnl.ResumeLayout(false);
            this.forgetPasswdPnl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Button loginBtn;
        private Guna.UI2.WinForms.Guna2TextBox passwdTxt;
        private Guna.UI2.WinForms.Guna2TextBox usernameTxt;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton1;
        private Guna.UI2.WinForms.Guna2CircleButton minimizeBtn;
        private Guna.UI2.WinForms.Guna2CircleButton closeAppBtn;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2HtmlLabel usernameLbl;
        private Guna.UI2.WinForms.Guna2HtmlLabel passwdLbl;
        private Guna.UI2.WinForms.Guna2GradientButton hidePasswd;
        private Guna.UI2.WinForms.Guna2GradientButton showPasswd;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox2;
        private Guna.UI2.WinForms.Guna2Button forgetPasswdBtn;
        private Guna.UI2.WinForms.Guna2Panel forgetPasswdPnl;
        private Guna.UI2.WinForms.Guna2TextBox forgetEmailTxt;
        private Guna.UI2.WinForms.Guna2Button forgetPasswdCodeBtn;
        private Guna.UI2.WinForms.Guna2HtmlLabel forgetPasswdCodeLbl;
    }
}
