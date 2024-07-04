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

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;
        private string Username;


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



        public Search(string username)
        {
            InitializeComponent();
            Username = username;
            client = new FireSharp.FirebaseClient(config);

            searchTxt.KeyDown += new KeyEventHandler(SearchTextBox_KeyDown);
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

        private async void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    e.SuppressKeyPress = true; // Prevent beep sound
            //    PerformSearch();
            //}

            if (e.KeyCode == Keys.Enter)
            {
                string searchUsername = searchTxt.Text.Trim();

                if (string.IsNullOrEmpty(searchUsername))
                {
                    MessageBox.Show("Please enter a username to search.");
                    return;
                }

                List<Post> posts = await GetPostsByUsername(searchUsername);

                if (posts.Count > 0)
                {
                    RenderPosts(posts);
                }
                else
                {
                    MessageBox.Show("No posts found for this username.");
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private async Task<List<Post>> GetPostsByUsername(string username)
        {
            FirebaseResponse response = await client.GetAsync("/posts");
            var jsonPosts = JsonConvert.DeserializeObject<Dictionary<string, PostDto>>(response.Body);
            List<Post> posts = new List<Post>();

            foreach (var item in jsonPosts)
            {
                Post post = await MapperPost(item.Value);
                if (post.user.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    posts.Add(post);
                }
            }

            return posts;
        }

        private async Task<Post> MapperPost(PostDto postDto)
        {
            List<string> likes = JsonConvert.DeserializeObject<List<string>>(postDto.likes);
            List<string> comments = JsonConvert.DeserializeObject<List<string>>(postDto.comments);

            Data userData = await GetUserByUsername(postDto.user);

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

        private void RenderPosts(List<Post> posts)
        {
            postSortPnl.Controls.Clear();

            foreach (Post post in posts)
            {
                System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel
                {
                    Width = 300,
                    Height = 300,
                    Margin = new Padding(5),
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
                };

                System.Windows.Forms.Label lblUsername = new System.Windows.Forms.Label
                {
                    Text = post.user,
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                PictureBox pictureBox = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 300,
                    Height = 200,
                    Location = new Point(0, 30)
                };

                LoadImageAsync(post.imageUrl, pictureBox);

                System.Windows.Forms.Label lblCaption = new System.Windows.Forms.Label
                {
                    Text = post.caption,
                    AutoSize = true,
                    Location = new Point(10, 240)
                };

                panel.Controls.Add(lblUsername);
                panel.Controls.Add(pictureBox);
                panel.Controls.Add(lblCaption);

                postSortPnl.Controls.Add(panel);
            }
        }

        private async void LoadImageAsync(string imageUrl, PictureBox pictureBox)
        {
            byte[] imageData;
            using (var webClient = new WebClient())
            {
                imageData = await webClient.DownloadDataTaskAsync(imageUrl);
            }

            pictureBox.Image = System.Drawing.Image.FromStream(new MemoryStream(imageData));
        }



        //private async void PerformSearch()
        //{
        //    string searchText = searchTxt.Text;
        //    if (!string.IsNullOrEmpty(searchText))
        //    {
        //        // Perform search in Firebase by username
        //        FirebaseResponse response = await client.GetAsync("users");
        //        var users = response.ResultAs<Dictionary<string, Data>>();

        //        var filteredUsers = users.Values.Where(user => user.username.ToLower().Contains(searchText.ToLower())).ToList();

        //        // Display search results in postSortPnl
        //        postSortPnl.Controls.Clear();
        //        int y = 10;
        //        foreach (var user in filteredUsers)
        //        {
        //            System.Windows.Forms.Label label = new System.Windows.Forms.Label
        //            {
        //                Text = $"Username: {user.username}, Email: {user.email}",
        //                Location = new Point(10, y),
        //                AutoSize = true
        //            };
        //            y += 30;
        //            postSortPnl.Controls.Add(label);

        //            // Search and display posts by this username
        //            await SearchByUsername(user.username);
        //        }
        //    }
        //}

        //private async Task SearchByUsername(string username)
        //{
        //    FirebaseResponse response = await client.GetAsync("posts");
        //    var posts = response.ResultAs<Dictionary<string, Post>>();

        //    var filteredPosts = posts.Values.Where(post => post.user == username).ToList();

        //    DisplayPosts(filteredPosts);
        //}


        //private async void SearchByPostName(string postName)
        //{
        //    FirebaseResponse response = await client.GetAsync("posts");
        //    var posts = response.ResultAs<Dictionary<string, Post>>();

        //    var filteredPosts = posts.Values.Where(post => post.caption.ToLower().Contains(postName.ToLower())).ToList();

        //    DisplayPosts(filteredPosts);
        //}

        //private void DisplayPosts(List<Post> posts)
        //{
        //    // Clear previous search results
        //    postSortPnl.Controls.Clear();

        //    int y = 10;

        //    // Display new search results
        //    foreach (var post in posts)
        //    {
        //        System.Windows.Forms.Panel postPanel = new System.Windows.Forms.Panel
        //        {
        //            Width = 300,
        //            Height = 150,
        //            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
        //            Location = new Point(10, y)
        //        };

        //        System.Windows.Forms.Label captionLabel = new System.Windows.Forms.Label
        //        {
        //            Text = $"Caption: {post.caption}",
        //            AutoSize = true,
        //            Location = new Point(10, 10)
        //        };

        //        System.Windows.Forms.Label userLabel = new System.Windows.Forms.Label
        //        {
        //            Text = $"User: {post.user}",
        //            AutoSize = true,
        //            Location = new Point(10, 40)
        //        };

        //        System.Windows.Forms.Label createdAtLabel = new System.Windows.Forms.Label
        //        {
        //            Text = $"Created At: {post.created_at}",
        //            AutoSize = true,
        //            Location = new Point(10, 70)
        //        };

        //        System.Windows.Forms.PictureBox postImage = new System.Windows.Forms.PictureBox
        //        {
        //            Width = 100,
        //            Height = 100,
        //            Location = new Point(200, 10),
        //            SizeMode = PictureBoxSizeMode.StretchImage
        //        };

        //        // Load the image from URL
        //        LoadImageFromUrl(post.imageUrl, postImage);

        //        postPanel.Controls.Add(captionLabel);
        //        postPanel.Controls.Add(userLabel);
        //        postPanel.Controls.Add(createdAtLabel);
        //        postPanel.Controls.Add(postImage);

        //        postSortPnl.Controls.Add(postPanel);

        //        y += 160; // Adjust for the next post
        //    }
        //}

        //private void LoadImageFromUrl(string url, PictureBox pictureBox)
        //{
        //    try
        //    {
        //        pictureBox.Load(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Failed to load image: " + ex.Message);
        //    }
        //}


        private async Task<Data> GetUserByUsername(string user)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + user);
            Data result = response.ResultAs<Data>();
            return result;
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