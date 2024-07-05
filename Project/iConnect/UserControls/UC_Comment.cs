using iConnect.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace iConnect.UserControls
{
    public partial class UC_Comment : UserControl
    {
        public string CommentText
        {
            get { return this.labelComment.Text; }
            set { this.labelComment.Text = value; }
        }

        public string AuthorText
        {
            get { return this.labelAuthorName.Text; }
            set { this.labelAuthorName.Text = value; }
        }

        public string CreatedAtText
        {
            get { return this.labelCreatedAt.Text; }
            set { this.labelCreatedAt.Text = value; }
        }

        public async void LoadAvatarAuthor(string image)
        {
            await LoadImageHttp.LoadImageAsync(image, this.postAvatarAuthor);
        }
        public void LoadAvatarAuthor(Bitmap image)
        {
            this.postAvatarAuthor.Image = image;
        }

        public bool ShowDelete
        {
            get { return this.buttonDeleteComment.Visible; }
            set { this.buttonDeleteComment.Visible = value; }
        }
        public System.EventHandler ButtonDeleteClick
        {
            set { this.buttonDeleteComment.Click += value; }
        }

        public Padding MarginWrapAvatar
        {
            get { return this.panelWrapperAvatar.Padding; }
            set { this.panelWrapperAvatar.Padding = value; }
        }

        public System.EventHandler ButtonReplyClick
        {
            set { this.btnReply.Click += value; }
        }

        public UC_Comment()
        {
            InitializeComponent();
        }

        private void btnReply_Click(object sender, System.EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
