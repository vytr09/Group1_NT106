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
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;

namespace iConnect
{
    public partial class Search : Form
    {

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;


        class Data
        {
            public string dateofb { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string AvatarUrl { get; set; }
            public string bio { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string gender { get; set; }
            public string relationship { get; set; }
        }

        class Post
        {
            public Guid id { get; set; } = Guid.NewGuid();
            public string imageUrl { get; set; }
            public string caption { get; set; }
            public string created_at { get; set; }
            public string user { get; set; }
            public Data userData { get; set; }
            public List<string> comments { get; set; } = null;
            public List<string> likes { get; set; } = null;
        }

        

        public Search()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(config);

            if (client == null)
            {
                MessageBox.Show("Failed to connect to Firebase.");
            }

            // Add KeyDown event handler for searchTextBox
            searchTxt.KeyDown += SearchTextBox_KeyDown;

        }

        class PostDto
        {
            public Guid id { get; set; } = Guid.NewGuid();
            public string imageUrl { get; set; }
            public string caption { get; set; }
            public string created_at { get; set; }
            public string user { get; set; }
            public string comments { get; set; }
            public string likes { get; set; }
        }

        class Comment
        {
            public Guid id { get; set; } = Guid.NewGuid();
            public string user { get; set; }
            public string key { get; set; }
            public string comment { get; set; }
            public string postId { get; set; }
            public string parent_id { get; set; }
            public string created_at { get; set; }
            public Data userData { get; set; }
            public List<Comment> children { get; set; } = new List<Comment>();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent beep sound
                PerformSearch();
            }
        }

        private async void PerformSearch()
        {
            string searchText = searchTxt.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                // Perform search in Firebase by username
                FirebaseResponse response = await client.GetAsync("users");
                var users = response.ResultAs<Dictionary<string, Data>>();

                var filteredUsers = users.Values.Where(user => user.username.ToLower().Contains(searchText.ToLower())).ToList();

                // Display search results in postSortPnl
                postSortPnl.Controls.Clear();
                int y = 10;
                foreach (var user in filteredUsers)
                {
                    System.Windows.Forms.Label label = new System.Windows.Forms.Label
                    {
                        Text = $"Username: {user.username}, Email: {user.email}",
                        Location = new Point(10, y),
                        AutoSize = true
                    };
                    y += 30;
                    postSortPnl.Controls.Add(label);
                }
            }
        }

        private async void SearchByUsername(string username)
        {
            FirebaseResponse response = await client.GetAsync("posts");
            var posts = response.ResultAs<Dictionary<string, Post>>();

            var filteredPosts = new List<Post>();

            foreach (var post in posts.Values)
            {
                if (post.user == username)
                {
                    filteredPosts.Add(post);
                }
            }

            DisplayPosts(filteredPosts);
        }

        private async void SearchByPostName(string postName)
        {
            FirebaseResponse response = await client.GetAsync("posts");
            var posts = response.ResultAs<Dictionary<string, Post>>();

            var filteredPosts = posts.Values.Where(post => post.caption.ToLower().Contains(postName.ToLower())).ToList();

            DisplayPosts(filteredPosts);
        }

