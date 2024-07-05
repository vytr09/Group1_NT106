using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iConnect
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void quyenriengtu_Click(object sender, EventArgs e)
        {
            //bam quyen rieng tu
            panel7.Visible = false;
            guna2Panel5.Visible = true;
            guna2Panel6.Visible = false;
            guna2Panel7.Visible = false;
            panel3.Visible = false;
        }

        private void chinhsuaquyenriengtu_Click(object sender, EventArgs e)
        {
            //bam cai dat
            panel7.Visible = true;
            guna2Panel5.Visible = false;
            guna2Panel6.Visible = false;
            guna2Panel7.Visible = false;
            guna2Panel8.Visible = false;
            panel3.Visible = false;
        }

        private void caidat_Click(object sender, EventArgs e)
        {
            //bam cai dat
            panel7.Visible = true;
            guna2Panel5.Visible = false;
            guna2Panel6.Visible = false;
            guna2Panel7.Visible = false;
            guna2Panel8.Visible = false;
            panel3.Visible = false;
        }

        private void thongbao_Click(object sender, EventArgs e)
        {
            guna2Panel6.Visible = true;
            guna2Panel7.Visible = false;
            guna2Panel8.Visible = false;
            panel3.Visible = false;
        }

        private void chan_Click(object sender, EventArgs e)
        {
            guna2Panel7.Visible = true;
            guna2Panel8.Visible = false;
            panel3.Visible = false;
        }

        private void trogiup_Click(object sender, EventArgs e)
        {
            guna2Panel8.Visible = true;
            panel3.Visible = false;
        }

        private void trangthaitaikhoan_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
        }
    }
}
