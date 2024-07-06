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
    public partial class UC_Conversation : UserControl
    {
        public UC_Conversation()
        {
            InitializeComponent();
        }

        public string UserID { get; set; }
        public long LastMessageTimestamp { get; set; }
        private string _name;
        private Image _avatar;

        public string Item_Name
        {
            get { return _name; }
            set { _name = value; UserName.Text = value; }
        }

        public Image Item_Avatar
        {
            get { return _avatar; }
            set { _avatar = value; UserAvatar.Image = value; }
        }

        public void UC_Conversation_Click(object sender, EventArgs e)
        {

        }
    }
}