        private void DisplayPosts(List<Post> posts)
        {
            // Clear previous search results
            postSortPnl.Controls.Clear();

            // Display new search results
            foreach (var post in posts)
            {
                System.Windows.Forms.Panel postPanel = new System.Windows.Forms.Panel
                {
                    Width = 300,
                    Height = 150,
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                    Margin = new Padding(10)
                };

                System.Windows.Forms.Label captionLabel = new System.Windows.Forms.Label
                {
                    Text = $"Caption: {post.caption}",
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                System.Windows.Forms.Label userLabel = new System.Windows.Forms.Label
                {
                    Text = $"User: {post.user}",
                    AutoSize = true,
                    Location = new Point(10, 40)
                };

                System.Windows.Forms.Label createdAtLabel = new System.Windows.Forms.Label
                {
                    Text = $"Created At: {post.created_at}",
                    AutoSize = true,
                    Location = new Point(10, 70)
                };

                postPanel.Controls.Add(captionLabel);
                postPanel.Controls.Add(userLabel);
                postPanel.Controls.Add(createdAtLabel);

                postSortPnl.Controls.Add(postPanel);
            }
        }

        //private async void LoadPostByUser()
        //{
        //    try
        //    {
        //        this.loadingPostForUser.Visible = true;
        //        this.flowLayoutLoadPostImages.Visible = false;

        //        List<Post> posts = await this.getPosts();
        //        posts = posts.FindAll((t) => t.user.Equals(Username)).OrderByDescending(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", null)).ToList();

        //        int totalPost = posts.Count;

        //        totalPostOfUser.Text = $"{totalPost} Bài viết";

        //        if (flowLayoutLoadPostImages.Controls.Count > 0)
        //        {
        //            flowLayoutLoadPostImages.Controls.Clear();
        //        }

        //        foreach (Post post in posts)
        //        {
        //            Panel panel = new Panel
        //            {
        //                Width = 300,
        //                Height = 300,
        //                Margin = new Padding(5),
        //                BorderStyle = BorderStyle.FixedSingle
        //            };

        //            PictureBox pictureBox = new PictureBox
        //            {
        //                SizeMode = PictureBoxSizeMode.StretchImage,
        //                Width = 300,
        //                Height = 300
        //            };

        //            pictureBox.Click += new EventHandler(async (object sender, EventArgs e) =>
        //            {
        //                this.panelDetailPost.Visible = true;
        //                profileBtn.Checked = false;
        //                profilePnl.Visible = false;
        //                await LoadComments(post.id.ToString());
        //            });

        //            await LoadImageHttp.LoadImageAsync(post.imageUrl, pictureBox);

        //            Button deleteButton = new Button
        //            {
        //                Text = "X",
        //                ForeColor = Color.Red,
        //                BackColor = Color.Transparent,
        //                FlatStyle = FlatStyle.Flat,
        //                Size = new Size(30, 30),
        //                Location = new Point(270, 0),
        //                Cursor = System.Windows.Forms.Cursors.Hand
        //            };

        //            deleteButton.FlatAppearance.BorderSize = 0;
        //            deleteButton.BringToFront();
        //            deleteButton.Click += async (sender, e) =>
        //            {
        //                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bài viết `{post.caption}` này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //                if (result == DialogResult.Yes)
        //                {
        //                    await this.DeletePost(post.id.ToString());
        //                    await this.DeleteCommentByPost(post.id.ToString());

        //                    MessageBox.Show("Xóa bài viết và comment thành công!");
        //                    this._posts.Clear();

        //                    totalPost -= 1;

        //                    totalPostOfUser.Text = $"{totalPost} Bài viết";

        //                    flowLayoutLoadPostImages.Controls.Remove(panel);
        //                }
        //            };

        //            panel.Controls.Add(pictureBox);
        //            panel.Controls.Add(deleteButton);
        //            deleteButton.BringToFront();

        //            flowLayoutLoadPostImages.Controls.Add(panel);
        //        }
        //    }
        //    finally
        //    {
        //        this.loadingPostForUser.Visible = false;
        //        this.flowLayoutLoadPostImages.Visible = true;
        //    }
        //}

        //private async Task<string> UploadPostAsync(string imagePath)
        //{
        //    try
        //    {
        //        // Generate a unique identifier for the user's avatar
        //        Guid imageId = Guid.NewGuid();
        //        string fileExtension = Path.GetExtension(imagePath);

        //        // Upload image to Firebase Storage
        //        var task = new FirebaseStorage("iconnect-nt106.appspot.com")
        //            .Child("posts")
        //            .Child(imageId.ToString() + fileExtension)
        //            .PutAsync(File.OpenRead(imagePath));

        //        // Wait for the upload to complete
        //        var downloadUrl = await task;

        //        // Return the download URL
        //        return downloadUrl;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error uploading post: {ex.Message}");
        //        return null;
        //    }
        //}

        private async Task<List<Post>> getPosts()
        {
            FirebaseResponse response = await client.GetAsync("/posts");

            Console.Write($"log:::{response.Body}");

            List<Post> posts = new List<Post>();

            if (response.Body == "null")
            {
                return posts;
            }

            if (response != null)
            {
                var jsonPosts = JsonConvert.DeserializeObject<Dictionary<string, PostDto>>(response.Body);

                foreach (var item in jsonPosts)
                {
                    Post post = await this.mapperPost(item.Value);
                    posts.Add(post);
                }
            }

            return posts.OrderBy(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
        }

        private async Task<Post> mapperPost(PostDto postDto)
        {
            List<string> likes = JsonConvert.DeserializeObject<List<string>>(postDto.likes);
            List<string> comments = JsonConvert.DeserializeObject<List<string>>(postDto.comments);

            Data userData = await this.GetUserByUsername(postDto.user);

            Post post = new Post();
            post.user = postDto.user;
            post.likes = likes;
            post.comments = comments;
            post.id = postDto.id;
            post.userData = userData;
            post.imageUrl = postDto.imageUrl;
            post.caption = postDto.caption;
            post.created_at = postDto.created_at;

            return post;
        }

        //private async void LoadPost()
        //{
        //    if (this._posts.Count > 0)
        //    {
        //        return;
        //    }

        //    List<Post> posts = await this.getPosts();
        //    this._posts = posts;
        //    this.RenderPosts(posts);
        //}

        private async Task<Data> GetUserByUsername(string user)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + user);
            Data result = response.ResultAs<Data>();
            return result;
        }

