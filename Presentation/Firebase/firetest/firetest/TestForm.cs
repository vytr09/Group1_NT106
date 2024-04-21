using System;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace firetest
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "rvtKMJ2TLCrHs2FnSBUPryhSihVbPRVoTfzYJWiX",
            BasePath = "https://projectfirebase-7ebe1-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        private void Form1_Load(object sender, EventArgs e)
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


        private async void btnAdd_Click(object sender, EventArgs e)
        {
            Data datalayer = new Data()
            {
                ID = txtID.Text,
                Name = txtName.Text,
                Age = txtAge.Text
            };

            FirebaseResponse response = client.Get("Information/" + txtID.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                Data result = response.ResultAs<Data>();

                if (txtID.Text.Equals(result.ID))
                {
                    MessageBox.Show("This ID already exists");
                }
            }
            else
            {
                FirebaseResponse response1 = client.Set("Information/" + txtID.Text, datalayer);
                MessageBox.Show("Data is inserted");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = client.Get("Information/" + txtID.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                Data result = response.ResultAs<Data>();

                if (txtID.Text.Equals(result.ID))
                {
                    txtName.Text = result.Name;
                    txtAge.Text = result.Age;
                }
            }
            else
            {
                MessageBox.Show("Data not found for the specified ID!!");
                txtName.Clear();
                txtAge.Clear();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = client.Get("Information/" + txtID.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                var result = new Data
                {
                    ID = txtID.Text,
                    Name = txtName.Text,
                    Age = txtAge.Text
                };

                client.Update("Information/" + txtID.Text, result);
                MessageBox.Show("Data update success");
            }
            else
            {
                MessageBox.Show("Data not found for the specified ID!!");
                txtName.Clear();
                txtAge.Clear();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = client.Get("Information/" + txtID.Text);

            if (response != null && response.ResultAs<Data>() != null)
            {
                client.Delete("Information/" + txtID.Text);
                MessageBox.Show("Data Deleted");
            }
            else
            {
                MessageBox.Show("Data not found for the specified ID!!");
                txtName.Clear();
                txtAge.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtID.Clear();
            txtName.Clear();
            txtAge.Clear();
        }
    }
}
