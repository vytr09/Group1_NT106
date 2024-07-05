using iConnect.Helpers;
using System.Net.Http;
using System.Windows.Forms;

namespace iConnect.UserControls
{
    public partial class UC_Post : UserControl
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public string PostCaption
        {
            get { return this.labelCaption.Text; }
            set { this.labelCaption.Text = value; }
        }

        public string AuthorName
        {
            get { return this.labelAuthorName.Text; }
            set { this.labelAuthorName.Text = value; }
        }

        public string PostCreatedAt
        {
            get { return this.labelCreatedAt.Text; }
            set { this.labelCreatedAt.Text = value; }
        }

        public async void LoadPostPicture(string url)
        {
            await LoadImageHttp.LoadImageAsync(url, this.picturePost);
        }

        public async void LoadAuthorAvatar(string url)
        {
            await LoadImageHttp.LoadImageAsync(url, this.postAvatarAuthor);
        }

        public string LabelCountComment
        {
            get { return this.labelCountComment.Text; }
            set { this.labelCountComment.Text = value; }
        }

        public string LabelCountLike
        {
            get { return this.labelCountLike.Text; }
            set { this.labelCountLike.Text = value; }
        }

        public System.EventHandler ButtonLikeClick
        {
            set { this.btnLike.Click += value; }
        }

        public System.EventHandler ButtonCommentClick
        {
            set { this.btnComment.Click += value; }
        }

        public System.Drawing.Image ButtonImageLike
        {
            get { return this.btnLike.Image; }
            set { this.btnLike.Image = value; }
        }

        public UC_Post()
        {
            InitializeComponent();
        }
    }
}
