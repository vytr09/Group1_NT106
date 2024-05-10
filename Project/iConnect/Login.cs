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
using Guna.UI2.WinForms;

namespace iConnect
{
    public partial class Login : Form
    {
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
                password = passwdTxt.Text,
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
                    Home home = new Home();
                    home.Closed += (s, args) => this.Close();
                    home.Show();
                }
                else
                {
                    passwdLbl.Text = "Tài khoản hoặc mật khẩu không chính xác.";
                }
            }
            else
            {
                passwdLbl.Text = "Tài khoản hoặc mật khẩu không chính xác.";
            }
        }

        class Data
        {
            public string username { get; set; }
            public string password { get; set; }

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
    }
}
