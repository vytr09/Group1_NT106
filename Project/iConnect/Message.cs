using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace iConnect
{
    public partial class Message : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;

        public Message()
        {
            InitializeComponent();
        }

        public class Messages
        {
            public string Text { get; set; }
            public string UserName { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private void guna2GradientButton5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton16_Click(object sender, EventArgs e)
        {
            string messageText = guna2TextBox1.Text;
            if (!string.IsNullOrEmpty(messageText))
            {
                var messages = new Messages
                {
                    Text = messageText,
                    //UserName = _currentUserName,
                    Timestamp = DateTime.Now
                };
                FirebaseResponse response = client.Set("Chats/" + messages.Timestamp, messages);
                guna2TextBox1.Clear();
            }
        }
    }
}
