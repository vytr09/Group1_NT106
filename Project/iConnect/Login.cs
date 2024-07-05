using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Guna.UI2.WinForms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;

namespace iConnect
{
    public partial class Login : Form
    {
        private string confirmationCode;
        bool ispasswdValid;
        bool isrepasswdValid;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;

        public Login()
        {
            InitializeComponent();
            loginBtn.Enabled = false;
            Data datalayer = new Data()
            {
                username = usernameTxt.Text,
                password = passwdTxt.Text
            };
            passwdTxt.KeyDown += passwordTxt_KeyDown;
            showPasswd.Click += showPasswd_Click; // Assign event handler for showPasswd button
            hidePasswd.Click += hidePasswd_Click; // Assign event handler for hidePasswd button
        }

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                if (client != null)
                {
                    //MessageBox.Show("Connection Success");
                }
            }
            catch
            {
                MessageBox.Show("Connection Failed");
            }
        }
        // Function to hash the password using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        private void loginBtn_Click(object sender, EventArgs e)
        {
            loginBtn.Checked = true;
            loginBtn.Text = "";
            // Hash the password using SHA256
            string hashedPassword = HashPassword(passwdTxt.Text);
            FirebaseResponse response = client.Get("Users/" + usernameTxt.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                Data result = response.ResultAs<Data>();

                if (usernameTxt.Text.Equals(result.username) && hashedPassword.Equals(result.password))
                {
                    this.Hide();
                    Home home = new Home(usernameTxt.Text); ;
                    home.Closed += (s, args) => this.Close();
                    home.Show();
                }
                else
                {
                    passwdLbl.Text = "Tài khoản hoặc mật khẩu không chính xác.";
                    loginBtn.Checked = false;
                    loginBtn.Text = "Đăng nhập";
                }
            }
            else
            {
                passwdLbl.Text = "Tài khoản hoặc mật khẩu không chính xác.";
                loginBtn.Checked = false;
                loginBtn.Text = "Đăng nhập";
            }
        }

        class Data
        {
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }

        }


        private void closeAppBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Signup signup = new Signup();
            signup.Closed += (s, args) => this.Show(); // Reopen login form when signup form is closed
            signup.Show();
        }

        private void usernameTxt_TextChanged(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(usernameTxt.Text))
            {
                usernameLbl.Text = "Tên tài khoản không được để trống";
                loginBtn.Enabled = false;
            }
            else
            {
                usernameLbl.Text = "";
                loginBtn.Enabled = !string.IsNullOrEmpty(passwdTxt.Text);
            }
        }

        private void passwdTxt_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(passwdTxt.Text))
            {
                passwdLbl.Text = "Mật khẩu không được để trống";
                loginBtn.Enabled = false;
            }
            else if (passwdTxt.Text.Length < 8)
            {
                passwdLbl.Text = "Mật khẩu phải từ 8 ký tự";
                loginBtn.Enabled = false;
            }
            else
            {
                passwdLbl.Text = "";
                loginBtn.Enabled = !string.IsNullOrEmpty(usernameTxt.Text);
            }
        }

        private void showPasswd_Click(object sender, EventArgs e)
        {
            passwdTxt.Focus();
            passwdTxt.UseSystemPasswordChar = false;
            passwdTxt.PasswordChar = default(char);
            showPasswd.Hide();
            hidePasswd.Show();
        }

        private void passwordTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loginBtn.PerformClick();
            }
        }

        private void hidePasswd_Click(object sender, EventArgs e)
        {
            passwdTxt.Focus();
            passwdTxt.UseSystemPasswordChar = true;
            hidePasswd.Hide();
            showPasswd.Show();
        }

        private void forgetPasswdBtn_Click(object sender, EventArgs e)
        {
            forgetPasswdPnl.Visible = true;
        }

        private void forgetEmailTxt_TextChanged(object sender, EventArgs e)
        {
            Data user = GetUserByEmail(forgetEmailTxt.Text);
            if (user != null)
            {
                forgetPasswdCodeLbl.Text = "";
                forgetPasswdCodeBtn.Enabled = true;
            }
            else
            {
                forgetPasswdCodeLbl.Text = "Email không tồn tại";
                forgetPasswdCodeBtn.Enabled = false;
            }
        }

        private Data GetUserByEmail(string email)
        {
            FirebaseResponse response = client.Get("Users");
            if (response != null && response.Body != "null")
            {
                Dictionary<string, Data> dataDict = response.ResultAs<Dictionary<string, Data>>();
                return dataDict.Values.FirstOrDefault(data => data.email == email);
            }
            return null;
        }


        private string GenerateConfirmationCode()
        {
            // Generate a random confirmation code
            // Example: you can use Guid.NewGuid().ToString() for a unique identifier
            return Guid.NewGuid().ToString().Substring(0, 8); // Generate an 8-character code
        }

        private void SendConfirmationEmail(string email, string confirmationCode)
        {
            // Email configuration
            string senderEmail = "kaitsukishi@gmail.com"; // Your email address
            string senderPassword = "psqhniyosuaxspqm"; // Your email password
            string smtpHost = "smtp.gmail.com"; // Your SMTP host
            int smtpPort = 587; // Your SMTP port (e.g., 587 for Gmail)

            // Email content
            string subject = "iConnect - Quên mật khẩu";
            string body = $"Chào bạn,<br><br>" +
                            $"Bạn đã yêu cầu thay đổi mật khẩu tài khoản của mình.<br><br>" +
                            $"Đây là mã xác nhận của bạn: <b>{confirmationCode}</b>.<br>" +
                            $"Vui lòng sử dụng mã này để hoàn tất quá trình thay đổi mật khẩu.<br><br>" +
                            $"Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.<br><br>" +
                            $"Trân trọng,<br>Đội ngũ quản trị viên";

            // Create and configure the SMTP client
            SmtpClient client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true // Enable SSL for secure connection (e.g., for Gmail)
            };

            // Create the email message
            MailMessage mailMessage = new MailMessage(senderEmail, email, subject, body)
            {
                IsBodyHtml = true // Set to true if your email body contains HTML
            };

            // Send the email
            client.Send(mailMessage);
        }

        private void forgetPasswdCodeBtn_Click(object sender, EventArgs e)
        {
            string email = forgetEmailTxt.Text;
            forgetPasswdPnl.Visible = false;
            codeEnterPnl.Visible = true;
            confirmationCode = GenerateConfirmationCode();

            try
            {
                SendConfirmationEmail(email, confirmationCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send confirmation email. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            // Assuming you have textbox for code input named "codeTextBox"
            string enteredCode = codeTxt.Text;

            if (enteredCode == confirmationCode)
            {
                codeEnterPnl.Visible = false;
                resetPasswdPnl.Visible = true;
            }
            else
            {
                MessageBox.Show("Mã xác nhận không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newPasswdTxt_TextChanged(object sender, EventArgs e)
        {
            string password = newPasswdTxt.Text;
            bool hasMinimumLength = password.Length >= 8;
            bool hasSpecialCharacter = password.Any(char.IsPunctuation);
            bool hasCapitalLetter = password.Any(char.IsUpper);

            if (!hasMinimumLength || !hasSpecialCharacter || !hasCapitalLetter)
            {
                newpasswdLbl.Text = "Mật khẩu cần ít nhất 8 ký tự, 1 ký tự đặc biệt và 1 chữ hoa";
                ispasswdValid = false;
            }
            else
            {
                newpasswdLbl.Text = "";
                ispasswdValid = true;
            }
        }

        private void renewPasswdTxt_TextChanged(object sender, EventArgs e)
        {
            isrepasswdValid = !string.IsNullOrEmpty(renewPasswdTxt.Text);
            renewPasswdLbl.Text = isrepasswdValid ? "" : "Mật khẩu xác nhận không được để trống";

            if (isrepasswdValid)
            {
                // Check confirm password similar password
                isrepasswdValid = newPasswdTxt.Text == renewPasswdTxt.Text;
                renewPasswdLbl.Text = isrepasswdValid ? "" : "Mật khẩu xác nhận không trùng khớp";
            }
        }

        private void resetPasswdBtn_Click(object sender, EventArgs e)
        {
            if (ispasswdValid && isrepasswdValid)
            {
                string hashedPassword = HashPassword(newPasswdTxt.Text);

                // Retrieve the user's data by email
                Data user = GetUserByEmail(forgetEmailTxt.Text);
                if (user != null)
                {
                    // Update only the password in the database
                    var updateData = new { password = hashedPassword };
                    FirebaseResponse updateResponse = client.Update("Users/" + user.username, updateData);

                    if (updateResponse.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Mật khẩu đã được thay đổi thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        resetPasswdPnl.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi thay đổi mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Tài khoản không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng đảm bảo mật khẩu hợp lệ và trùng khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showNewPasswdBtn_Click(object sender, EventArgs e)
        {
            newPasswdTxt.Focus();
            newPasswdTxt.UseSystemPasswordChar = false;
            newPasswdTxt.PasswordChar = default(char);
            showNewPasswdBtn.Hide();
            hideNewPasswdBtn.Show();
        }

        private void showNewrePasswdBtn_Click(object sender, EventArgs e)
        {
            renewPasswdTxt.Focus();
            renewPasswdTxt.UseSystemPasswordChar = false;
            renewPasswdTxt.PasswordChar = default(char);
            showNewrePasswdBtn.Hide();
            hideNewrePasswdBtn.Show();
        }

        private void hideNewPasswdBtn_Click(object sender, EventArgs e)
        {
            newPasswdTxt.Focus();
            newPasswdTxt.UseSystemPasswordChar = true;
            hideNewPasswdBtn.Hide();
            showNewPasswdBtn.Show();
        }

        private void hideNewrePasswdBtn_Click(object sender, EventArgs e)
        {
            renewPasswdTxt.Focus();
            renewPasswdTxt.UseSystemPasswordChar = true;
            hideNewrePasswdBtn.Hide();
            showNewrePasswdBtn.Show();
        }

        private async void googleBtn_Click(object sender, EventArgs e)
        {
            googleBtn.Text = "Loading...";

            try
            {
                UserCredential credential;
                string[] scopes = { Oauth2Service.Scope.UserinfoProfile, Oauth2Service.Scope.UserinfoEmail };

                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                    );
                }

                if (credential != null && !credential.Token.IsExpired(SystemClock.Default))
                {
                    var oauth2Service = new Oauth2Service(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "iConnect"
                    });

                    var userInfoRequest = oauth2Service.Userinfo.Get();
                    var userInfo = await userInfoRequest.ExecuteAsync();

                    if (userInfo != null)
                    {
                        string userEmail = userInfo.Email;
                        string userName = userInfo.Name.Replace(" ", "_");

                        client = new FireSharp.FirebaseClient(config);
                        if (client == null)
                        {
                            MessageBox.Show("Firebase connection failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            googleBtn.Text = "Login";
                            return;
                        }

                        Data user = GetUserByEmail(userEmail);
                        if (user != null)
                        {
                            FirebaseResponse response = client.Get("Users/" + user.username);
                            string actualUsername = response.ResultAs<Data>().username;

                            this.Hide();
                            Home home = new Home(actualUsername);
                            home.Closed += (s, args) => this.Close();
                            home.Show();
                        }
                        else
                        {
                            Data newUser = new Data
                            {
                                username = userName,
                                email = userEmail,
                                password = HashPassword("a")
                            };

                            SetResponse response = client.Set("Users/" + userName, newUser);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                MessageBox.Show("User registered successfully. Your default password is 'a'. Please change it in setting", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Hide();
                                Home home = new Home(userName);
                                home.Closed += (s, args) => this.Close();
                                home.Show();
                            }
                            else
                            {
                                MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                googleBtn.Text = "Login";
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve user info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        googleBtn.Text = "Login";
                    }
                }
                else
                {
                    // If token is expired, attempt to refresh it
                    if (credential != null && credential.Token.IsExpired(SystemClock.Default))
                    {
                        bool refreshed = await credential.RefreshTokenAsync(CancellationToken.None);
                        if (refreshed)
                        {
                            // Retry the operation after token refresh
                            googleBtn_Click(sender, e); // Recursive call to handle after token refresh
                        }
                        else
                        {
                            MessageBox.Show("Failed to refresh Google token. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            googleBtn.Text = "Login";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Google Sign-In failed or token expired.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        googleBtn.Text = "Login";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to log in. Error: {ex.Message}");
                googleBtn.Text = "Login";
            }
        }
    }
}
