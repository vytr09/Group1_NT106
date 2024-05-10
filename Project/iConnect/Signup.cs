using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net;
using System.Net.Mail;

namespace iConnect
{
    public partial class Signup : Form
    {
        bool isnameValid;
        bool isemailValid;
        bool isdobValid;
        bool isusernameValid;
        bool ispasswdValid;
        bool isrepasswdValid;
        private string confirmationCode;
        public Signup()
        {
            InitializeComponent();
            signupBtn.Enabled = false;
        }

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };

        IFirebaseClient client;

        private void Signup_Load(object sender, EventArgs e)
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

        //function to check email format
        private bool IsValidEmail(string email)
        {
            // Regular expression pattern for validating email format
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Check if the email matches the pattern
            return Regex.IsMatch(email, pattern);
        }

        private void UpdateButtonState()
        {
            signupBtn.Enabled = isnameValid && isemailValid && isdobValid && isusernameValid && ispasswdValid && isrepasswdValid;
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

        private void signupBtn_Click(object sender, EventArgs e)
        {

            // Generate confirmation code or link (you can replace this with your own logic)
            confirmationCode = GenerateConfirmationCode();
            signupBtn.Text = "";
            signupBtn.Checked = true;

            // Send confirmation email
            try
            {
                SendConfirmationEmail(emailTxt.Text, confirmationCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send confirmation email. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            confirmPnl.Visible = true;
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
            string senderEmail = "nt106group1@outlook.com"; // Your email address
            string senderPassword = "group1nt106"; // Your email password
            string smtpHost = "smtp-mail.outlook.com"; // Your SMTP host
            int smtpPort = 587; // Your SMTP port (e.g., 587 for Gmail)

            // Email content
            string subject = "iConnect - Xác nhận đăng ký";
            string body = $"Chào bạn,<br><br>Cảm ơn bạn đã đăng ký tài khoản!<br><br>Đây là mã xác nhận của bạn: " +
                $"<b>{confirmationCode}</b>. " +
                $"<br>Vui lòng sử dụng mã này để hoàn tất quá trình đăng ký tài khoản.<br>" +
                $"<br>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.<br>" +
                $"<br>Chúng tôi rất vui mừng được chào đón bạn vào cộng đồng của chúng tôi!<br><br>Trân trọng,<br>Đội ngũ quản trị viên";


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


        class Data
        {
            public string dateofb { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Signup.ActiveForm.Hide();
            Login login = new Login();
            login.Show();
        }

        private void closeAppBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nameTxt_TextChanged(object sender, EventArgs e)
        {
            isnameValid = !string.IsNullOrEmpty(nameTxt.Text);
            nameLbl.Text = isnameValid ? "" : "Tên không được để trống";
            UpdateButtonState();
        }

        private void emailTxt_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(emailTxt.Text))
            {
                emailLbl.Text = "Email không được để trống";
                isemailValid = false;
            }
            else if (!IsValidEmail(emailTxt.Text))
            {
                emailLbl.Text = "Email không hợp lệ";
                isemailValid = false;
            }
            else
            {
                FirebaseResponse responseEmail = client.Get("Users");
                if (responseEmail != null && responseEmail.Body != "null")
                {
                    Dictionary<string, Data> dataDict = responseEmail.ResultAs<Dictionary<string, Data>>();
                    isemailValid = !dataDict.Any(data => data.Value.email == emailTxt.Text);
                    emailLbl.Text = isemailValid ? "" : "Email đã tồn tại";
                }
                else
                {
                    isemailValid = true;
                    emailLbl.Text = "";
                }
            }
            UpdateButtonState();
        }

        private void dobTxt_TextChanged(object sender, EventArgs e)
        {
            // Lấy ngày tháng năm sinh từ TextBox
            string dob = dobTxt.Text;

            // Kiểm tra độ dài của chuỗi ngày tháng năm sinh
            if (dob.Length != 10)
            {
                dobLbl.Text = "Ngày tháng năm sinh không hợp lệ";
                isdobValid = false;
            }
            else
            {
                // Kiểm tra xem chuỗi ngày tháng năm sinh có thể được chuyển đổi thành một ngày hợp lệ không
                if (DateTime.TryParseExact(dob, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    // Nếu chuyển đổi thành công, đánh dấu là ngày tháng năm sinh hợp lệ và xóa thông báo lỗi
                    dobLbl.Text = "";
                    isdobValid = true;
                }
                else
                {
                    // Nếu không, hiển thị thông báo lỗi và đánh dấu là ngày tháng năm sinh không hợp lệ
                    dobLbl.Text = "Ngày tháng năm sinh không hợp lệ";
                    isdobValid = false;
                }
            }

            // Cập nhật trạng thái của nút Đăng ký
            UpdateButtonState();
        }

        private void usernameTxt_TextChanged(object sender, EventArgs e)
        {
            isusernameValid = !string.IsNullOrEmpty(usernameTxt.Text);
            usernameLbl.Text = isusernameValid ? "" : "Tên tài khoản không được để trống";
            if (isusernameValid)
            {
                FirebaseResponse response = client.Get("Users/" + usernameTxt.Text);
                isusernameValid = response == null || response.ResultAs<Data>() == null;
                usernameLbl.Text = isusernameValid ? "" : "Tên tài khoản đã tồn tại";
            }
            UpdateButtonState();
        }

        private void passwdTxt_TextChanged(object sender, EventArgs e)
        {
            string password = passwdTxt.Text;
            bool hasMinimumLength = password.Length >= 8;
            bool hasSpecialCharacter = password.Any(char.IsPunctuation);
            bool hasCapitalLetter = password.Any(char.IsUpper);

            if (!hasMinimumLength || !hasSpecialCharacter || !hasCapitalLetter)
            {
                passwdLbl.Text = "Mật khẩu cần ít nhất 8 ký tự, 1 ký tự đặc biệt và 1 chữ hoa";
                ispasswdValid = false;
            }
            else
            {
                passwdLbl.Text = "";
                ispasswdValid = true;
            }

            UpdateButtonState();
        }

        private void repasswdTxt_TextChanged(object sender, EventArgs e)
        {
            isrepasswdValid = !string.IsNullOrEmpty(repasswdTxt.Text);
            repasswdLbl.Text = isrepasswdValid ? "" : "Mật khẩu xác nhận không được để trống";

            if (isrepasswdValid)
            {
                // Check confirm password similar password
                isrepasswdValid = passwdTxt.Text == repasswdTxt.Text;
                repasswdLbl.Text = isrepasswdValid ? "" : "Mật khẩu xác nhận không trùng khớp";
            }

            UpdateButtonState();
        }

        private void hidePasswd_Click(object sender, EventArgs e)
        {
            passwdTxt.Focus();
            passwdTxt.UseSystemPasswordChar = true;
            hidePasswd.Hide();
            showPasswd.Show();
        }

        private void showPasswd_Click(object sender, EventArgs e)
        {
            passwdTxt.Focus();
            passwdTxt.UseSystemPasswordChar = false;
            passwdTxt.PasswordChar = default(char);
            showPasswd.Hide();
            hidePasswd.Show();
        }

        private void rePasswdHide_Click(object sender, EventArgs e)
        {
            repasswdTxt.Focus();
            repasswdTxt.UseSystemPasswordChar = true;
            rePasswdHide.Hide();
            rePasswdShow.Show();
        }

        private void rePasswdShow_Click(object sender, EventArgs e)
        {
            repasswdTxt.Focus();
            repasswdTxt.UseSystemPasswordChar = false;
            repasswdTxt.PasswordChar = default(char);
            rePasswdShow.Hide();
            rePasswdHide.Show();
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            // Assuming you have textbox for code input named "codeTextBox"
            string enteredCode = codeTxt.Text;

            if (enteredCode == confirmationCode)
            {
                // Hash the password using SHA256
                string hashedPassword = HashPassword(passwdTxt.Text);
                Data datalayer = new Data()
                {
                    username = usernameTxt.Text,
                    password = hashedPassword,
                    email = emailTxt.Text,
                    name = nameTxt.Text,
                    dateofb = dobTxt.Text
                };

                FirebaseResponse response1 = client.Set("Users/" + usernameTxt.Text, datalayer);
                MessageBox.Show("Đăng ký thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Mã xác nhận không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
