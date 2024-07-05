using Firebase.Storage;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using iConnect.Helpers;
using iConnect.UserControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;

namespace iConnect
{
    public partial class Search : Form
    {

       

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void postBtn_Click(object sender, EventArgs e)
        {
            postBtn.Checked = true;
            allBtn.Checked = false;
            recentBtn.Checked = false;
            userBtn.Checked = false;

            all2Pnl.Visible = false;
            sort2Pnl.Visible = false;

            postSortPnl.Visible = true;
            userSortPnl.Visible = false;
        }

        private void allBtn_Click(object sender, EventArgs e)
        {
            postBtn.Checked = false;
            allBtn.Checked = true;
            recentBtn.Checked = false;
            userBtn.Checked = false;

            all2Pnl.Visible = true;
            sort2Pnl.Visible = true;
            postSortPnl.Visible = false;
            userSortPnl.Visible = false;
        }

        private void recentBtn_Click(object sender, EventArgs e)
        {
            postBtn.Checked = false;
            allBtn.Checked = false;
            recentBtn.Checked = true;
            userBtn.Checked = false;

            all2Pnl.Visible = false;
            sort2Pnl.Visible = false;
            postSortPnl.Visible = true;
            userSortPnl.Visible = false;
        }

        private void userBtn_Click(object sender, EventArgs e)
        {
            postBtn.Checked = false;
            allBtn.Checked = false;
            recentBtn.Checked = false;
            userBtn.Checked = true;

            all2Pnl.Visible = false;
            sort2Pnl.Visible = false;
            postSortPnl.Visible = false;
            userSortPnl.Visible = true;
        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void userSortPnl_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}