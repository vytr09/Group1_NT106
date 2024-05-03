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
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
        }

        private void homeBtn_Click(object sender, EventArgs e)
        {
            homePnl.Visible = true;
            homeBtn.Checked = true;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            msgBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
        }

        private void notiBtn_Click(object sender, EventArgs e)
        {
            homePnl.Visible = false;
            notiBtn.Checked = true;
            searchBtn.Checked = false;
            homeBtn.Checked = false;
            msgBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            homePnl.Visible = false;
            notiBtn.Checked = false;
            homeBtn.Checked = false;
            searchBtn.Checked = true;
            searchPnl.Visible = true;
            msgBtn.Checked = false;
            msgPnl.Visible = false;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
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

        private void userBtn_Click_1(object sender, EventArgs e)
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

        private void recentBtn_Click_1(object sender, EventArgs e)
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

        private void guna2Button7_Click(object sender, EventArgs e)
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

        private void msgBtn_Click(object sender, EventArgs e)
        {
            homePnl.Visible = false;
            msgBtn.Checked = true;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            homeBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = true;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {

        }

        private void followerBtn_Click(object sender, EventArgs e)
        {
            followerPnl.Visible = true;
        }

        private void followingBtn_Click(object sender, EventArgs e)
        {
            followingPnl.Visible = true;
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            followingPnl.Visible = false;
        }

        private void guna2ControlBox3_Click(object sender, EventArgs e)
        {
            followerPnl.Visible = false;
        }

        private void profileBtn_Click(object sender, EventArgs e)
        {
            homePnl.Visible = false;
            profileBtn.Checked = true;
            msgBtn.Checked = false;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            homeBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profilePnl.Visible = true;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
        }

        private void chinhsuatrangcanhan_Click(object sender, EventArgs e)
        {
            //bam cai dat
            panel7.Visible = true;
            guna2Panel9.Visible = false;
            guna2Panel10.Visible = false;
            guna2Panel13.Visible = false;
            guna2Panel12.Visible = false;
            panel3.Visible = false;
        }

        private void trangthaitaikhoan_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
            panel7.Visible = false;
        }

        private void thongbao_Click(object sender, EventArgs e)
        {
            panel7.Visible = false;
            guna2Panel10.Visible = true;
            guna2Panel13.Visible = false;
            guna2Panel12.Visible = false;
            panel3.Visible = false;
        }

        private void quyenriengtu_Click(object sender, EventArgs e)
        {
            //bam quyen rieng tu
            panel7.Visible = false;
            guna2Panel9.Visible = true;
            guna2Panel10.Visible = false;
            guna2Panel13.Visible = false;
            panel3.Visible = false;
        }

        private void chan_Click(object sender, EventArgs e)
        {
            panel7.Visible = false;
            guna2Panel13.Visible = true;
            guna2Panel12.Visible = false;
            panel3.Visible = false;
        }

        private void trogiup_Click(object sender, EventArgs e)
        {
            guna2Panel12.Visible = true;
            panel3.Visible = false;
        }

        private void settingBtn_Click(object sender, EventArgs e)
        {
            homePnl.Visible = false;
            profileBtn.Checked = false;
            settingBtn.Checked = true;
            msgBtn.Checked = false;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            homeBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profilePnl.Visible = false;
            settingPnl.Visible = true;

            //bam cai dat
            panel7.Visible = true;
            guna2Panel9.Visible = false;
            guna2Panel10.Visible = false;
            guna2Panel13.Visible = false;
            guna2Panel12.Visible = false;
            panel3.Visible = false;
        }

        private void guna2Panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void closeAppBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void allnotiBtn_Click(object sender, EventArgs e)
        {
            recnotiBtn.Checked = false;
            allnotiBtn.Checked = true;
            mennotiBtn.Checked = false;
        }

        private void recnotiBtn_Click(object sender, EventArgs e)
        {
            recnotiBtn.Checked = true;
            allnotiBtn.Checked = false;
            mennotiBtn.Checked = false;
        }

        private void mennotiBtn_Click(object sender, EventArgs e)
        {
            mennotiBtn.Checked = true;
            recnotiBtn.Checked = false;
            allnotiBtn.Checked = false;
        private void homePnl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trendPnl_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnForYou_Click(object sender, EventArgs e)
        {
            btnForYou.Checked = true;
            btnFollowing.Checked = false;

            pnlForYou.Visible = true;
            pnlFollowing.Visible = false;
        }

        private void btnFollowing_Click(object sender, EventArgs e)
        {
            btnFollowing.Checked = true;
            btnForYou.Checked = false;

            pnlFollowing.Visible = true;
            pnlForYou.Visible = false;
        }
    }
}
