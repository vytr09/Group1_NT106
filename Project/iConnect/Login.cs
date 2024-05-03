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

        private void loginBtn_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = client.Get("Information/" + usernameTxt.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                Data result = response.ResultAs<Data>();

                if (usernameTxt.Text.Equals(result.username) && passwdTxt.Text.Equals(result.password))
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
            else
            {
                passwdLbl.Text = "";
                loginBtn.Enabled = !string.IsNullOrEmpty(usernameTxt.Text);
            }
        }
    }
}
