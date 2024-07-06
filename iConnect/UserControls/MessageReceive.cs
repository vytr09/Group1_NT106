using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iConnect.UserControls
{
    public partial class MessageReceive : UserControl
    {
        public MessageReceive()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return MessageContent.Text; }
            set { MessageContent.Text = value; }
        }

        public string Timestamp
        {
            get { return MessageTime.Text; }
            set { MessageTime.Text = value; }
        }
    }
}
