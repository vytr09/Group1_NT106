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
        public Login()
        {
            InitializeComponent();
            loginBtn.Enabled = false;
            Data datalayer = new Data()
            {
                username = usernameTxt.Text,
                password = passwdTxt.Text,
            };
        }

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };

        IFirebaseClient client;

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

        private void loginBtn_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = client.Get("Information/" + usernameTxt.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                Data result = response.ResultAs<Data>();

                if (usernameTxt.Text.Equals(result.username))
                {
                    // Check if password matches the password stored in the database
                    if (passwdTxt.Text.Equals(result.password))
                    {
                        Login.ActiveForm.Hide();
                        Home home = new Home();
                        home.Closed += (s, args) => this.Close();
                        home.Show();
                        //MessageBox.Show("Login successful!");
                    }
                    else
                    {
                        passwdLbl.Text = "Tài khoản hoặc mật khẩu không chính xác.";
                    }
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
            // Hide the current instance of the login form
            this.Hide();

            // Show the signup form
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
            }
        }

        private void passwdTxt_TextChanged(object sender, EventArgs e)
        {
            // Check if password is valid
            if (string.IsNullOrEmpty(passwdTxt.Text))
            {
                passwdLbl.Text = "Mật khẩu không được để trống";
            }
            else
            {
                passwdLbl.Text = "";
                if (string.IsNullOrEmpty(usernameTxt.Text))
                {
                    usernameLbl.Text = "Tên tài khoản không được để trống";
                    loginBtn.Enabled = false;
                }
                else
                {
                    usernameLbl.Text = "";
                    loginBtn.Enabled = true;
                }
            }
        }
    }
}