        //private void RenderPosts(List<Post> posts)
        //{
        //    if (posts.Count < 0)
        //    {
        //        return;
        //    }

        //    if (this.panelRenderPost.Controls.Count > 0)
        //    {
        //        this.panelRenderPost.Controls.Clear();
        //    }

        //    bool isLoading = false;

        //    foreach (Post post in posts)
        //    {
        //        Console.WriteLine(post.id);
        //        isLoading = true;

        //        UC_Post panelPost = new UC_Post();

        //        panelPost.Name = $"{post.id}";
        //        panelPost.Dock = DockStyle.Top;
        //        panelPost.LabelCountComment = $"{post.comments.Count}";
        //        panelPost.LabelCountLike = $"{post.likes.Count}";
        //        panelPost.AuthorName = $"{post.userData.name}";
        //        panelPost.LoadPostPicture(post.imageUrl);
        //        panelPost.PostCaption = post.caption;
        //        panelPost.PostCreatedAt = this.FormatFacebookTime(post.created_at);
        //        panelPost.ButtonImageLike = !post.likes.Contains(Username) ? global::iConnect.Properties.Resources.heart : global::iConnect.Properties.Resources.redheart;

        //        if (!string.IsNullOrEmpty(post.userData.AvatarUrl))
        //        {
        //            panelPost.LoadAuthorAvatar(post.userData.AvatarUrl);
        //        }

        //        panelPost.ButtonCommentClick = new System.EventHandler((object sender, EventArgs e) =>
        //        {
        //            this.handleClickBtnComment(post.id.ToString());
        //        });

        //        panelPost.ButtonLikeClick = new System.EventHandler((object sender, EventArgs e) =>
        //        {
        //            this.handleClickBtnLike(post.id.ToString(), panelPost);
        //        });

        //        this.panelRenderPost.Controls.Add(panelPost);

        //        isLoading = false;
        //    }

        //    if (!isLoading)
        //    {
        //        this.panelLoadingPost.Visible = false;
        //        this.panelRenderPost.Visible = true;
        //    }
        //}

        private async Task<Post> getPostById(string postId)
        {
            // Retrieve the user data from the database
            FirebaseResponse response = await client.GetAsync("/posts/" + postId);
            PostDto post = response.ResultAs<PostDto>();
            return await mapperPost(post);
        }

        private PostDto mapperToPostDto(Post post)
        {
            PostDto postDto = new PostDto();

            postDto.user = post.user;
            postDto.comments = JsonConvert.SerializeObject(post.comments);
            postDto.likes = JsonConvert.SerializeObject(post.likes);
            postDto.created_at = post.created_at;
            postDto.caption = post.caption;
            postDto.id = post.id;
            postDto.imageUrl = post.imageUrl;

            return postDto;
        }

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
            userBtn.Checked =false;

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
