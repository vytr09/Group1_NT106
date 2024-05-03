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

namespace iConnect
{
    public partial class Signup : Form
    {
        public Signup()
        {
            InitializeComponent();
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
                    MessageBox.Show("Connection Success");
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

        private void signupBtn_Click(object sender, EventArgs e)
        {
            Data datalayer = new Data()
            {
                username = usernameTxt.Text,
                password = passwdTxt.Text,
                email = guna2TextBox5.Text
            };

            // Check if username is valid
            if (string.IsNullOrEmpty(usernameTxt.Text))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            // Check if email is valid
            if (string.IsNullOrEmpty(guna2TextBox5.Text))
            {
                MessageBox.Show("Please enter an email.");
                return;
            }

            // Check if full name is valid
            if (string.IsNullOrEmpty(guna2TextBox6.Text))
            {
                MessageBox.Show("Please enter full name.");
                return;
            }

            // Check if birthday is valid
            if (string.IsNullOrEmpty(guna2TextBox4.Text))
            {
                MessageBox.Show("Please enter birthday.");
                return;
            }

            // Check if password is valid
            if (string.IsNullOrEmpty(passwdTxt.Text))
            {
                MessageBox.Show("Please enter password.");
                return;
            }

            //Check if confirm password is valid
            if (string.IsNullOrEmpty(repasswdTxt.Text))
            {
                MessageBox.Show("Please enter confirm password.");
                return;
            }

            //Check confirm password similar password
            if (passwdTxt.Text != repasswdTxt.Text)
            {
                MessageBox.Show("Confirm password and password do not match");
                return;
            }

            // Check if the entered email has correct syntax
            if (!IsValidEmail(guna2TextBox5.Text))
            {
                MessageBox.Show("Invalid email format. Please enter a valid email address.");
                return;
            }

            // Check if email already exists
            FirebaseResponse responseEmail = client.Get("Information");
            if (responseEmail != null && responseEmail.Body != "null")
            {
                Dictionary<string, Data> dataDict = responseEmail.ResultAs<Dictionary<string, Data>>();
                foreach (var data in dataDict)
                {
                    if (data.Value.email == guna2TextBox5.Text)
                    {
                        MessageBox.Show("Email already exists. Please enter another email.");
                        return;
                    }
                }
            }

            //Check if username already exists
            FirebaseResponse response = client.Get("Information/" + usernameTxt.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                Data result = response.ResultAs<Data>();

                if (usernameTxt.Text.Equals(result.username))
                {
                    MessageBox.Show("This ID already exists");
                }
            }
            else
            {
                FirebaseResponse response1 = client.Set("Information/" + usernameTxt.Text, datalayer);
                MessageBox.Show("Data is inserted");
            }
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

        private void closeAppBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }
    }
}
