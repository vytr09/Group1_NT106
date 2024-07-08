using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using iConnect.Helpers;
using iConnect.UserControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading;

namespace iConnect
{
    public partial class Home : Form
    {
        private string pathImageUpload = string.Empty;
        private List<Post> _posts = new List<Post>();
        private string selectedPostId = string.Empty;
        private string selectedCommentId = string.Empty;

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

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "LvKz9QEmzDWQ6ncJrL9woSgNH9IChypOchmOSTOB",
            BasePath = "https://iconnect-nt106-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;
        private string Username;
        public Home(string username)
        {
            InitializeComponent();
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Username = username;
            reload();
            this.Load += async (sender, e) => await Form_Load(sender, e);
            // Gắn sự kiện KeyPress cho textbox tìm kiếm
            searchTxt.KeyPress += new KeyPressEventHandler(SearchTextBox_KeyPress);
            searchTxt.TextChanged += new EventHandler(searchTxt_TextChanged);
            notiBtn.CheckedChanged += new EventHandler(notiBtn_CheckedChanged);
        }

        // Call this method when the form loads to populate the blocked users panel
        private async Task Form_Load(object sender, EventArgs e)
        {
            await DisplayRandomUsers();
            await CheckForUnreadNotifications();
            await DisplayUsers();
            await DisplayUsers1();
        }

        // profile and post render
        // profile and post render
        // profile and post render
        private async void reload()
        {
            this.panelDetailPost.Visible = false;

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Users/" + Username);
            Data result = response.ResultAs<Data>();

            proBtn.Text = "@" + result.username;
            await LoadAvatarAsync(Username);

            if (this.homeBtn.Checked)
            {
                if (this.panelRenderPost.Controls.Count > 0)
                {
                    this.panelRenderPost.Controls.Clear();
                }

                this.panelLoadingPost.Visible = true;
                this.panelRenderPost.Visible = false;

                if (this._posts.Count > 0)
                {
                    this.RenderPosts(this._posts);
                }
                else
                {
                    LoadPost();
                }
            }

            if (this.profileBtn.Checked)
            {
                usrName.Text = result.username;
                fullName.Text = result.name;
                dateofBirth.Text = "Ngày sinh: " + result.dateofb;
                changeUsrname.Text = result.username;
                proPnlAddress.Text = "Sinh sống tại: " + result.city;
                proPnlFromLbl.Text = "Đến từ: " + result.country;
                bioLbl.Text = result.bio;
                await LoadProfileAvatarAsync(Username);
            }

            if (this.settingBtn.Checked)
            {
                // Assuming the date is stored in dd/MM/yyyy format
                string dateFormat = "dd/MM/yyyy";

                try
                {
                    if (!string.IsNullOrEmpty(result.dateofb))
                    {
                        changeBDay.Value = DateTime.ParseExact(result.dateofb, dateFormat, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // Handle the case where dateofb is null or empty
                        changeBDay.Value = DateTime.Now; // Or set to a default date
                    }
                }
                catch (FormatException ex)
                {
                    // Handle parsing errors
                    MessageBox.Show($"Failed to parse date of birth. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    changeBDay.Value = DateTime.Now; // Or set to a default date
                }

                bioTxtChange.Text = result.bio;
                changeFullName.Text = result.name;
                countryTxt.Text = result.country;
                currentCity.Text = result.city;
                genderCmb.Text = result.gender;
                relaCmb.Text = result.relationship;
            }
        }

        // Method to load the avatar image from the AvatarUrl
        private async Task LoadAvatarAsync(string username)
        {
            try
            {
                // Retrieve the user data from the database
                FirebaseResponse response = await client.GetAsync("Users/" + username);
                Data userData = response.ResultAs<Data>();

                // Check if the user has an avatar URL
                if (!string.IsNullOrEmpty(userData.AvatarUrl))
                {
                    // Load the avatar image using the AvatarUrl
                    byte[] imageData;
                    using (var webClient = new WebClient())
                    {
                        imageData = await webClient.DownloadDataTaskAsync(userData.AvatarUrl);
                    }

                    // Resize the image to fit the PictureBox
                    Image resizedImage = ResizeImage(Image.FromStream(new MemoryStream(imageData)), avatarPro.Width, avatarPro.Height);

                    // Load the resized image into the PictureBox
                    avatarPro.Image = resizedImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading avatar: {ex.Message}");
            }
        }
        private async Task LoadProfileAvatarAsync(string username)
        {
            try
            {
                // Retrieve the user data from the database
                FirebaseResponse response = await client.GetAsync("Users/" + username);
                Data userData = response.ResultAs<Data>();
                proPnlAvt.InitialImage = Properties.Resources.profile;

                // Check if the user has an avatar URL
                if (!string.IsNullOrEmpty(userData.AvatarUrl))
                {
                    // Load the avatar image using the AvatarUrl
                    byte[] imageData;
                    using (var webClient = new WebClient())
                    {
                        imageData = await webClient.DownloadDataTaskAsync(userData.AvatarUrl);
                    }

                    // Resize the image to fit the PictureBox
                    Image resizedImage2 = ResizeImage(Image.FromStream(new MemoryStream(imageData)), proPnlAvt.Width, proPnlAvt.Height);
                    proPnlAvt.Image = resizedImage2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading avatar: {ex.Message}");
            }
        }
        private Image ResizeImage(Image image, int width, int height)
        {
            // Create a new Bitmap with the desired width and height
            Bitmap resizedImage = new Bitmap(width, height);

            // Create a Graphics object from the resized image
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                // Set the interpolation mode to high quality bicubic
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Draw the original image onto the resized image using DrawImage method
                graphics.DrawImage(image, 0, 0, width, height);
            }

            // Return the resized image
            return resizedImage;
        }

        // end of profile and post render
        // end of profile and post render
        // end of profile and post render

        // main buttons

        private void homeBtn_Click(object sender, EventArgs e)
        {
            profile = null;
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
            notiPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            reload();
            this.clearPickerUpload();
        }

        private async void notiBtn_Click(object sender, EventArgs e)
        {
            profile = null;
            homePnl.Visible = false;
            notiBtn.Checked = true;

            reload();
            this.clearPickerUpload();

            searchBtn.Checked = false;
            homeBtn.Checked = false;
            msgBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
            notiPnl.Visible = true;
            postBtn.Checked = false;
            addPostPannel.Visible = false;
            await DisplayRandomUsers();
            await DisplayNotifications();
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            profile = null;
            homePnl.Visible = false;
            notiBtn.Checked = false;
            homeBtn.Checked = false;

            reload();
            this.clearPickerUpload();

            searchBtn.Checked = true;
            searchPnl.Visible = true;
            msgBtn.Checked = false;
            msgPnl.Visible = false;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
            notiPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;
        }

        private void allBtn_Click(object sender, EventArgs e)
        {
            postBtnSearch.Checked = false;
            allBtn.Checked = true;
            recentBtn.Checked = false;
            userBtn.Checked = false;
            all2Pnl.Visible = true;
            sort2Pnl.Visible = true;
            postSortPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;
            userSortPnl.Visible = false;

            //DisplayAll();
        }

        private void userBtn_Click_1(object sender, EventArgs e)
        {

            postBtnSearch.Checked = false;
            allBtn.Checked = false;
            recentBtn.Checked = false;
            userBtn.Checked = true;
            all2Pnl.Visible = false;
            sort2Pnl.Visible = false;
            postSortPnl.Visible = false;
            userSortPnl.Visible = true;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            // lọc the người dùng
            //DisplayUsers();
        }

        private void recentBtn_Click_1(object sender, EventArgs e)
        {
            postBtnSearch.Checked = false;
            allBtn.Checked = false;
            recentBtn.Checked = true;
            userBtn.Checked = false;
            all2Pnl.Visible = false;
            sort2Pnl.Visible = false;
            postSortPnl.Visible = true;
            userSortPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;
        }

        private void postBtnSearch_Click(object sender, EventArgs e)
        {
            postBtnSearch.Checked = true;
            allBtn.Checked = false;
            recentBtn.Checked = false;
            userBtn.Checked = false;
            all2Pnl.Visible = false;
            sort2Pnl.Visible = false;
            postSortPnl.Visible = true;
            userSortPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            // hàm lọc theo bài viết
            DisplayPosts();
        }

        private void msgBtn_Click(object sender, EventArgs e)
        {
            profile = null;
            homeBtn.Checked = false;

            reload();
            this.clearPickerUpload();

            homePnl.Visible = false;
            msgBtn.Checked = true;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = true;
            profileBtn.Checked = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
            notiPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            LoadMessageList();
        }

        private async void followerBtn_Click(object sender, EventArgs e)
        {
            string username = GetProfileUsername();

            // Fetch current user
            Data currentUser = await GetCurrentUserInfo();

            if (username == currentUser.username)
            {
                // Show followers panel for current user's own profile
                followPnl.Visible = true;
                followPnl.Parent.Controls.SetChildIndex(followPnl, 0);
                await DisplayFollowData("Follower", "Người theo dõi");
            }
            else
            {
                // Fetch account type
                FirebaseResponse accountTypeResponse = await client.GetAsync($"Settings/{username}/AccountType");
                string accountType = accountTypeResponse.Body.Trim('"');

                // Check if current user follows the profile being viewed
                FirebaseResponse followResponse = await client.GetAsync($"Follow/{currentUser.username}/Following/{username}");
                bool isFollowing = followResponse.Body != "null" && followResponse.Body != "null\n";

                if (accountType == "private" && !isFollowing)
                {
                    // Show restricted view message for private accounts
                    ShowPrivateAccountMessageInFollowPanel();
                }
                else
                {
                    followPnl.Visible = true;
                    followPnl.Parent.Controls.SetChildIndex(followPnl, 0);
                    await DisplayFollowData("Follower", "Người theo dõi");
                }
            }
        }

        private async void followingBtn_Click(object sender, EventArgs e)
        {
            string username = GetProfileUsername();

            // Fetch current user
            Data currentUser = await GetCurrentUserInfo();

            if (username == currentUser.username)
            {
                // Show following panel for current user's own profile
                followPnl.Visible = true;
                followPnl.Parent.Controls.SetChildIndex(followPnl, 0);
                await DisplayFollowData("Following", "Đang theo dõi");
            }
            else
            {
                // Fetch account type
                FirebaseResponse accountTypeResponse = await client.GetAsync($"Settings/{username}/AccountType");
                string accountType = accountTypeResponse.Body.Trim('"');

                // Check if current user follows the profile being viewed
                FirebaseResponse followResponse = await client.GetAsync($"Follow/{currentUser.username}/Following/{username}");
                bool isFollowing = followResponse.Body != "null" && followResponse.Body != "null\n";

                if (accountType == "private" && !isFollowing)
                {
                    // Show restricted view message for private accounts
                    ShowPrivateAccountMessageInFollowPanel();
                }
                else
                {
                    followPnl.Visible = true;
                    followPnl.Parent.Controls.SetChildIndex(followPnl, 0);
                    await DisplayFollowData("Following", "Đang theo dõi");
                }
            }
        }


        private void ShowPrivateAccountMessageInFollowPanel()
        {
            followPnl.Controls.Clear();
            Label privateMessage = new Label
            {
                Text = "Đây là tài khoản riêng tư, theo dõi để xem",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            followPnl.Controls.Add(privateMessage);
        }

        private async void profileBtn_Click(object sender, EventArgs e)
        {
            profile = null;
            followProfileBtn.Visible = false;
            homeBtn.Checked = false;

            reload();
            LoadPostByUser();
            this.clearPickerUpload();

            homePnl.Visible = false;
            profileBtn.Checked = true;
            msgBtn.Checked = false;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profilePnl.Visible = true;
            settingPnl.Visible = false;
            settingBtn.Checked = false;
            notiPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            // Fetch follower and following counts and update button texts
            await UpdateFollowerFollowingCounts();
        }
        // end of main buttons

        private async Task UpdateFollowerFollowingCounts()
        {
            try
            {
                // Fetch the current user
                Data currentUser = await GetCurrentUserInfo();

                // Fetch the follower list
                FirebaseResponse followerResponse = await client.GetAsync($"Follow/{currentUser.username}/Follower");
                var followerDict = followerResponse.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followerResponse.Body) :
                    new Dictionary<string, FollowInfo>();
                int followerCount = followerDict.Count;

                // Fetch the following list
                FirebaseResponse followingResponse = await client.GetAsync($"Follow/{currentUser.username}/Following");
                var followingDict = followingResponse.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followingResponse.Body) :
                    new Dictionary<string, FollowInfo>();
                int followingCount = followingDict.Count;

                // Update the button texts
                followerBtn.Text = $"{followerCount} người theo dõi";
                followingBtn.Text = $"{followingCount} đang theo dõi";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating follower and following counts: {ex.Message}");
                followerBtn.Text = "0 người theo dõi";
                followingBtn.Text = "0 đang theo dõi";
            }
        }

        private async Task DeleteComment(string postId, string keyId)
        {
            await client.DeleteAsync($"comments/{postId}/{keyId}");
        }

        private async Task DeleteCommentByPost(string postId)
        {
            await client.DeleteAsync($"comments/{postId}");
        }


        // handle post in profile
        // handle post in profile
        // handle post in profile
        private async Task DeletePost(string postId)
        {
            await client.DeleteAsync($"posts/{postId}");
        }

        private async void LoadPostByUser()
        {
            try
            {
                this.loadingPostForUser.Visible = true;
                this.flowLayoutLoadPostImages.Visible = false;

                List<Post> posts = await this.getPosts();
                posts = posts.FindAll(post => post.user.Equals(Username))
                             .OrderByDescending(post => DateTime.ParseExact(post.created_at, "dd/MM/yyyy HH:mm:ss", null))
                             .ToList();

                int totalPost = posts.Count;
                totalPostOfUser.Text = $"{totalPost} Bài viết";

                if (flowLayoutLoadPostImages.Controls.Count > 0)
                {
                    flowLayoutLoadPostImages.Controls.Clear();
                }

                foreach (Post post in posts)
                {
                    Panel panel = new Panel
                    {
                        Width = 300,
                        Height = 300,
                        Margin = new Padding(5),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    PictureBox pictureBox = new PictureBox
                    {
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Width = 300,
                        Height = 300
                    };

                    pictureBox.Click += new EventHandler(async (object sender, EventArgs e) =>
                    {
                        this.panelDetailPost.Visible = true;
                        profileBtn.Checked = false;
                        profilePnl.Visible = false;
                        await LoadComments(post.id.ToString());
                    });

                    await LoadImageHttp.LoadImageAsync(post.imageUrl, pictureBox);

                    Button deleteButton = new Button
                    {
                        Text = "X",
                        ForeColor = Color.Red,
                        BackColor = Color.Transparent,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(30, 30),
                        Location = new Point(270, 0),
                        Cursor = System.Windows.Forms.Cursors.Hand
                    };

                    deleteButton.FlatAppearance.BorderSize = 0;
                    deleteButton.BringToFront();
                    deleteButton.Click += async (sender, e) =>
                    {
                        var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bài viết `{post.caption}` này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            await this.DeletePost(post.id.ToString());
                            await this.DeleteCommentByPost(post.id.ToString());

                            MessageBox.Show("Xóa bài viết và comment thành công!");
                            this._posts.Clear();

                            totalPost -= 1;
                            totalPostOfUser.Text = $"{totalPost} Bài viết";

                            flowLayoutLoadPostImages.Controls.Remove(panel);
                        }
                    };

                    panel.Controls.Add(pictureBox);
                    panel.Controls.Add(deleteButton);
                    deleteButton.BringToFront();

                    flowLayoutLoadPostImages.Controls.Add(panel);
                }
            }
            finally
            {
                this.loadingPostForUser.Visible = false;
                this.flowLayoutLoadPostImages.Visible = true;
            }
        }
        // end of handle post in profile
        // end of handle post in profile
        // end of handle post in profile

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

        private async void trangthaitaikhoan_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
            panel7.Visible = false;

            // Update the removed content textbox
            await UpdateRemovedContentTextBox();
        }

        private async Task UpdateRemovedContentTextBox()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"Settings/RemovedPosts");
                List<string> removedPosts = response.ResultAs<List<string>>();

                if (removedPosts == null || removedPosts.Count == 0)
                {
                    removedContent.Text = "Không có bài viết nào bị xóa";
                }
                else
                {
                    removedContent.Text = string.Join(Environment.NewLine, removedPosts);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving removed posts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                removedContent.Text = "Không có bài viết nào bị xóa";
            }
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

        private async void settingBtn_Click(object sender, EventArgs e)
        {
            profile = null;
            homeBtn.Checked = false;

            reload();
            this.clearPickerUpload();
            await LoadBlockedUsers();

            homePnl.Visible = false;
            profileBtn.Checked = false;
            settingBtn.Checked = true;
            msgBtn.Checked = false;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profilePnl.Visible = false;
            settingPnl.Visible = true;
            notiPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            //bam cai dat
            panel7.Visible = true;
            guna2Panel9.Visible = false;
            guna2Panel10.Visible = false;
            guna2Panel13.Visible = false;
            guna2Panel12.Visible = false;
            panel3.Visible = false;

            // Check and print removed posts
            await CheckAndPrintRemovedPosts();

            // Retrieve and set the account type
            string accountType = await GetAccountType(Username);
            privateAccountBtn.Checked = (accountType == "private");

            string notificationSetting = await GetNotificationStatus(Username);
            notifSettingBtn.Checked = (notificationSetting == "on");
        }
        private async Task CheckAndPrintRemovedPosts()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"Settings/RemovedPosts");
                List<string> removedPosts = response.ResultAs<List<string>>();

                if (removedPosts == null)
                {
                    // If RemovedPosts doesn't exist, create an empty array
                    removedPosts = new List<string>();
                    await client.SetAsync($"Settings/RemovedPosts", removedPosts);
                }
                else
                {
                    // Print out post IDs
                    MessageBox.Show("Removed Posts IDs: " + string.Join(", ", removedPosts), "Removed Posts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving removed posts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            //pnlForYou.Visible = true;
            //pnlFollowing.Visible = false;
        }

        private void btnFollowing_Click(object sender, EventArgs e)
        {
            btnFollowing.Checked = true;
            btnForYou.Checked = false;
            //pnlFollowing.Visible = true;
            //pnlForYou.Visible = false;
        }

        private void proBtn_Click(object sender, EventArgs e)
        {
            if (btnLogout.Visible == false)
            {
                btnLogout.Visible = true;
            }
            else
            {
                btnLogout.Visible = false;
            }
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Clear Google credentials or any stored tokens
                string credPath = "token.json";
                if (Directory.Exists(credPath))
                {
                    Directory.Delete(credPath, true);
                }

                // Close current form and show the login form
                this.Hide();
                Login login = new Login();
                login.Closed += (s, args) => this.Close();
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to log out. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editPro_Click(object sender, EventArgs e)
        {
            settingBtn_Click(sender, e);
        }

        private async Task<string> UploadAvatarAsync(string imagePath)
        {
            try
            {
                // Generate a unique identifier for the user's avatar
                string userId = usrName.Text;

                // Upload image to Firebase Storage
                var task = new FirebaseStorage("iconnect-nt106.appspot.com")
                    .Child("avatars")
                    .Child(userId + ".jpg")
                    .PutAsync(File.OpenRead(imagePath));

                // Wait for the upload to complete
                var downloadUrl = await task;

                // Return the download URL
                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading avatar: {ex.Message}");
                return null;
            }
        }

        private async Task<string> UploadPostAsync(string imagePath)
        {
            try
            {
                // Generate a unique identifier for the user's avatar
                Guid imageId = Guid.NewGuid();
                string fileExtension = Path.GetExtension(imagePath);

                // Upload image to Firebase Storage
                var task = new FirebaseStorage("iconnect-nt106.appspot.com")
                    .Child("posts")
                    .Child(imageId.ToString() + fileExtension)
                    .PutAsync(File.OpenRead(imagePath));

                // Wait for the upload to complete
                var downloadUrl = await task;

                // Return the download URL
                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading post: {ex.Message}");
                return null;
            }
        }

        private async Task UpdateAvatarAsync(string username, string imagePath)
        {
            try
            {
                // Upload the avatar and get the download URL
                string avatarUrl = await UploadAvatarAsync(imagePath);

                // Retrieve the existing user data from the database
                FirebaseResponse response = await client.GetAsync("Users/" + username);

                // Check if the response is not null and contains data
                if (response != null && response.Body != "null")
                {
                    Data existingUserData = response.ResultAs<Data>();

                    // Update only the avatar URL in the existing user data
                    existingUserData.AvatarUrl = avatarUrl;

                    // Perform a partial update to only update the avatar URL
                    FirebaseResponse updateResponse = await client.UpdateAsync("Users/" + username, existingUserData);

                    // Check if the update was successful
                    if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Avatar updated successfully.");
                        await LoadAvatarAsync(username);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update avatar.");
                    }
                }
                else
                {
                    MessageBox.Show("User data not found or null.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating avatar: {ex.Message}");
            }
        }

        private async void uploadAvtBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg; *.jpeg; *.png";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = dialog.FileName;

                // Pass the username and image path to UpdateAvatarAsync
                await UpdateAvatarAsync(usrName.Text, imagePath);
            }
        }

        private void avatarPro_Click(object sender, EventArgs e)
        {
            settingBtn_Click(sender, e);
        }

        private async void saveUsrInfoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Fetch new information from UI components
                string bio = bioTxtChange.Text;
                string name = changeFullName.Text;
                string dateofb = changeBDay.Text;
                string country = countryTxt.Text;
                string city = currentCity.Text;
                string gender = genderCmb.Text;
                string relationship = relaCmb.Text;
                string newUsername = changeUsrname.Text;

                // Check if the new username is different from the current one
                if (newUsername != Username)
                {
                    // Check if the new username already exists
                    FirebaseResponse newUserCheckResponse = await client.GetAsync("Users/" + newUsername);
                    if (newUserCheckResponse != null && newUserCheckResponse.Body != "null")
                    {
                        MessageBox.Show("Username already exists. Please choose a different username.");
                        return;
                    }
                }

                // Fetch existing data from Firebase
                FirebaseResponse response = await client.GetAsync("Users/" + Username);

                if (response != null && response.Body != "null")
                {
                    // Parse existing data
                    Data existingData = response.ResultAs<Data>();

                    // Update fields with new data
                    existingData.bio = bio;
                    existingData.name = name;
                    existingData.dateofb = dateofb;
                    existingData.country = country;
                    existingData.city = city;
                    existingData.gender = gender;
                    existingData.relationship = relationship;

                    // Only update the username if it's different
                    if (newUsername != Username)
                    {
                        existingData.username = newUsername;
                    }

                    // Retain other fields such as AvatarUrl and email
                    string avatarUrl = existingData.AvatarUrl;
                    string email = existingData.email;
                    string password = existingData.password;

                    // Create new data entry with updated key
                    Data newData = new Data
                    {
                        bio = existingData.bio,
                        name = existingData.name,
                        dateofb = existingData.dateofb,
                        country = existingData.country,
                        city = existingData.city,
                        gender = existingData.gender,
                        relationship = existingData.relationship,
                        username = existingData.username,
                        AvatarUrl = avatarUrl,
                        email = email,
                        password = password
                    };

                    SetResponse setResponse = await client.SetAsync("Users/" + newUsername, newData);

                    if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Update the global Username variable only if the new data entry is successful
                        if (newUsername != Username)
                        {
                            // Delete old data entry
                            FirebaseResponse deleteResponse = await client.DeleteAsync("Users/" + Username);

                            if (deleteResponse.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                MessageBox.Show("Could not delete old user information.");
                                return;
                            }

                            Username = newUsername; // Update the global variable after deletion
                        }

                        usrName.Text = newUsername; // Update the username in the UI
                        MessageBox.Show("User information updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Could not update user information.");
                    }
                }
                else
                {
                    MessageBox.Show("Could not find user information.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user information: {ex.Message}");
            }
        }

        private void postBtn_Click(object sender, EventArgs e)
        {
            profile = null;
            reload();
            postBtn.Checked = true;
            addPostPannel.Visible = true;
            homePnl.Visible = false;
            homeBtn.Checked = false;
            profileBtn.Checked = false;
            settingBtn.Checked = false;
            msgBtn.Checked = false;
            notiBtn.Checked = false;
            searchBtn.Checked = false;
            searchPnl.Visible = false;
            msgPnl.Visible = false;
            profilePnl.Visible = false;
            settingPnl.Visible = false;
            notiPnl.Visible = false;

            //bam cai dat
            panel7.Visible = false;
            guna2Panel9.Visible = false;
            guna2Panel10.Visible = false;
            guna2Panel13.Visible = false;
            guna2Panel12.Visible = false;
            panel3.Visible = false;
        }

        private void panelUploadImagePost_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Chọn ảnh bạn muốn upload.";
            dialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg; *.jpeg; *.png";

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filename = dialog.FileName;
                int x = 200;
                int y = 140;

                PictureBox pictureBox = new PictureBox();
                Image image = Image.FromFile(filename);

                pictureBox.Image = image;
                pictureBox.Size = new Size(x, y);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                this.clearPickerUpload();
                this.pathImageUpload = filename;
                this.panelImagesReview.Controls.Add(pictureBox);
            }
        }

        private void clearPickerUpload()
        {
            if (this.panelImagesReview.Controls.Count > 0)
            {
                this.panelImagesReview.Controls.Clear();
            }
        }

        private string getDateNow()
        {
            DateTime now = DateTime.Now;
            return now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private async void btnSubmitAddPost_Click(object sender, EventArgs e)
        {
            string captionPost = txbCaptionAddPost.Text;
            string imagePath = this.pathImageUpload;

            if (string.IsNullOrEmpty(captionPost) || string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Vui lòng nhập nội dung và chọn ảnh!");
                return;
            }

            string imageUrl = await this.UploadPostAsync(imagePath);

            PostDto post = new PostDto();
            string formattedDate = this.getDateNow();

            post.user = this.Username;
            post.imageUrl = imageUrl;
            post.caption = captionPost;
            post.likes = "[]";
            post.comments = "[]";
            post.created_at = formattedDate;

            this._posts.Add(await mapperPost(post));

            MessageBox.Show("Thêm bài viết thành công.");
            this.pathImageUpload = String.Empty;
            txbCaptionAddPost.Text = String.Empty;

            postBtn.Checked = false;
            addPostPannel.Visible = false;
            homePnl.Visible = true;
            homeBtn.Checked = true;

            SetResponse response = await client.SetAsync($"/posts/{post.id}", post);

            if (response != null)
            {
                this.clearPickerUpload();
                this.reload();
                return;
            }
        }

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

                // Fetch current user
                Data currentUser = await GetCurrentUserInfo();

                foreach (var item in jsonPosts)
                {
                    // Fetch account type of the post creator
                    FirebaseResponse accountTypeResponse = await client.GetAsync($"Settings/{item.Value.user}/AccountType");
                    string accountType = accountTypeResponse.Body.Trim('"');

                    // Check if the current user follows the profile being viewed
                    FirebaseResponse followResponse = await client.GetAsync($"Follow/{currentUser.username}/Following/{item.Value.user}");
                    bool isFollowing = followResponse.Body != "null" && followResponse.Body != "null\n";

                    // Check if the post author is blocked or has blocked the current user
                    bool isBlocked = await IsUserBlocked(currentUser.username, item.Value.user);
                    if (isBlocked)
                    {
                        continue; // Skip this post if the user is blocked
                    }

                    // Always add posts from the current user
                    if (item.Value.user == currentUser.username)
                    {
                        Post post = await this.mapperPost(item.Value);
                        posts.Add(post);
                        continue;
                    }

                    // Only add posts from public profiles or private profiles followed by the current user
                    if (accountType == "public" || (accountType == "private" && isFollowing))
                    {
                        Post post = await this.mapperPost(item.Value);
                        posts.Add(post);
                    }
                }
            }

            return posts.OrderBy(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
        }




        private async void LoadPost()
        {
            if (this._posts.Count > 0)
            {
                return;
            }

            List<Post> posts = await this.getPosts();
            this._posts = posts;
            this.RenderPosts(posts);
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

        private async Task<Data> GetUserByUsername(string user)
        {
            FirebaseResponse response = await client.GetAsync("Users/" + user);
            Data result = response.ResultAs<Data>();
            return result;
        }

        string profile;
        private void RenderPosts(List<Post> posts)
        {
            if (posts.Count < 0)
            {
                return;
            }

            if (this.panelRenderPost.Controls.Count > 0)
            {
                this.panelRenderPost.Controls.Clear();
            }

            bool isLoading = false;

            foreach (Post post in posts)
            {
                Console.WriteLine(post.id);
                isLoading = true;

                UC_Post panelPost = new UC_Post();

                panelPost.Name = $"{post.id}";
                panelPost.Dock = DockStyle.Top;
                panelPost.LabelCountComment = $"{post.comments.Count}";
                panelPost.LabelCountLike = $"{post.likes.Count}";
                panelPost.AuthorName = $"{post.userData.name}";
                panelPost.LoadPostPicture(post.imageUrl);
                panelPost.PostCaption = post.caption;
                panelPost.PostCreatedAt = this.FormatFacebookTime(post.created_at);
                panelPost.ButtonImageLike = !post.likes.Contains(Username) ? global::iConnect.Properties.Resources.heart : global::iConnect.Properties.Resources.redheart;

                if (!string.IsNullOrEmpty(post.userData.AvatarUrl))
                {
                    panelPost.LoadAuthorAvatar(post.userData.AvatarUrl);
                }

                panelPost.ButtonCommentClick = new System.EventHandler((object sender, EventArgs e) =>
                {
                    this.handleClickBtnComment(post.id.ToString());
                });

                panelPost.ButtonLikeClick = new System.EventHandler((object sender, EventArgs e) =>
                {
                    this.handleClickBtnLike(post.id.ToString(), panelPost);
                });

                // Add event handler for avatar click
                panelPost.AvatarClick += (s, e) =>
                {
                    LoadUserProfile(post.userData.username);
                    profile = post.userData.username;
                };

                // Add event handler for username click
                panelPost.UsernameClick += (s, e) =>
                {
                    LoadUserProfile(post.userData.username);
                    profile = post.userData.username;
                };

                this.panelRenderPost.Controls.Add(panelPost);

                isLoading = false;
            }

            if (!isLoading)
            {
                this.panelLoadingPost.Visible = false;
                this.panelRenderPost.Visible = true;
            }
        }

        private string FormatFacebookTime(string input)
        {
            DateTime inputDateTime;
            if (!DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out inputDateTime))
            {
                return "Invalid Date";
            }

            DateTime now = DateTime.Now;
            TimeSpan timeDifference = now - inputDateTime;

            if (timeDifference.TotalSeconds < 60)
            {
                return "Vừa xong";
            }
            if (timeDifference.TotalMinutes < 60)
            {
                return $"{timeDifference.Minutes} phút trước";
            }
            if (timeDifference.TotalHours < 24)
            {
                return $"{timeDifference.Hours} giờ trước";
            }
            if (timeDifference.TotalDays < 2)
            {
                return "Hôm qua";
            }
            if (timeDifference.TotalDays < 7)
            {
                return $"{timeDifference.Days} ngày trước";
            }
            if (timeDifference.TotalDays < 30)
            {
                int weeks = (int)(timeDifference.TotalDays / 7);
                return $"{weeks} tuần trước";
            }
            if (timeDifference.TotalDays < 365)
            {
                int months = (int)(timeDifference.TotalDays / 30);
                return $"{months} tháng trước";
            }

            int years = (int)(timeDifference.TotalDays / 365);
            return $"{years} năm trước";
        }

        private async void handleClickBtnComment(string postId)
        {

            if (this.panelLoadComments.Controls.Count > 0)
            {
                this.panelLoadComments.Controls.Clear();
            }

            await this.LoadComments(postId);
        }

        private async Task LoadComments(string postId)
        {
            try
            {
                if (this.panelLoadComments.Controls.Count > 0)
                {
                    this.panelLoadComments.Controls.Clear();
                }

                this.panelLoadComments.Visible = false;
                this.panelDetailPost.Visible = true;
                this.homePnl.Visible = false;
                this.panelLoadingPostDetail.Visible = true;

                Post post = await this.getPostById(postId);
                string user = this.Username;
                this.selectedPostId = postId;

                List<Comment> comments = await this.getCommentByPostId(postId);

                this.labelAuthorDetail.Text = post.userData?.name ?? "Deleted User";
                await LoadImageHttp.LoadImageAsync(post.imageUrl, this.picturePostDetail);
                this.labelPostCaptionDetail.Text = post.caption;
                this.labelCreatedAt.Text = this.FormatFacebookTime(post.created_at);

                foreach (var item in comments)
                {
                    UC_Comment comment = new UC_Comment();

                    comment.Dock = DockStyle.Top;
                    comment.AuthorText = item.userData?.name ?? "Deleted User";
                    comment.CommentText = item.comment;
                    comment.CreatedAtText = this.FormatFacebookTime(item.created_at);
                    comment.Padding = new Padding(0, 0, 0, 10);
                    comment.ShowDelete = item.user.Equals(Username);

                    comment.ButtonDeleteClick = new System.EventHandler((object sender, EventArgs e) =>
                    {
                        this.handleDeleteComment(item.postId, item.key, item.id.ToString(), item.children);
                    });

                    comment.ButtonReplyClick = new System.EventHandler((object sender, EventArgs e) =>
                    {
                        this.handleReplayComment(item.id.ToString(), item.user);
                    });

                    string avatarUrl = item.userData?.AvatarUrl;
                    if (!string.IsNullOrEmpty(avatarUrl))
                    {
                        comment.LoadAvatarAuthor(avatarUrl);
                    }
                    else
                    {
                        // Load default avatar from resources
                        comment.LoadAvatarAuthor(Properties.Resources.profile);
                    }

                    if (item.children.Count > 0)
                    {
                        foreach (var child in item.children)
                        {
                            UC_Comment commentChild = new UC_Comment();

                            Padding padding = new Padding(40, 0, 0, 10);

                            commentChild.Dock = DockStyle.Top;
                            commentChild.Padding = padding;
                            commentChild.AuthorText = child.userData?.name ?? "Deleted User";
                            commentChild.CommentText = child.comment;
                            commentChild.CreatedAtText = this.FormatFacebookTime(child.created_at);
                            commentChild.ShowDelete = child.user.Equals(Username);

                            commentChild.ButtonDeleteClick = new System.EventHandler((object sender, EventArgs e) =>
                            {
                                this.handleDeleteComment(child.postId, child.key, child.id.ToString(), new List<Comment>());
                            });

                            commentChild.ButtonReplyClick = new System.EventHandler((object sender, EventArgs e) =>
                            {
                                this.handleReplayComment(item.id.ToString(), child.user);
                            });

                            string childAvatarUrl = child.userData?.AvatarUrl;
                            if (!string.IsNullOrEmpty(childAvatarUrl))
                            {
                                commentChild.LoadAvatarAuthor(childAvatarUrl);
                            }
                            else
                            {
                                // Load default avatar from resources
                                commentChild.LoadAvatarAuthor(Properties.Resources.profile);
                            }

                            this.panelLoadComments.Controls.Add(commentChild);
                        }
                    }

                    this.panelLoadComments.Controls.Add(comment);
                }
            }
            finally
            {
                this.panelLoadComments.Visible = true;
                this.panelLoadingPostDetail.Visible = false;
            }
        }


        private async void handleDeleteComment(string postId, string key, string commentId, List<Comment> child)
        {
            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bình luận này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Post post = await this.getPostById(postId);
                await this.DeleteComment(postId, key);

                post.comments.Remove(commentId);

                if (child.Count > 0)
                {
                    foreach (Comment cmt in child)
                    {
                        await this.DeleteComment(cmt.postId, cmt.key);
                        post.comments.Remove(cmt.id.ToString());
                    }
                }

                await client.UpdateAsync($"posts/{postId}", this.mapperToPostDto(post));
                MessageBox.Show("Xóa bình luận thành công!");
                await this.LoadComments(postId);
                this._posts.Clear();
            }
        }

        private void handleReplayComment(string commentId, string user)
        {
            this.labelCommentReplay.Text = $"@{user}";
            this.selectedCommentId = commentId;
            this.btnCancelReply.Visible = true;
        }

        private async void handleClickBtnLike(string postId, UC_Post panel)
        {
            Post post = await this.getPostById(postId);
            int foundPostIndex = this._posts.FindIndex((t) => t.id.Equals(new Guid(postId)));
            string user = this.Username;
            bool isLike = true;

            if (post.likes.Contains(user))
            {
                post.likes.Remove(user);
                this._posts[foundPostIndex].likes.Remove(user);
                isLike = false;
            }
            else
            {
                post.likes.Add(user);
                this._posts[foundPostIndex].likes.Add(user);
                // Create notification
                await CreateNotification(this.Username, post.user, "Like", $"{this.Username} đã thích bài viết của bạn.");
            }

            int countLikeComponent = this._posts[foundPostIndex].likes.Count;

            panel.LabelCountLike = $"{countLikeComponent}";
            panel.ButtonImageLike = !isLike ? global::iConnect.Properties.Resources.heart : global::iConnect.Properties.Resources.redheart;

            FirebaseResponse updateResponse = await client.UpdateAsync("/posts/" + postId, mapperToPostDto(post));

            // Check if the update was successful
            if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
        }

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

        private async Task<List<Comment>> getCommentByPostId(string postId)
        {
            // Retrieve the user data from the database
            FirebaseResponse response = await client.GetAsync($"/comments/{postId}");

            if (response.Body == "null")
            {
                return new List<Comment>();
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, Comment>>(response.Body);

            foreach (var kvp in data)
            {
                Data user = await this.GetUserByUsername(kvp.Value.user);
                kvp.Value.userData = user;
                kvp.Value.key = kvp.Key;
            }

            var comments = data.Values.OrderBy(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

            return this.OrganizeComments(comments);
        }

        private async void btnSubmitAddComment_Click(object sender, EventArgs e)
        {
            string comment = txbComment.Text;
            string user = this.Username;
            string postId = this.selectedPostId;
            string parentId = this.selectedCommentId;
            int foundPostIndex = this._posts.FindIndex((t) => t.id.Equals(new Guid(postId)));

            // validate
            if (string.IsNullOrEmpty(postId))
            {
                MessageBox.Show("Không tìm thấy id của bài viết!");
                return;
            }

            if (string.IsNullOrEmpty(comment))
            {
                MessageBox.Show("Vui lòng nhập nội dung của bạn!");
                return;
            }

            // insert comment
            Comment newComment = new Comment();

            newComment.user = user;
            newComment.comment = !string.IsNullOrEmpty(parentId) ? $"{this.labelCommentReplay.Text} {comment}" : comment;
            newComment.postId = postId;
            newComment.created_at = this.getDateNow();
            newComment.parent_id = string.IsNullOrEmpty(parentId) ? "" : parentId;

            PushResponse response = await this.client.PushAsync($"/comments/{newComment.postId}", newComment);

            // Update count comment in post
            Post post = await this.getPostById(postId);

            if (response != null)
            {
                MessageBox.Show("Đăng bình luận thành công!");
                txbComment.Text = "";
                if (!string.IsNullOrEmpty(parentId)) this.selectedCommentId = string.Empty;
                this.HiddenReply();
                // Create notification
                await CreateNotification(this.Username, post.user, "Comment", $"{this.Username} đã bình luận về bài viết.");
            }

            post.comments.Add(newComment.id.ToString());

            this._posts[foundPostIndex].comments.Add(newComment.id.ToString());

            await this.client.UpdateAsync($"/posts/{postId}", this.mapperToPostDto(post));

            await this.LoadComments(postId);
        }

        private List<Comment> OrganizeComments(List<Comment> comments)
        {
            var commentDict = comments.ToDictionary(c => c.id.ToString());

            var rootComments = new List<Comment>();

            foreach (var comment in comments)
            {
                if (string.IsNullOrEmpty(comment.parent_id))
                {
                    rootComments.Add(comment);
                }
                else if (commentDict.TryGetValue(comment.parent_id, out var parentComment))
                {
                    Console.WriteLine($"parentComment.comment:::${parentComment.comment}");
                    Console.WriteLine($"children:::${comment.comment}");
                    parentComment.children.Add(comment);
                }
            }

            return rootComments;
        }


        private void HiddenReply()
        {
            this.selectedCommentId = string.Empty;
            this.labelCommentReplay.Text = "";
            this.btnCancelReply.Visible = false;
        }

        private void btnCancelReply_Click(object sender, EventArgs e)
        {
            this.HiddenReply();
        }

        //private void SearchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        PerformSearch();
        //        e.Handled = true; // Đánh dấu sự kiện đã được xử lý
        //    }
        //}

        

        private void profilePnl_Paint(object sender, PaintEventArgs e)
        {

        }

        // report start
        // report start
        // report start
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private void addfileBtn_Click(object sender, EventArgs e)
        {
            // Prompt user to select a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Check the file size
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 10 * 1024 * 1024)
                {
                    MessageBox.Show("File size exceeds 10MB. Please select a smaller file.");
                    return;
                }

                // Display selected file name in the UI
                filenameLbl.Text = Path.GetFileName(filePath);
            }
            else
            {
                MessageBox.Show("No file selected.");
            }
        }

        private async void reportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string fileUrl = null;
                string filePath = null;

                // Check if a file is selected
                if (!string.IsNullOrEmpty(filenameLbl.Text))
                {
                    filePath = openFileDialog.FileName; // Get the file path
                                                        // Upload the selected file to Firebase Storage
                    fileUrl = await UploadFileToStorageAsync(filePath);
                }

                // Get the report details from UI
                string helpReportDetail = helpreportDetail.Text;

                // Get the username (you might want to replace this with actual user info)
                string username = Username; // Replace with actual username retrieval logic

                // Submit the report to Firebase Realtime Database
                await SubmitReportAsync(helpReportDetail, fileUrl, username);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting report: {ex.Message}");
            }
        }

        private async Task<string> UploadFileToStorageAsync(string filePath)
        {
            try
            {
                // Generate a unique identifier for the file
                string fileName = Path.GetFileName(filePath);

                // Upload the file to Firebase Storage
                var task = new FirebaseStorage("iconnect-nt106.appspot.com")
                    .Child("reports")
                    .Child(fileName)
                    .PutAsync(File.OpenRead(filePath));

                // Wait for the upload to complete
                var downloadUrl = await task;

                // Return the download URL
                return downloadUrl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading file: {ex.Message}");
                return null;
            }
        }

        private async Task SubmitReportAsync(string helpReportDetail, string fileUrl, string username)
        {
            try
            {
                // Generate a unique identifier for the report
                string reportId = Guid.NewGuid().ToString();

                // Create a new report object
                Report report = new Report
                {
                    Id = reportId,
                    HelpReportDetail = helpReportDetail,
                    FileUrl = fileUrl,
                    Username = username,
                    CreatedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Status = "Pending"
                };

                // Submit the report to Firebase Realtime Database
                SetResponse response = await client.SetAsync("Reports/" + reportId, report);

                // Check if the report was submitted successfully
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Report submitted successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to submit report.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting report: {ex.Message}");
            }
        }
        public class Report
        {
            public string Id { get; set; }
            public string HelpReportDetail { get; set; }
            public string FileUrl { get; set; }
            public string Username { get; set; }
            public string Status { get; set; }
            public string CreatedAt { get; set; }
        }
        // report end
        // report end
        // report end


        // block start
        // block start
        // block start
        private void AddUserToSearchResults(Data user)
        {
            Guna.UI2.WinForms.Guna2Panel userPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = searchblockPnl.Width - 40,
                Height = UserPanelHeight, // Use the constant height
                Tag = user
            };

            Guna.UI2.WinForms.Guna2PictureBox avatar = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Width = 50,
                Height = 50,
                Left = 5,
                Top = 5,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadius = 25
            };

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                avatar.ImageLocation = user.AvatarUrl;
            }
            else
            {
                avatar.Image = Properties.Resources.profile; // Ensure you have a default avatar image in your resources
            }

            Guna.UI2.WinForms.Guna2HtmlLabel nameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.name,
                Left = 60,
                Top = 5,
                Width = 200,
                Font = new Font("Segoe UI", 10)
            };

            Guna.UI2.WinForms.Guna2HtmlLabel usernameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "@" + user.username,
                Left = 60,
                Top = 25,
                Width = 200,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };

            Guna.UI2.WinForms.Guna2Button blockButton = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Block",
                Left = searchblockPnl.Width - 140,
                Width = 100,
                AutoRoundedCorners = true,
                FillColor = Color.MediumAquamarine,
                Tag = user
            };
            blockButton.Click += BlockButton_Click;

            userPanel.Controls.Add(avatar);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(usernameLabel);
            userPanel.Controls.Add(blockButton);

            searchblockPnl.Controls.Add(userPanel);

            // Reposition all user panels in searchblockPnl
            for (int i = 0; i < searchblockPnl.Controls.Count; i++)
            {
                if (searchblockPnl.Controls[i] is Guna.UI2.WinForms.Guna2Panel panel)
                {
                    SetPanelLocation(panel, i);
                }
            }
        }
        private async void BlockButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button blockButton = sender as Guna.UI2.WinForms.Guna2Button;
            Data userToBlock = blockButton.Tag as Data;

            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            if (currentUser.username == userToBlock.username)
            {
                MessageBox.Show("You cannot block yourself.");
                return;
            }

            Guna.UI2.WinForms.Guna2Panel userPanel = blockButton.Parent as Guna.UI2.WinForms.Guna2Panel;
            searchblockPnl.Controls.Remove(userPanel);
            blockedPnl.Controls.Add(userPanel);

            RepositionBlockedUsers();

            blockButton.Text = "Unblock";
            blockButton.Click -= BlockButton_Click;
            blockButton.Click += UnblockButton_Click;

            await BlockUser(currentUser, userToBlock);
        }
        private void RepositionBlockedUsers()
        {
            int yPosition = 0;
            foreach (Control control in blockedPnl.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Panel userPanel)
                {
                    SetPanelLocation(userPanel, yPosition);
                    yPosition++;
                }
            }
        }

        private async Task BlockUser(Data currentUser, Data userToBlock)
        {
            try
            {
                var blockRelationship = new
                {
                    BlockedBy = currentUser.username,
                    BlockedUser = userToBlock.username
                };

                await client.SetAsync($"BlockedUsers/{currentUser.username}/{userToBlock.username}", blockRelationship);
                MessageBox.Show($"User {userToBlock.username} has been blocked.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error blocking user: {ex.Message}");
            }
        }

        private async Task UnblockUser(Data currentUser, Data userToUnblock)
        {
            try
            {
                await client.DeleteAsync($"BlockedUsers/{currentUser.username}/{userToUnblock.username}");
                MessageBox.Show($"User {userToUnblock.username} has been unblocked.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unblocking user: {ex.Message}");
            }
        }

        private async void UnblockButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button unblockButton = sender as Guna.UI2.WinForms.Guna2Button;
            Data userToUnblock = unblockButton.Tag as Data;

            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            Guna.UI2.WinForms.Guna2Panel userPanel = unblockButton.Parent as Guna.UI2.WinForms.Guna2Panel;
            blockedPnl.Controls.Remove(userPanel);

            RepositionBlockedUsers();

            await UnblockUser(currentUser, userToUnblock);
        }

        private async Task<Data> GetCurrentUserInfo()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"Users/{Username}");
                Data currentUser = JsonConvert.DeserializeObject<Data>(response.Body);
                return currentUser;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving current user info: {ex.Message}");
                return null;
            }
        }

        private async void searchblockBtn_Click_1(object sender, EventArgs e)
        {
            string searchText = searchblockTxt.Text.ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a username to search.");
                return;
            }

            searchblockPnl.Controls.Clear();

            try
            {
                FirebaseResponse response = await client.GetAsync("Users");
                var usersDict = JsonConvert.DeserializeObject<Dictionary<string, Data>>(response.Body);

                Data currentUser = await GetCurrentUserInfo();
                FirebaseResponse blockedResponse = await client.GetAsync($"BlockedUsers/{currentUser.username}");
                var blockedUsersDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(blockedResponse.Body) ?? new Dictionary<string, object>();

                foreach (var user in usersDict.Values)
                {
                    if (user.username.ToLower().Contains(searchText) && !blockedUsersDict.ContainsKey(user.username) && user.username != currentUser.username)
                    {
                        AddUserToSearchResults(user);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching users: {ex.Message}");
            }
        }

        private async Task LoadBlockedUsers()
        {
            try
            {
                Data currentUser = await GetCurrentUserInfo();
                FirebaseResponse response = await client.GetAsync($"BlockedUsers/{currentUser.username}");
                var blockedUsersDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Body) ?? new Dictionary<string, object>();

                blockedPnl.Controls.Clear();

                foreach (var blockedUser in blockedUsersDict.Keys)
                {
                    FirebaseResponse userResponse = await client.GetAsync($"Users/{blockedUser}");
                    Data user = JsonConvert.DeserializeObject<Data>(userResponse.Body);
                    AddUserToBlockedPanel(user);
                }

                RepositionBlockedUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading blocked users: {ex.Message}");
            }
        }

        private void AddUserToBlockedPanel(Data user)
        {
            Guna.UI2.WinForms.Guna2Panel userPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = blockedPnl.Width - 40,
                Height = UserPanelHeight, // Use the constant height
                Tag = user
            };

            Guna.UI2.WinForms.Guna2PictureBox avatar = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Width = 50,
                Height = 50,
                Left = 5,
                Top = 5,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadius = 25
            };

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                avatar.ImageLocation = user.AvatarUrl;
            }
            else
            {
                avatar.Image = Properties.Resources.profile;
            }

            Guna.UI2.WinForms.Guna2HtmlLabel nameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.name,
                Left = 60,
                Top = 5,
                Width = 200
            };

            Guna.UI2.WinForms.Guna2HtmlLabel usernameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.username,
                Left = 60,
                Top = 25,
                Width = 200
            };

            Guna.UI2.WinForms.Guna2Button unblockButton = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Unblock",
                Left = blockedPnl.Width - 140,
                Width = 100,
                AutoRoundedCorners = true,
                FillColor = Color.MediumAquamarine,
                Tag = user
            };
            unblockButton.Click += UnblockButton_Click;

            userPanel.Controls.Add(avatar);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(usernameLabel);
            userPanel.Controls.Add(unblockButton);

            blockedPnl.Controls.Add(userPanel);
        }

        private const int UserPanelHeight = 60;
        private const int UserPanelMarginBottom = 10;
        private const int UserPanelTopMargin = 10;


        private void SetPanelLocation(Guna.UI2.WinForms.Guna2Panel panel, int index)
        {
            int topMargin = index == 0 ? UserPanelTopMargin : 0;
            panel.Location = new Point(20, index * (UserPanelHeight + UserPanelMarginBottom) + topMargin);
        }

        private async Task<bool> IsUserBlocked(string currentUser, string otherUser)
        {
            FirebaseResponse blockedByResponse = await client.GetAsync($"BlockedUsers/{currentUser}/{otherUser}");
            FirebaseResponse blockedUserResponse = await client.GetAsync($"BlockedUsers/{otherUser}/{currentUser}");

            bool isBlockedByCurrentUser = blockedByResponse.Body != "null" && blockedByResponse.Body != "null\n";
            bool isBlockedByOtherUser = blockedUserResponse.Body != "null" && blockedUserResponse.Body != "null\n";

            return isBlockedByCurrentUser || isBlockedByOtherUser;
        }

        // block end
        // block end
        // block end


        // settings start
        // settings start
        // settings start
        private async void privateAccountBtn_Click(object sender, EventArgs e)
        {
            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            string currentAccountType = await GetAccountType(currentUser.username);

            if (currentAccountType == null)
            {
                MessageBox.Show("Error: Could not retrieve account type.");
                return;
            }

            string newAccountType;

            if (privateAccountBtn.Checked)
            {
                privateAccountBtn.Checked = false;
                newAccountType = "public";
            }
            else
            {
                privateAccountBtn.Checked = true;
                newAccountType = "private";
            }

            await UpdateAccountType(currentUser.username, newAccountType);
        }
        private async Task<string> GetAccountType(string username)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"Settings/{username}/AccountType");
                string accountType = response.ResultAs<string>();

                if (string.IsNullOrEmpty(accountType))
                {
                    // If AccountType doesn't exist, create it with default value "public"
                    accountType = "public";
                    await client.SetAsync($"Settings/{username}/AccountType", accountType);
                }

                return accountType;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving account type: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "public"; // Return default value if there's an error
            }
        }

        private async Task UpdateAccountType(string username, string newAccountType)
        {
            try
            {
                await client.SetAsync($"Settings/{username}/AccountType", newAccountType);
                MessageBox.Show($"Account type has been updated to {newAccountType}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating account type: {ex.Message}");
            }
        }

        private string verificationCode;

        private async void sendCodeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Data currentUser = await GetCurrentUserInfo();

                if (currentUser == null)
                {
                    MessageBox.Show("Error: Could not retrieve current user info.");
                    return;
                }

                verificationCode = GenerateVerificationCode();
                SendVerificationCodeToEmail(currentUser.email, verificationCode);

                MessageBox.Show("Verification code sent to your email.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending verification code: {ex.Message}");
            }
        }

        private void SendVerificationCodeToEmail(string email, string confirmationCode)
        {
            // Email configuration
            string senderEmail = "kaitsukishi@gmail.com"; // Your email address
            string senderPassword = "psqhniyosuaxspqm"; // Your email password
            string smtpHost = "smtp.gmail.com"; // Your SMTP host
            int smtpPort = 587; // Your SMTP port (e.g., 587 for Gmail)

            // Email content
            string subject = "iConnect - Thay đổi mật khẩu";
            string body = $"Chào bạn,<br><br>" +
                            $"Bạn đã yêu cầu thay đổi mật khẩu tài khoản của mình.<br><br>" +
                            $"Đây là mã xác nhận của bạn: <b>{confirmationCode}</b>.<br>" +
                            $"Vui lòng sử dụng mã này để hoàn tất quá trình thay đổi mật khẩu.<br><br>" +
                            $"Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.<br><br>" +
                            $"Trân trọng,<br>Đội ngũ quản trị viên";

            // Create and configure the SMTP client
            SmtpClient client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true // Enable SSL for secure connection (e.g., for Gmail)
            };

            // Create the email message
            MailMessage mailMessage = new MailMessage(senderEmail, email, subject, body)
            {
                IsBodyHtml = true // Set to true if your email body contains HTML
            };

            // Send the email
            client.Send(mailMessage);
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private async void changePwdBtn_Click(object sender, EventArgs e)
        {
            string oldPassword = oldPwdTxt.Text;
            string newPassword = newPwdTxt.Text;
            string reEnterPassword = renewPwdTxt.Text;
            string enteredCode = verifyCodeTxt.Text;

            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            if (HashPassword(oldPassword) != currentUser.password)
            {
                MessageBox.Show("Old password is incorrect.");
                return;
            }

            if (!IsValidPassword(newPassword))
            {
                MessageBox.Show("New password does not meet the requirements.");
                return;
            }

            if (newPassword != reEnterPassword)
            {
                MessageBox.Show("Re-entered password does not match the new password.");
                return;
            }

            if (enteredCode != verificationCode)
            {
                MessageBox.Show("Verification code is incorrect.");
                return;
            }

            try
            {
                currentUser.password = HashPassword(newPassword);
                await UpdateUserPassword(currentUser);

                MessageBox.Show("Password changed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}");
            }
        }
        private bool IsValidPassword(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(ch => "!@#$%^&*()".Contains(ch))) return false;

            return true;
        }

        private async Task UpdateUserPassword(Data user)
        {
            try
            {
                await client.UpdateAsync($"Users/{user.username}", user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user password", ex);
            }
        }

        private string emailVerificationCode;
        private async void sendEmailCodeBtn_Click(object sender, EventArgs e)
        {
            string newEmail = newEmailTxt.Text;
            string emailPassword = emailpwdTxt.Text;

            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            if (string.IsNullOrEmpty(newEmail) || !IsValidEmail(newEmail))
            {
                MessageBox.Show("Please enter a valid new email.");
                return;
            }

            if (HashPassword(emailPassword) != currentUser.password)
            {
                MessageBox.Show("Password is incorrect.");
                return;
            }

            try
            {
                // Check if new email already exists in the database
                FirebaseResponse response = await client.GetAsync($"Users");
                var usersDict = JsonConvert.DeserializeObject<Dictionary<string, Data>>(response.Body);

                if (usersDict.Values.Any(user => user.email.ToLower() == newEmail.ToLower()))
                {
                    MessageBox.Show("This email is already associated with another account.");
                    return;
                }

                emailVerificationCode = GenerateVerificationCode();
                SendVerificationCodeToEmail(newEmail, emailVerificationCode);

                MessageBox.Show("Verification code sent to the new email.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending verification code: {ex.Message}");
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async void changeEmailBtn_Click(object sender, EventArgs e)
        {
            string newEmail = newEmailTxt.Text;
            string emailPassword = emailpwdTxt.Text;
            string enteredCode = emailVerifyCodeTxt.Text;

            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            if (string.IsNullOrEmpty(newEmail) || !IsValidEmail(newEmail))
            {
                MessageBox.Show("Please enter a valid new email.");
                return;
            }

            if (HashPassword(emailPassword) != currentUser.password)
            {
                MessageBox.Show("Password is incorrect.");
                return;
            }

            if (enteredCode != emailVerificationCode)
            {
                MessageBox.Show("Verification code is incorrect.");
                return;
            }

            try
            {
                currentUser.email = newEmail;
                await UpdateUserEmail(currentUser);

                MessageBox.Show("Email changed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing email: {ex.Message}");
            }
        }
        private async Task UpdateUserEmail(Data user)
        {
            try
            {
                await client.UpdateAsync($"Users/{user.username}", user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user email", ex);
            }
        }
        // settings end
        // settings end
        // settings end

        // Search
        private void SearchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Prevent the 'ding' sound on Enter
                SearchPostsByUsername(searchTxt.Text);
            }
        }

        private async Task SearchPostsByUsername(string username)
        {
            try
            {
                ClearSearchResultsForPost(); // Clear previous post search results
                List<Post> posts = await this.getPostsByUsername(username);
                List<Post> filteredPosts = new List<Post>();

                foreach (var post in posts)
                {
                    bool isBlocked = await IsUserBlocked(Username, post.user);
                    if (!isBlocked)
                    {
                        filteredPosts.Add(post);
                    }
                }

                if (filteredPosts.Count > 0)
                {
                    this.RenderPosts1(filteredPosts, this.sort2Pnl);
                }
                else
                {
                    Console.WriteLine("No posts found for the specified username.");
                    this.panelRenderPost.Controls.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching for posts: {ex.Message}");
            }
        }

        private async Task<List<Post>> getPostsByUsername(string username)
        {
            FirebaseResponse response = await client.GetAsync("/posts");

            List<Post> posts = new List<Post>();

            if (response.Body == "null")
            {
                return posts;
            }

            var jsonPosts = JsonConvert.DeserializeObject<Dictionary<string, PostDto>>(response.Body);

            foreach (var item in jsonPosts)
            {
                Post post = await this.mapperPost(item.Value);
                if (post.user.Equals(username, StringComparison.OrdinalIgnoreCase) || post.userData.name.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    posts.Add(post);
                }
            }

            return posts.OrderBy(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();

        }

        private void RenderPosts1(List<Post> posts, Panel targetPanel)
        {
            if (posts.Count == 0)
            {
                return;
            }

            targetPanel.Controls.Clear();

            foreach (Post post in posts)
            {
                Console.WriteLine(post.id);

                UC_Post panelPost = new UC_Post();

                panelPost.Name = $"{post.id}";
                panelPost.Dock = DockStyle.Top;
                panelPost.LabelCountComment = $"{post.comments.Count}";
                panelPost.LabelCountLike = $"{post.likes.Count}";
                panelPost.AuthorName = $"{post.userData.name}";
                panelPost.LoadPostPicture(post.imageUrl);
                panelPost.PostCaption = post.caption;
                panelPost.PostCreatedAt = this.FormatFacebookTime(post.created_at);
                panelPost.ButtonImageLike = !post.likes.Contains(Username) ? global::iConnect.Properties.Resources.heart : global::iConnect.Properties.Resources.redheart;

                if (!string.IsNullOrEmpty(post.userData.AvatarUrl))
                {
                    panelPost.LoadAuthorAvatar(post.userData.AvatarUrl);
                }

                panelPost.ButtonCommentClick = new System.EventHandler((object sender, EventArgs e) =>
                {
                    this.handleClickBtnComment(post.id.ToString());
                });

                panelPost.ButtonLikeClick = new System.EventHandler((object sender, EventArgs e) =>
                {
                    this.handleClickBtnLike(post.id.ToString(), panelPost);
                });

                // Add event handler for avatar click
                panelPost.AvatarClick += (s, e) =>
                {
                    LoadUserProfile(post.userData.username);
                    profile = post.userData.username;
                };

                // Add event handler for username click
                panelPost.UsernameClick += (s, e) =>
                {
                    LoadUserProfile(post.userData.username);
                    profile = post.userData.username;
                };

                targetPanel.Controls.Add(panelPost);
            }

            targetPanel.Visible = true;
        }

        private void ClearSearchResults()
        {
            if (this.sort2Pnl.Controls.Count > 0)
            {
                this.sort2Pnl.Controls.Clear();
            }
        }

        private void ClearSearchResultsForPost()
        {
            if (this.postSortPnl.Controls.Count > 0)
            {
                this.postSortPnl.Controls.Clear();
            }
        }

        private void ClearSearchResultsForUser()
        {
            if (this.all2Pnl.Controls.Count > 0)
            {
                this.all2Pnl.Controls.Clear();
            }
        }

        private void ClearSearchResultsForUser1()
        {
            if (this.userSortPnl.Controls.Count > 0)
            {
                this.userSortPnl.Controls.Clear();
            }
        }


        private void ClearAllSearchResults()
        {
            ClearSearchResults(); // Clear general search results
            ClearSearchResultsForPost(); // Clear post search results
            ClearSearchResultsForUser(); // Clear user search results
            ClearSearchResultsForUser1(); // Clear user search results for button 'nguoi dung'
        }

        private CancellationTokenSource _searchCts;

        private async void searchTxt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Cancel the previous search if it's still running
                _searchCts?.Cancel();

                // Create a new CancellationTokenSource for the current search
                _searchCts = new CancellationTokenSource();

                // Wait for 500 milliseconds to debounce the input
                await Task.Delay(500, _searchCts.Token);

                ClearAllSearchResults(); // Clear all previous search results

                string searchQuery = searchTxt.Text.Trim();

                // Log for debugging purposes
                Console.WriteLine($"Search query: {searchQuery}");

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    await DisplayUsers(searchQuery);
                    await DisplayUsers1(searchQuery);
                    await SearchPostsByUsername(searchQuery); // Search posts by username
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore this exception as it's expected when a new search is initiated before the delay finishes
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during search: {ex.Message}");
                Console.WriteLine($"An error occurred during search: {ex.Message}");
            }
        }

        // Filter for Post

        private async void DisplayPosts()
        {
            ClearSearchResultsForPost(); // Clear previous search results
            List<Post> posts = await this.getPostsByUsername(searchTxt.Text);
            RenderPosts1(posts, this.postSortPnl);
        }

        // Filter for User

        private async void renderUser(Data user, Guna.UI2.WinForms.Guna2Panel parentPanel, int topPosition)
        {
            Guna.UI2.WinForms.Guna2Panel userPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = parentPanel.Width - 20,
                Height = 60, // Adjusted height of the user panel
                Top = topPosition,
                Left = 5,
                Tag = user
            };

            Guna.UI2.WinForms.Guna2PictureBox avatar = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Width = 50,
                Height = 50,
                Left = 3,
                Top = 5,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadius = 25
            };

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                avatar.ImageLocation = user.AvatarUrl;
            }
            else
            {
                avatar.Image = Properties.Resources.profile; // Default profile image
            }

            avatar.Click += async (s, e) =>
            {
                await LoadUserProfile(user.username);
                profile = user.username;
            };

            Guna.UI2.WinForms.Guna2HtmlLabel nameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.name,
                Left = 60,
                Top = 10,
                Width = 200,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.Transparent
            };

            Guna.UI2.WinForms.Guna2HtmlLabel usernameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "@" + user.username,
                Left = 60,
                Top = 30,
                Width = 200,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                BackColor = Color.Transparent
            };

            userPanel.Controls.Add(avatar);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(usernameLabel);

            // Check if the current user is the same as the user being rendered
            if (user.username != Username)
            {
                Guna.UI2.WinForms.Guna2Button followButton = new Guna.UI2.WinForms.Guna2Button
                {
                    Width = 120,
                    Height = 40,
                    Left = userPanel.Width - 130,
                    Top = 15,
                    Font = new Font("Segoe UI", 10),
                    Tag = user
                };
                followButton.Click += FollowButton_Click;

                // Check if the current user is already following the selected user
                FirebaseResponse response = await client.GetAsync($"Follow/{Username}/Following/{user.username}");
                bool isFollowing = response.Body != "null";

                if (isFollowing)
                {
                    followButton.Text = "Bỏ theo dõi";
                    followButton.BorderColor = Color.MediumAquamarine;
                    followButton.FillColor = Color.Transparent;
                    followButton.ForeColor = Color.MediumAquamarine;
                    followButton.BorderThickness = 2;
                    followButton.AutoRoundedCorners = true;
                    followButton.Click -= FollowButton_Click;
                    followButton.Click += UnfollowButton_Click;
                }
                else
                {
                    followButton.Text = "Theo dõi";
                    followButton.BorderColor = Color.Transparent;
                    followButton.FillColor = Color.MediumAquamarine;
                    followButton.ForeColor = Color.White;
                    followButton.AutoRoundedCorners = true;
                    followButton.BorderThickness = 0;
                }

                userPanel.Controls.Add(followButton);
            }

            parentPanel.Controls.Add(userPanel);
        }

        private async Task DisplayUsers(string searchQuery = "")
        {
            try
            {
                // Fetch all users
                FirebaseResponse response = await client.GetAsync("Users");
                if (response.Body == "null")
                {
                    MessageBox.Show("No users found.");
                    return;
                }

                var usersDict = JsonConvert.DeserializeObject<Dictionary<string, Data>>(response.Body);
                var usersList = usersDict?.Values?.ToList() ?? new List<Data>(); // Convert to list or initialize an empty list

                // Clear existing controls in the main panel
                this.all2Pnl.Controls.Clear();

                // Convert search query to lowercase for case-insensitive comparison
                searchQuery = searchQuery.ToLower();

                // Fetch the current user
                Data currentUser = await GetCurrentUserInfo();

                // Filter users based on search query and blocked status
                var filteredUsers = new List<Data>();

                foreach (var user in usersList)
                {
                    bool isBlocked = await IsUserBlocked(currentUser.username, user.username);
                    if ((user.name?.ToLower()?.Contains(searchQuery) == true || user.username?.ToLower()?.Contains(searchQuery) == true) && !isBlocked)
                    {
                        filteredUsers.Add(user);
                    }
                }

                // Display filtered users
                int topPosition = 20; // Initial top position
                int verticalSpacing = 70; // Spacing between users

                foreach (var user in filteredUsers)
                {
                    renderUser(user, this.all2Pnl, topPosition);
                    topPosition += verticalSpacing; // Increment top position for next user
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying users: {ex.Message}");
            }
        }


        private async Task DisplayUsers1(string searchQuery = "")
        {
            try
            {
                // Fetch all users
                FirebaseResponse response = await client.GetAsync("Users");
                if (response.Body == "null")
                {
                    MessageBox.Show("No users found.");
                    return;
                }

                var usersDict = JsonConvert.DeserializeObject<Dictionary<string, Data>>(response.Body);
                var usersList = usersDict?.Values?.ToList() ?? new List<Data>(); // Convert to list or initialize an empty list

                // Clear existing controls in the main panel
                this.all2Pnl.Controls.Clear();

                // Convert search query to lowercase for case-insensitive comparison
                searchQuery = searchQuery.ToLower();

                // Fetch the current user
                Data currentUser = await GetCurrentUserInfo();

                // Filter users based on search query and blocked status
                var filteredUsers = new List<Data>();

                foreach (var user in usersList)
                {
                    bool isBlocked = await IsUserBlocked(currentUser.username, user.username);
                    if ((user.name?.ToLower()?.Contains(searchQuery) == true || user.username?.ToLower()?.Contains(searchQuery) == true) && !isBlocked)
                    {
                        filteredUsers.Add(user);
                    }
                }
                // Display filtered users
                int topPosition = 20; // Initial top position
                int verticalSpacing = 70; // Spacing between users

                foreach (var user in filteredUsers)
                {
                    renderUser(user, this.userSortPnl, topPosition);
                    topPosition += verticalSpacing; // Increment top position for next user
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying users: {ex.Message}");
            }
        }


        // End search

        // start of account settings
        // start of account settings
        // start of account settings
        private async void delAccountBtn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.",
                                             "Confirm Account Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Assuming the username is stored in a variable called Username
                    string username = Username;

                    // Delete user data
                    await client.DeleteAsync($"Users/{username}");

                    // Delete user posts
                    var userPosts = await getPostsByUsername(username);
                    foreach (var post in userPosts)
                    {
                        await client.DeleteAsync($"posts/{post.id}");
                    }

                    // Delete user comments
                    var userComments = await getCmtByUsername(username);
                    foreach (var comment in userComments)
                    {
                        await client.DeleteAsync($"comments/{comment.postId}/{comment.id}");
                    }

                    // Delete user from blocked lists and their own blocked list
                    await DeleteUserFromBlockedLists(username);
                    await DeleteUserOwnBlockedList(username);

                    // Delete user's settings
                    await client.DeleteAsync($"Settings/{username}");

                    MessageBox.Show("Account deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to login screen or perform logout
                    btnLogout.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting account: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Helper class to deserialize blocked user data
        public class Block
        {
            public string BlockedBy { get; set; }
            public string BlockedUser { get; set; }
        }


        private async Task<List<Comment>> getCmtByUsername(string username)
        {
            FirebaseResponse response = await client.GetAsync("/comments");

            List<Comment> comments = new List<Comment>();

            if (response.Body == "null")
            {
                return comments;
            }

            var jsonComments = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Comment>>>(response.Body);

            foreach (var postComments in jsonComments)
            {
                foreach (var item in postComments.Value)
                {
                    if (item.Value.user.Equals(username, StringComparison.OrdinalIgnoreCase))
                    {
                        comments.Add(item.Value);
                    }
                    if (item.Value.children != null && item.Value.children.Count > 0)
                    {
                        comments.AddRange(GetNestedComments(item.Value.children, username));
                    }
                }
            }

            // Log the collected comments
            Debug.WriteLine($"Total comments found: {comments.Count}");
            foreach (var comment in comments)
            {
                Debug.WriteLine($"Comment ID: {comment.id}, User: {comment.user}, Post ID: {comment.postId}, Parent ID: {comment.parent_id}");
            }

            return comments.OrderBy(c => DateTime.ParseExact(c.created_at, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).ToList();
        }

        private List<Comment> GetNestedComments(List<Comment> comments, string username)
        {
            List<Comment> userComments = new List<Comment>();
            foreach (var comment in comments)
            {
                if (comment.user.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    userComments.Add(comment);
                }
                if (comment.children != null && comment.children.Count > 0)
                {
                    userComments.AddRange(GetNestedComments(comment.children, username));
                }
            }
            return userComments;
        }

        private async Task DeleteUserFromBlockedLists(string username)
        {
            var blockedUsersResponse = await client.GetAsync("BlockedUsers");
            if (blockedUsersResponse.Body != "null")
            {
                var blockedUsers = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Block>>>(blockedUsersResponse.Body);
                if (blockedUsers != null)
                {
                    foreach (var blockedUser in blockedUsers)
                    {
                        if (blockedUser.Value != null && blockedUser.Value.ContainsKey(username))
                        {
                            await client.DeleteAsync($"BlockedUsers/{blockedUser.Key}/{username}");
                        }
                    }
                }
            }
        }

        private async Task DeleteUserOwnBlockedList(string username)
        {
            var userBlockedListResponse = await client.GetAsync($"BlockedUsers/{username}");
            if (userBlockedListResponse.Body != "null")
            {
                await client.DeleteAsync($"BlockedUsers/{username}");
            }
        }


        private async void lockAccountBtn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to lock your account?",
                             "Confirm Account Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Assuming the username is stored in a variable called Username
                    string settingsPath = $"Settings/{Username}";

                    // Set the account status to locked
                    await client.SetAsync($"{settingsPath}/AccountStatus", "locked");

                    MessageBox.Show("Account locked successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to login screen or perform logout
                    btnLogout.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error locking account: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // noti setting
        private async void notifSettingBtn_Click(object sender, EventArgs e)
        {
            Data currentUser = await GetCurrentUserInfo();

            if (currentUser == null)
            {
                MessageBox.Show("Error: Could not retrieve current user info.");
                return;
            }

            string currentStatus = await GetNotificationStatus(currentUser.username);

            if (currentStatus == null)
            {
                MessageBox.Show("Error: Could not retrieve notification status.");
                return;
            }

            string newStatus;

            if (notifSettingBtn.Checked)
            {
                notifSettingBtn.Checked = false;
                newStatus = "off";
            }
            else
            {
                notifSettingBtn.Checked = true;
                newStatus = "on";
            }

            await UpdateNotificationStatus(currentUser.username, newStatus);
        }

        private async Task<string> GetNotificationStatus(string username)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync($"Settings/{username}/NotificationStatus");
                string status = response.ResultAs<string>();

                if (string.IsNullOrEmpty(status))
                {
                    // If notification status doesn't exist, create it with default value "off"
                    status = "off";
                    await client.SetAsync($"Settings/{username}/NotificationStatus", status);
                }

                if (status == "on")
                {
                    notiStatusLbl.Text = "Tắt thông báo";
                }
                else
                {
                    notiStatusLbl.Text = "Bật thông báo";
                }

                return status;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving notification status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "off"; // Return default value if there's an error
            }
        }

        private async Task UpdateNotificationStatus(string username, string newStatus)
        {
            try
            {
                await client.SetAsync($"Settings/{username}/NotificationStatus", newStatus);
                if (newStatus == "on")
                {
                    notiStatusLbl.Text = "Tắt thông báo";
                }
                else
                {
                    notiStatusLbl.Text = "Bật thông báo";
                }
                MessageBox.Show($"Notification has been updated to {newStatus}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating notification status: {ex.Message}");
            }
        }

        // account settings end
        // account settings end
        // account settings end


        // start of recommendation and folllow
        // start of recommendation and follow
        // start of recommendation and follow
        public class FollowInfo
        {
            public DateTime followed_at { get; set; }
        }

        private async Task DisplayRandomUsers()
        {
            try
            {
                // Fetch all users
                FirebaseResponse response = await client.GetAsync("Users");
                var usersDict = JsonConvert.DeserializeObject<Dictionary<string, Data>>(response.Body);

                // Fetch the current user
                Data currentUser = await GetCurrentUserInfo();

                // Fetch the current user's following list
                FirebaseResponse followingResponse = await client.GetAsync($"Follow/{currentUser.username}/Following");
                var followingDict = followingResponse.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followingResponse.Body) :
                    new Dictionary<string, FollowInfo>();

                // Exclude the current user and those already followed
                var otherUsers = new List<Data>();

                foreach (var user in usersDict.Values)
                {
                    bool isBlocked = await IsUserBlocked(currentUser.username, user.username);
                    if (user.username != currentUser.username && !followingDict.ContainsKey(user.username) && !isBlocked)
                    {
                        otherUsers.Add(user);
                    }
                }

                // Select three random users
                Random random = new Random();
                var randomUsers = otherUsers.OrderBy(x => random.Next()).Take(3).ToList();

                // Clear existing controls in recommendation panels
                recUserPnl.Controls.Clear();
                recUser2Pnl.Controls.Clear();
                recUser3Pnl.Controls.Clear();

                // Add the "Có thể bạn sẽ biết" text
                AddRecommendationLabel();

                // Display the random users
                int topPosition = 40; // Initial top position
                int verticalSpacing = 70; // Adjusted spacing between users

                foreach (var user in randomUsers)
                {
                    AddUserToTrendPanel(user, topPosition, recUserPnl);
                    AddUserToTrendPanel(user, topPosition, recUser2Pnl);
                    AddUserToTrendPanel(user, topPosition, recUser3Pnl);
                    topPosition += verticalSpacing; // Increment top position for next user
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying random users: {ex.Message}");
            }
        }


        private void AddRecommendationLabel()
        {
            Guna.UI2.WinForms.Guna2HtmlLabel recommendationLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "Có thể bạn sẽ biết",
                Top = 6,
                Width = recUserPnl.Width,
                Left = 50,
                Height = 30, // Adjust as necessary
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent,
            };
            Guna.UI2.WinForms.Guna2HtmlLabel recommendationLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "Có thể bạn sẽ biết",
                Top = 6,
                Width = recUserPnl.Width,
                Left = 50,
                Height = 30, // Adjust as necessary
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent,
            };
            Guna.UI2.WinForms.Guna2HtmlLabel recommendationLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "Có thể bạn sẽ biết",
                Top = 6,
                Width = recUserPnl.Width,
                Left = 50,
                Height = 30, // Adjust as necessary
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent,
            };

            recUserPnl.Controls.Add(recommendationLabel);
            recUser2Pnl.Controls.Add(recommendationLabel2);
            recUser3Pnl.Controls.Add(recommendationLabel3);
        }


        private async void AddUserToTrendPanel(Data user, int topPosition, Guna.UI2.WinForms.Guna2Panel parentPanel)
        {
            Guna.UI2.WinForms.Guna2Panel userPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = parentPanel.Width - 20,
                Height = 60, // Adjusted height of the user panel
                Top = topPosition,
                Left = 5,
                Tag = user
            };

            Guna.UI2.WinForms.Guna2PictureBox avatar = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Width = 50,
                Height = 50,
                Left = 3,
                Top = 10,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadius = 25
            };

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                avatar.ImageLocation = user.AvatarUrl;
            }
            else
            {
                avatar.Image = Properties.Resources.profile;
            }

            Guna.UI2.WinForms.Guna2HtmlLabel nameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.name,
                Left = 55,
                Top = 10,
                Width = 200,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.Transparent
            };

            Guna.UI2.WinForms.Guna2HtmlLabel usernameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "@" + user.username,
                Left = 55,
                Top = 30,
                Width = 200,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                BackColor = Color.Transparent
            };

            Guna.UI2.WinForms.Guna2Button followButton = new Guna.UI2.WinForms.Guna2Button
            {
                Left = parentPanel.Width - 120,
                Top = 15,
                Width = 100,
                AutoRoundedCorners = true,
                FillColor = Color.MediumAquamarine,
                Tag = user,
                Font = new Font("Segoe UI", 9)
            };
            followButton.Click += FollowButton_Click;

            // Check if the current user is already following the selected user
            FirebaseResponse response = await client.GetAsync($"Follow/{Username}/Following/{user.username}");
            bool isFollowing = response.Body != "null";

            if (isFollowing)
            {
                followButton.Text = "Bỏ theo dõi";
                followButton.BorderColor = Color.MediumAquamarine;
                followButton.FillColor = Color.Transparent;
                followButton.ForeColor = Color.MediumAquamarine;
                followButton.BorderThickness = 2;
                followButton.AutoRoundedCorners = true;
                followButton.Click -= FollowButton_Click;
                followButton.Click += UnfollowButton_Click;
            }
            else
            {
                followButton.Text = "Theo dõi";
                followButton.BorderColor = Color.Transparent;
                followButton.FillColor = Color.MediumAquamarine;
                followButton.ForeColor = Color.White;
                followButton.AutoRoundedCorners = true;
                followButton.BorderThickness = 0;
            }

            userPanel.Controls.Add(followButton);
            avatar.Click += (s, e) =>
            {
                LoadUserProfile(user.username);
                profile = user.username;
            };

            userPanel.Controls.Add(avatar);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(usernameLabel);
            userPanel.Controls.Add(followButton);

            parentPanel.Controls.Add(userPanel);
        }

        private async void FollowButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button followButton = sender as Guna.UI2.WinForms.Guna2Button;
            Data userToFollow = followButton.Tag as Data;

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();

            // Check if the current user is already following the selected user
            FirebaseResponse response = await client.GetAsync($"Follow/{currentUser.username}/Following/{userToFollow.username}");
            bool isFollowing = response.Body != "null";

            if (!isFollowing)
            {
                // Follow the user with a timestamp
                var followData = new { followed_at = DateTime.UtcNow };
                await client.SetAsync($"Follow/{currentUser.username}/Following/{userToFollow.username}", followData);
                await client.SetAsync($"Follow/{userToFollow.username}/Follower/{currentUser.username}", followData);

                // Create notification
                await CreateNotification(currentUser.username, userToFollow.username, "Follow", $"{currentUser.username} đã bắt đầu theo dõi bạn.");

                // Update button to "Unfollow"
                followButton.Text = "Bỏ theo dõi";
                followButton.BorderColor = Color.MediumAquamarine;
                followButton.FillColor = Color.Transparent;
                followButton.ForeColor = Color.MediumAquamarine;
                followButton.BorderThickness = 2;
                followButton.Click -= FollowButton_Click;
                followButton.Click += UnfollowButton_Click;
            }
            else
            {
                // Unfollow the user
                await UnfollowUser(currentUser, userToFollow, followButton);
            }
        }

        private async void UnfollowButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button followButton = sender as Guna.UI2.WinForms.Guna2Button;
            Data userToFollow = followButton.Tag as Data;

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();

            // Unfollow the user
            await UnfollowUser(currentUser, userToFollow, followButton);
        }

        private async Task UnfollowUser(Data currentUser, Data userToUnfollow, Guna.UI2.WinForms.Guna2Button followButton)
        {
            // Unfollow the user with a timestamp
            var unfollowData = new { unfollowed_at = DateTime.UtcNow };
            await client.DeleteAsync($"Follow/{currentUser.username}/Following/{userToUnfollow.username}");
            await client.DeleteAsync($"Follow/{userToUnfollow.username}/Follower/{currentUser.username}");

            // Optionally, you can log the unfollow action with a timestamp
            await client.SetAsync($"UnfollowLog/{currentUser.username}/{userToUnfollow.username}", unfollowData);
            await client.SetAsync($"UnfollowLog/{userToUnfollow.username}/{currentUser.username}", unfollowData);

            // Update button to "Theo dõi"
            followButton.Text = "Theo dõi";
            followButton.BorderColor = Color.Transparent;
            followButton.FillColor = Color.MediumAquamarine;
            followButton.ForeColor = Color.White;
            followButton.BorderThickness = 0;
            followButton.Click -= UnfollowButton_Click;
            followButton.Click += FollowButton_Click;
        }

        private void exitFollowBtn_Click(object sender, EventArgs e)
        {
            followPnl.Visible = false;
        }

        private async Task DisplayFollowData(string followType, string title)
        {
            try
            {
                // Clear existing controls in followPnl
                followPnl.Controls.Clear();

                // Add the title
                AddFollowTitle(title);

                // Fetch the current profile user (not the current user)
                Data profileUser = await GetProfileUserInfo(); // Adjust this method to get the user of the profile being viewed

                // Fetch the follow data for the profile user
                FirebaseResponse followResponse = await client.GetAsync($"Follow/{profileUser.username}/{followType}");
                var followDict = followResponse.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followResponse.Body) :
                    new Dictionary<string, FollowInfo>();

                // Display user information
                int topPosition = 70; // Initial top position
                int verticalSpacing = 70; // Spacing between users

                foreach (var follow in followDict)
                {
                    string username = follow.Key;
                    FirebaseResponse userResponse = await client.GetAsync($"Users/{username}");
                    Data user = JsonConvert.DeserializeObject<Data>(userResponse.Body);

                    await AddUserToFollowPanel(user, topPosition, followType, profileUser); // Pass profileUser instead of currentUser
                    topPosition += verticalSpacing; // Increment top position for next user
                }

                // Re-add the exitFollowBtn at the end
                followPnl.Controls.Add(exitFollowBtn);

                // Adjust the position of exitFollowBtn if needed
                exitFollowBtn.Top = 0;
                exitFollowBtn.Left = (followPnl.Width - exitFollowBtn.Width);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying {followType.ToLower()}: {ex.Message}");
            }
        }

        private async Task<Data> GetProfileUserInfo()
        {
            // Implement the logic to get the user information of the profile being viewed
            // This could be based on a stored username or some other identifier
            string profileUsername = GetProfileUsername(); // Example method to get profile username
            FirebaseResponse response = await client.GetAsync($"Users/{profileUsername}");
            Data profileUser = JsonConvert.DeserializeObject<Data>(response.Body);
            return profileUser;
        }

        private string GetProfileUsername()
        {
            string currentProfileUsername = profile; // Implement the logic to get the profile username
            if (string.IsNullOrEmpty(currentProfileUsername))
            {
                return Username; // Return the current user's username if no profile username is set
            }
            return currentProfileUsername; // This should be a variable holding the current profile username
        }


        private void AddFollowTitle(string title)
        {
            Guna.UI2.WinForms.Guna2HtmlLabel titleLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = title,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Top = 30,
                Left = (followPnl.Width - 170) / 2,
                TextAlignment = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            followPnl.Controls.Add(titleLabel);

            Guna.UI2.WinForms.Guna2Panel linePanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Height = 1,
                Width = (int)(followPnl.Width * 0.8),
                Top = titleLabel.Bottom + 10,
                Left = (followPnl.Width - (int)(followPnl.Width * 0.8)) / 2,
                BackColor = Color.Black,
                BorderRadius = 1
            };

            followPnl.Controls.Add(linePanel);
        }

        private async Task AddUserToFollowPanel(Data user, int topPosition, string followType, Data currentUser)
        {
            if (user == null)
            {
                Console.WriteLine("User is null. Skipping user panel creation.");
                return;
            }

            if (currentUser == null)
            {
                Console.WriteLine("Current user is null. Skipping user panel creation.");
                return;
            }

            Guna.UI2.WinForms.Guna2Panel userPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = (int)(followPnl.Width * 0.8),
                Height = 60,
                Top = topPosition + 20,
                Left = (followPnl.Width - (int)(followPnl.Width * 0.8)) / 2,
                Tag = user
            };

            Guna.UI2.WinForms.Guna2PictureBox avatar = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Width = 50,
                Height = 50,
                Left = 3,
                Top = 5,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadius = 25
            };

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                avatar.ImageLocation = user.AvatarUrl;
            }
            else
            {
                avatar.Image = Properties.Resources.profile; // Default profile image
            }

            Guna.UI2.WinForms.Guna2HtmlLabel nameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.name,
                Left = 60,
                Top = 5,
                Width = 200,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            Guna.UI2.WinForms.Guna2HtmlLabel usernameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "@" + user.username,
                Left = 60,
                Top = 30,
                Width = 200,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                BackColor = Color.Transparent
            };

            avatar.Click += (s, e) =>
            {
                Console.WriteLine($"Avatar clicked for user: {user.username}");
                LoadUserProfile(user.username);
                profile = user.username;
            };

            userPanel.Controls.Add(avatar);
            userPanel.Controls.Add(nameLabel);
            userPanel.Controls.Add(usernameLabel);

            if (followType == "Follower")
            {
                if (currentUser.username != Username)
                {
                    if (user.username != Username)
                    {
                        Guna.UI2.WinForms.Guna2Button followButton = new Guna.UI2.WinForms.Guna2Button
                        {
                            Width = 120,
                            Height = 40,
                            Left = userPanel.Width - 130,
                            Top = 15,
                            Font = new Font("Segoe UI", 10),
                            Tag = user
                        };
                        followButton.Click += FollowButton_Click;

                        // Check if the current user is already following the selected user
                        FirebaseResponse response = await client.GetAsync($"Follow/{Username}/Following/{user.username}");
                        bool isFollowing = response.Body != "null";

                        if (isFollowing)
                        {
                            followButton.Text = "Bỏ theo dõi";
                            followButton.BorderColor = Color.MediumAquamarine;
                            followButton.FillColor = Color.Transparent;
                            followButton.ForeColor = Color.MediumAquamarine;
                            followButton.BorderThickness = 2;
                            followButton.AutoRoundedCorners = true;
                            followButton.Click -= FollowButton_Click;
                            followButton.Click += UnfollowButton_Click;
                        }
                        else
                        {
                            followButton.Text = "Theo dõi";
                            followButton.BorderColor = Color.Transparent;
                            followButton.FillColor = Color.MediumAquamarine;
                            followButton.ForeColor = Color.White;
                            followButton.AutoRoundedCorners = true;
                            followButton.BorderThickness = 0;
                        }

                        userPanel.Controls.Add(followButton);
                    }
                }
                else
                {
                    Guna.UI2.WinForms.Guna2Button removeButton = new Guna.UI2.WinForms.Guna2Button
                    {
                        Text = "Gỡ",
                        Width = 80,
                        Height = 40,
                        Left = userPanel.Width - 90,
                        Top = 15,
                        Font = new Font("Segoe UI", 10),
                        Tag = user
                    };
                    removeButton.FillColor = Color.Black;
                    removeButton.ForeColor = Color.White;
                    removeButton.AutoRoundedCorners = true;
                    removeButton.Click += RemoveFollowerButton_Click;
                    userPanel.Controls.Add(removeButton);
                }
            }
            else if (followType == "Following")
            {
                if (currentUser.username != Username)
                {
                    if (user.username != Username)
                    {
                        Guna.UI2.WinForms.Guna2Button followButton = new Guna.UI2.WinForms.Guna2Button
                        {
                            Width = 120,
                            Height = 40,
                            Left = userPanel.Width - 130,
                            Top = 15,
                            Font = new Font("Segoe UI", 10),
                            Tag = user
                        };
                        followButton.Click += FollowButton_Click;

                        // Check if the current user is already following the selected user
                        FirebaseResponse response = await client.GetAsync($"Follow/{Username}/Following/{user.username}");
                        bool isFollowing = response.Body != "null";

                        if (isFollowing)
                        {
                            followButton.Text = "Bỏ theo dõi";
                            followButton.BorderColor = Color.MediumAquamarine;
                            followButton.FillColor = Color.Transparent;
                            followButton.ForeColor = Color.MediumAquamarine;
                            followButton.BorderThickness = 2;
                            followButton.AutoRoundedCorners = true;
                            followButton.Click -= FollowButton_Click;
                            followButton.Click += UnfollowButton_Click;
                        }
                        else
                        {
                            followButton.Text = "Theo dõi";
                            followButton.BorderColor = Color.Transparent;
                            followButton.FillColor = Color.MediumAquamarine;
                            followButton.ForeColor = Color.White;
                            followButton.AutoRoundedCorners = true;
                            followButton.BorderThickness = 0;
                        }

                        userPanel.Controls.Add(followButton);
                    }
                }
                else
                {
                    Guna.UI2.WinForms.Guna2Button followButton = new Guna.UI2.WinForms.Guna2Button
                    {
                        Width = 120,
                        Height = 40,
                        Left = userPanel.Width - 130,
                        Top = 15,
                        Font = new Font("Segoe UI", 10),
                        Tag = user
                    };
                    followButton.Click += FollowButton_Click;

                    // Check if the current user is already following the selected user
                    FirebaseResponse response = await client.GetAsync($"Follow/{currentUser.username}/Following/{user.username}");
                    bool isFollowing = response.Body != "null";

                    if (isFollowing)
                    {
                        followButton.Text = "Bỏ theo dõi";
                        followButton.BorderColor = Color.MediumAquamarine;
                        followButton.FillColor = Color.Transparent;
                        followButton.ForeColor = Color.MediumAquamarine;
                        followButton.BorderThickness = 2;
                        followButton.AutoRoundedCorners = true;
                        followButton.Click -= FollowButton_Click;
                        followButton.Click += UnfollowButton_Click;
                    }
                    else
                    {
                        followButton.Text = "Theo dõi";
                        followButton.BorderColor = Color.Transparent;
                        followButton.FillColor = Color.MediumAquamarine;
                        followButton.ForeColor = Color.White;
                        followButton.AutoRoundedCorners = true;
                        followButton.BorderThickness = 0;
                    }

                    userPanel.Controls.Add(followButton);
                }
            }

            followPnl.Controls.Add(userPanel);
        }

        private async void RemoveFollowerButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button removeButton = sender as Guna.UI2.WinForms.Guna2Button;
            Data userToRemove = removeButton.Tag as Data;

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();

            // Remove the follower
            await client.DeleteAsync($"Follow/{currentUser.username}/Follower/{userToRemove.username}");
            await client.DeleteAsync($"Follow/{userToRemove.username}/Following/{currentUser.username}");

            // Optionally, you can log the remove action with a timestamp
            var removeData = new { removed_at = DateTime.UtcNow };
            await client.SetAsync($"RemoveLog/{currentUser.username}/{userToRemove.username}", removeData);
            await client.SetAsync($"RemoveLog/{userToRemove.username}/{currentUser.username}", removeData);

            // Remove the user panel from followPnl
            followPnl.Controls.Remove(removeButton.Parent);
        }

        // end of recommendation and follow
        // end of recommendation and follow
        // end of recommendation and follow

        // start of notification
        // start of notification
        // start of notification
        public class Notification
        {
            public string Sender { get; set; }
            public string Recipient { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
            public bool IsRead { get; set; } = false; // Default to false for new notifications
        }
        private async Task CreateNotification(string sender, string recipient, string type, string message)
        {
            Notification notification = new Notification
            {
                Sender = sender,
                Recipient = recipient,
                Type = type,
                Message = message,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await client.PushAsync($"Notifications/{recipient}", notification);
        }
        private async Task<Dictionary<string, Notification>> FetchNotifications(string username)
        {
            FirebaseResponse response = await client.GetAsync($"Notifications/{username}");
            if (response.Body != "null")
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Notification>>(response.Body);
            }
            return new Dictionary<string, Notification>();
        }
        private async Task DisplayNotifications()
        {
            try
            {
                // Show loading GIF
                loadingGif.Visible = true;
                loadingGif.BringToFront();

                // Fetch the current user
                Data currentUser = await GetCurrentUserInfo();

                // Fetch the user's notifications
                FirebaseResponse response = await client.GetAsync($"Notifications/{currentUser.username}");
                var notifications = response.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, Notification>>(response.Body) :
                    new Dictionary<string, Notification>();

                // Clear existing controls in loadNotiPnl
                loadNotiPnl.Controls.Clear();

                // Display notifications
                int topPosition = 10;
                int verticalSpacing = 60;

                foreach (var notification in notifications.Values.OrderByDescending(n => n.Timestamp))
                {
                    AddNotificationToPanel(notification, topPosition);
                    topPosition += verticalSpacing;
                }

                // Mark notifications as read
                foreach (var key in notifications.Keys)
                {
                    notifications[key].IsRead = true;
                    await client.SetAsync($"Notifications/{currentUser.username}/{key}", notifications[key]);
                }

                // Update notification button icon only if it is checked
                if (notiBtn.Checked)
                {
                    notiBtn.CustomImages.Image = Properties.Resources.Notification_fill; // Set to normal image
                    notiBtn.Invalidate(); // Force the button to redraw
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying notifications: {ex.Message}");
            }
            finally
            {
                // Hide loading GIF
                loadingGif.Visible = false;
            }
        }

        private async void AddNotificationToPanel(Notification notification, int topPosition)
        {
            Guna.UI2.WinForms.Guna2Panel notificationPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = loadNotiPnl.Width - 20,
                Height = 60,
                Top = topPosition,
                Left = 10,
                BorderRadius = 5
            };
            // Fetch the user who made the notification
            FirebaseResponse userResponse = await client.GetAsync($"Users/{notification.Sender}");
            Data user = JsonConvert.DeserializeObject<Data>(userResponse.Body);

            Guna.UI2.WinForms.Guna2PictureBox avatar = new Guna.UI2.WinForms.Guna2PictureBox
            {
                Width = 50,
                Height = 50,
                Left = 3,
                Top = 5,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadius = 25
            };

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                avatar.ImageLocation = user.AvatarUrl;
            }
            else
            {
                avatar.Image = Properties.Resources.profile; // Default profile image
            }

            Guna.UI2.WinForms.Guna2HtmlLabel messageLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = notification.Message,
                Left = 60,
                Top = 15,
                Width = loadNotiPnl.Width - 120,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.Transparent
            };

            Guna.UI2.WinForms.Guna2Panel separatorPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Width = loadNotiPnl.Width - 20,
                Height = 1,
                Top = notificationPanel.Bottom + 7,
                Left = 10,
                BackColor = Color.Gray,
                BorderRadius = 1
            };

            avatar.Click += (s, e) =>
            {
                LoadUserProfile(user.username);
                profile = user.username;
            };

            notificationPanel.Controls.Add(avatar);
            notificationPanel.Controls.Add(messageLabel);

            loadNotiPnl.Controls.Add(notificationPanel);
            loadNotiPnl.Controls.Add(separatorPanel);
        }

        private Image ResizeNotiImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        private async Task CheckForUnreadNotifications()
        {
            try
            {
                // Fetch the current user
                Data currentUser = await GetCurrentUserInfo();

                // Fetch the user's notifications
                FirebaseResponse response = await client.GetAsync($"Notifications/{currentUser.username}");
                var notifications = response.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, Notification>>(response.Body) :
                    new Dictionary<string, Notification>();

                // Check for unread notifications
                bool hasUnread = notifications.Values.Any(n => !n.IsRead);
                Image notificationIcon;
                if (notiBtn.Checked)
                {
                    notificationIcon = hasUnread ? Properties.Resources.notification_new : Properties.Resources.Notification_fill;
                }
                else
                {
                    notificationIcon = hasUnread ? Properties.Resources.notification_new : Properties.Resources.Notification;
                }
                // Resize the icon to the desired size, e.g., 32x32 pixels
                int desiredWidth = 32;
                int desiredHeight = 32;
                notificationIcon = ResizeImage(notificationIcon, desiredWidth, desiredHeight);

                notiBtn.CustomImages.Image = notificationIcon;
                notiBtn.Invalidate(); // Force the button to redraw
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking notifications: {ex.Message}");
            }
        }

        // Event handler for notiBtn CheckedChanged
        private async void notiBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (notiBtn.Checked)
            {
                notiBtn.CustomImages.Image = Properties.Resources.Notification_fill;
                notiBtn.Invalidate(); // Force the button to redraw
                await DisplayNotifications();
            }
            else
            {
                await CheckForUnreadNotifications();
            }
        }

        private async void deleteAllNoti_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete all notifications?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Fetch the current user
                    Data currentUser = await GetCurrentUserInfo();

                    // Delete all notifications for the user
                    await client.DeleteAsync($"Notifications/{currentUser.username}");

                    // Clear the notification panel
                    loadNotiPnl.Controls.Clear();

                    // Update notification button icon
                    notiBtn.CustomImages.Image = Properties.Resources.Notification; // Set to default image
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting notifications: {ex.Message}");
                }
            }
        }
        // end of notification
        // end of notification
        // end of notification


        //>> BEGIN OF MESSAGE

        private ChatService _chatService;
        private string _sender;
        private string _receiver;
        private string _chatID;
        private List<UC_Conversation> _allConversations = new List<UC_Conversation>();

        private async void LoadMessageList()
        {
            _chatService = new ChatService(client);

            MessageListPanel.Controls.Clear();
            _allConversations.Clear();

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();
            _sender = currentUser.username;

            // Check if the current user is already following the selected user
            FirebaseResponse followingResponse = await client.GetAsync($"Follow/{currentUser.username}/Following");
            var followingDict = followingResponse.Body != "null" ?
                    JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followingResponse.Body) :
                    new Dictionary<string, FollowInfo>();
            int followingCount = followingDict.Count;

            if (followingCount > 0)
            {
                UC_Conversation[] uc_Conversation = new UC_Conversation[followingCount];

                int i = 0;
                foreach (var key in followingDict.Keys)
                {
                    // Fetch user data for each following user
                    FirebaseResponse response = await client.GetAsync($"Users/{key}");
                    Data userData = JsonConvert.DeserializeObject<Data>(response.Body);

                    // Initialize the message item with the user data
                    uc_Conversation[i] = new UC_Conversation();
                    uc_Conversation[i].UserID = key;
                    uc_Conversation[i].Item_Name = userData.name;
                    string avatarURL = userData.AvatarUrl;
                    if (avatarURL != null)
                    {
                        uc_Conversation[i].Item_Avatar = await GetImageFromUrl(avatarURL);
                    }
                    else
                    {
                        uc_Conversation[i].Item_Avatar = Properties.Resources.profile;
                    }

                    // Add the message item to the panel
                    MessageListPanel.Controls.Add(uc_Conversation[i]);
                    _allConversations.Add(uc_Conversation[i]);

                    // Add the click event handler
                    uc_Conversation[i].Click += UC_Conversation_Click;

                    i++;
                }
            }
            else
            {
                Label label = new Label();
                label.Text = "Your following list is empty!";
                label.AutoSize = true;

                MessageListPanel.Controls.Add(label);
            }
        }

        private async Task<Image> GetImageFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        private async void UC_Conversation_Click(object sender, EventArgs e)
        {
            foreach (Control control in MessageListPanel.Controls)
            {
                if (control is UC_Conversation uc)
                {
                    uc.BackColor = Color.White;
                }
            }

            UC_Conversation clickedItem = sender as UC_Conversation;
            clickedItem.BackColor = Color.LightBlue;

            // Get User ID
            _receiver = clickedItem.UserID;
            _chatID = GenerateChatId(_sender, _receiver);

            ConvTitle.Text = clickedItem.Item_Name;
            ConvImage.Image = clickedItem.Item_Avatar;
            ConvTitle.Visible = true;
            ConvImage.Visible = true;
            FirstMessage.Visible = false;
            MessageText.Visible = true;
            SendButton.Visible = true;

            await InitializeChat(); // Load the chat for the selected receiver
        }

        private string GenerateChatId(string user1, string user2)
        {
            return string.Compare(user1, user2) < 0 ? $"{user1}_{user2}" : $"{user2}_{user1}";
        }

        private async Task InitializeChat()
        {
            try
            {
                // Clear previous messages
                ChatContent.Controls.Clear();

                var messages = await _chatService.ObserveMessages(_chatID);
                if (messages != null && messages.Count > 0)
                {
                    foreach (var message in messages)
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (message.Sender == _sender)
                            {
                                var messageItem = new UC_MessageSend
                                {
                                    Message = message.Text,
                                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(message.Timestamp).ToLocalTime().ToString("HH:mm")
                                };

                                ChatContent.Controls.Add(messageItem);
                                ChatContent.ScrollControlIntoView(messageItem);
                            }
                            else
                            {
                                var messageItem = new MessageReceive
                                {
                                    Message = message.Text,
                                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(message.Timestamp).ToLocalTime().ToString("HH:mm")
                                };

                                ChatContent.Controls.Add(messageItem);
                                ChatContent.ScrollControlIntoView(messageItem);
                            }
                        }));
                    }
                }
                else
                {
                    Console.WriteLine("No messages found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing chat: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string messageText = MessageText.Text;
                if (!string.IsNullOrEmpty(messageText))
                {
                    await _chatService.SendMessageAsync(_chatID, _sender, _receiver, messageText);
                    MessageText.Clear();
                    await InitializeChat(); // Refresh the chat to show the new message
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private void SearchConversations(string keyword)
        {
            MessageListPanel.Controls.Clear();

            var filteredConversations = _allConversations
                .Where(conv => conv.Item_Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (filteredConversations.Count > 0)
            {
                foreach (var conversation in filteredConversations)
                {
                    MessageListPanel.Controls.Add(conversation);
                }
            }
            else
            {
                Label label = new Label();
                label.Text = "No conversations found!";
                label.AutoSize = true;
                MessageListPanel.Controls.Add(label);
            }
        }


        private void MessageText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendButton_Click(sender, e);
            }
        }


        private void SearchConv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchConversations(SearchConv.Text);
            }
        }

        private void SearchConv_TextChanged(object sender, EventArgs e)
        {
            SearchConversations(SearchConv.Text);
        }


        // start of profile showing
        // start of profile showing
        // start of profile showing

        private async Task LoadUserProfile(string username)
        {
            try
            {
                profilePnl.Visible = true;

                // Fetch the user data from the database
                FirebaseResponse userResponse = await client.GetAsync($"Users/{username}");
                Data userData = userResponse.ResultAs<Data>();

                // Fetch the account type
                FirebaseResponse accountTypeResponse = await client.GetAsync($"Settings/{username}/AccountType");
                string accountType = accountTypeResponse.Body.Trim('"');

                // If viewing own profile, load profile data and skip further checks
                if (username == this.Username)
                {
                    // Load user profile data
                    usrName.Text = userData.username;
                    fullName.Text = userData.name;
                    dateofBirth.Text = "Ngày sinh: " + userData.dateofb;
                    proPnlAddress.Text = "Sinh sống tại: " + userData.city;
                    proPnlFromLbl.Text = "Đến từ: " + userData.country;
                    bioLbl.Text = userData.bio;

                    editPro.Visible = true; // Show edit profile button for own profile

                    // Clear any previous follow buttons
                    RemovePreviousFollowButton();

                    // Load user avatar
                    await LoadProfileAvatarAsync(username);

                    // Update follower and following counts
                    await UpdateFollowerFollowingCounts(username);

                    // Load user's posts
                    await LoadPostByUser(username);

                    return; // Skip the rest as we don't need to check for blocks or following for own profile
                }

                // Check if the current user is blocked by or has blocked the profile being viewed
                FirebaseResponse blockedByResponse = await client.GetAsync($"BlockedUsers/{username}/BlockedBy/{this.Username}");
                bool isBlockedByCurrentUser = blockedByResponse.Body != "null";

                FirebaseResponse blockedUserResponse = await client.GetAsync($"BlockedUsers/{this.Username}/BlockedBy/{username}");
                bool hasBlockedCurrentUser = blockedUserResponse.Body != "null";

                if (isBlockedByCurrentUser || hasBlockedCurrentUser)
                {
                    MessageBox.Show("You cannot view this profile.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    profilePnl.Visible = false;
                    return;
                }

                // Check if the current user is following the user (for private accounts)
                bool isFollowing = false;
                if (accountType == "private")
                {
                    FirebaseResponse followResponse = await client.GetAsync($"Follow/{this.Username}/Following/{username}");
                    isFollowing = followResponse.Body != "null";
                }

                // Load user profile data
                usrName.Text = userData.username;
                fullName.Text = userData.name;
                dateofBirth.Text = "Ngày sinh: " + userData.dateofb;
                proPnlAddress.Text = "Sinh sống tại: " + userData.city;
                proPnlFromLbl.Text = "Đến từ: " + userData.country;
                bioLbl.Text = userData.bio;

                // Hide edit profile button if viewing another user's profile
                editPro.Visible = false;

                // Clear any previous follow buttons
                RemovePreviousFollowButton();

                if (username != this.Username)
                {
                    ShowFollowButton(username);
                }

                // Load user avatar
                await LoadProfileAvatarAsync(username);

                // Update follower and following counts
                await UpdateFollowerFollowingCounts(username);

                if (accountType == "private" && !isFollowing)
                {
                    // Show restricted view for private accounts
                    ShowPrivateAccountMessage();
                }
                else
                {
                    // Load user's posts
                    await LoadPostByUser(username);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ShowPrivateAccountMessage()
        {
            flowLayoutLoadPostImages.Controls.Clear();
            Label privateMessage = new Label
            {
                Text = "Đây là tài khoản riêng tư, theo dõi để xem",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                
            };
            flowLayoutLoadPostImages.Controls.Add(privateMessage);
        }


        private void RemovePreviousFollowButton()
        {
            followProfileBtn.Visible = false;

            proPnlAvt.Image = Properties.Resources.profile;
        }

        private async void ShowFollowButton(string username)
        {
            followProfileBtn.Visible = true;

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();

            // Check if the current user is already following the displayed user
            FirebaseResponse response = await client.GetAsync($"Follow/{currentUser.username}/Following/{username}");
            bool isFollowing = response.Body != "null";

            followProfileBtn.Tag = username; // Store the username as tag

            if (isFollowing)
            {
                followProfileBtn.Text = "Bỏ theo dõi";
                followProfileBtn.BorderColor = Color.MediumAquamarine;
                followProfileBtn.FillColor = Color.Transparent;
                followProfileBtn.ForeColor = Color.MediumAquamarine;
                followProfileBtn.BorderThickness = 2;
                followProfileBtn.Click += UnfollowProfileButton_Click;
            }
            else
            {
                followProfileBtn.Text = "Theo dõi";
                followProfileBtn.BorderColor = Color.Transparent;
                followProfileBtn.FillColor = Color.MediumAquamarine;
                followProfileBtn.ForeColor = Color.White;
                followProfileBtn.BorderThickness = 0;
                followProfileBtn.Click += FollowProfileButton_Click;
            }
        }

        private async void FollowProfileButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button followProfileBtn = sender as Guna.UI2.WinForms.Guna2Button;
            string usernameToFollow = followProfileBtn.Tag as string;

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();

            // Check if the current user is already following the selected user
            FirebaseResponse response = await client.GetAsync($"Follow/{currentUser.username}/Following/{usernameToFollow}");
            bool isFollowing = response.Body != "null";

            if (!isFollowing)
            {
                // Follow the user with a timestamp
                var followData = new { followed_at = DateTime.UtcNow };
                await client.SetAsync($"Follow/{currentUser.username}/Following/{usernameToFollow}", followData);
                await client.SetAsync($"Follow/{usernameToFollow}/Follower/{currentUser.username}", followData);

                // Create notification
                await CreateNotification(currentUser.username, usernameToFollow, "Follow", $"{currentUser.username} đã bắt đầu theo dõi bạn.");

                // Update button to "Unfollow"
                followProfileBtn.Text = "Bỏ theo dõi";
                followProfileBtn.BorderColor = Color.MediumAquamarine;
                followProfileBtn.FillColor = Color.Transparent;
                followProfileBtn.ForeColor = Color.MediumAquamarine;
                followProfileBtn.BorderThickness = 2;
                followProfileBtn.Click -= FollowProfileButton_Click;
                followProfileBtn.Click += UnfollowProfileButton_Click;

                // Reload the profile to show posts if it's a private account
                await LoadUserProfile(usernameToFollow);
            }
            else
            {
                // Unfollow the user
                await UnfollowProfileUser(currentUser, usernameToFollow, followProfileBtn);
            }
            await UpdateFollowerFollowingCounts(usernameToFollow);
        }

        private async void UnfollowProfileButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button followProfileBtn = sender as Guna.UI2.WinForms.Guna2Button;
            string usernameToUnfollow = followProfileBtn.Tag as string;

            // Fetch the current user
            Data currentUser = await GetCurrentUserInfo();

            // Unfollow the user
            await UnfollowProfileUser(currentUser, usernameToUnfollow, followProfileBtn);

            // Reload the profile to show restricted view if it's a private account
            await LoadUserProfile(usernameToUnfollow);
            await UpdateFollowerFollowingCounts(usernameToUnfollow);
        }


        private async Task UnfollowProfileUser(Data currentUser, string usernameToUnfollow, Guna.UI2.WinForms.Guna2Button followProfileBtn)
        {
            // Unfollow the user with a timestamp
            var unfollowData = new { unfollowed_at = DateTime.UtcNow };
            await client.DeleteAsync($"Follow/{currentUser.username}/Following/{usernameToUnfollow}");
            await client.DeleteAsync($"Follow/{usernameToUnfollow}/Follower/{currentUser.username}");

            // Optionally, you can log the unfollow action with a timestamp
            await client.SetAsync($"UnfollowLog/{currentUser.username}/{usernameToUnfollow}", unfollowData);
            await client.SetAsync($"UnfollowLog/{usernameToUnfollow}/{currentUser.username}", unfollowData);

            // Update button to "Theo dõi"
            followProfileBtn.Text = "Theo dõi";
            followProfileBtn.BorderColor = Color.Transparent;
            followProfileBtn.FillColor = Color.MediumAquamarine;
            followProfileBtn.ForeColor = Color.White;
            followProfileBtn.BorderThickness = 0;
            followProfileBtn.Click -= UnfollowProfileButton_Click;
            followProfileBtn.Click += FollowProfileButton_Click;
        }

        private async Task UpdateFollowerFollowingCounts(string username)
        {
            try
            {
                FirebaseResponse followerResponse = await client.GetAsync($"Follow/{username}/Follower");
                var followerDict = followerResponse.Body != "null" ? JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followerResponse.Body) : new Dictionary<string, FollowInfo>();
                int followerCount = followerDict.Count;

                FirebaseResponse followingResponse = await client.GetAsync($"Follow/{username}/Following");
                var followingDict = followingResponse.Body != "null" ? JsonConvert.DeserializeObject<Dictionary<string, FollowInfo>>(followingResponse.Body) : new Dictionary<string, FollowInfo>();
                int followingCount = followingDict.Count;

                followerBtn.Text = $"{followerCount} người theo dõi";
                followingBtn.Text = $"{followingCount} đang theo dõi";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating follower and following counts: {ex.Message}");
                followerBtn.Text = "0 người theo dõi";
                followingBtn.Text = "0 đang theo dõi";
            }
        }

        private async Task LoadPostByUser(string username)
        {
            try
            {
                this.loadingPostForUser.Visible = true;
                this.flowLayoutLoadPostImages.Visible = false;

                // Fetch account type
                FirebaseResponse accountTypeResponse = await client.GetAsync($"Settings/{username}/AccountType");
                string accountType = accountTypeResponse.Body.Trim('"');

                // Fetch current user
                Data currentUser = await GetCurrentUserInfo();

                bool isCurrentUser = username == currentUser.username;

                // Check if current user follows the profile being viewed
                bool isFollowing = false;
                if (!isCurrentUser)
                {
                    FirebaseResponse followResponse = await client.GetAsync($"Follow/{currentUser.username}/Following/{username}");
                    isFollowing = followResponse.Body != "null" && followResponse.Body != "null\n";
                }

                // If the account is private and the current user is not following, show a private account message
                if (!isCurrentUser && accountType == "private" && !isFollowing)
                {
                    ShowPrivateAccountMessage();
                    return;
                }

                List<Post> posts = await this.getPostsByUsername(username);
                posts = posts.OrderByDescending(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", null)).ToList();

                int totalPost = posts.Count;
                totalPostOfUser.Text = $"{totalPost} Bài viết";

                if (flowLayoutLoadPostImages.Controls.Count > 0)
                {
                    flowLayoutLoadPostImages.Controls.Clear();
                }

                foreach (Post post in posts)
                {
                    Panel panel = new Panel
                    {
                        Width = 300,
                        Height = 300,
                        Margin = new Padding(5),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    PictureBox pictureBox = new PictureBox
                    {
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Width = 300,
                        Height = 300
                    };

                    pictureBox.Click += new EventHandler(async (object sender, EventArgs e) =>
                    {
                        this.panelDetailPost.Visible = true;
                        profileBtn.Checked = false;
                        profilePnl.Visible = false;
                        await LoadComments(post.id.ToString());
                    });

                    await LoadImageHttp.LoadImageAsync(post.imageUrl, pictureBox);

                    panel.Controls.Add(pictureBox);

                    if (isCurrentUser)
                    {
                        Button deleteButton = new Button
                        {
                            Text = "X",
                            ForeColor = Color.Red,
                            BackColor = Color.Transparent,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(30, 30),
                            Location = new Point(270, 0),
                            Cursor = System.Windows.Forms.Cursors.Hand
                        };

                        deleteButton.FlatAppearance.BorderSize = 0;
                        deleteButton.BringToFront();
                        deleteButton.Click += async (sender, e) =>
                        {
                            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bài viết `{post.caption}` này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                await this.DeletePost(post.id.ToString());
                                await this.DeleteCommentByPost(post.id.ToString());

                                MessageBox.Show("Xóa bài viết và comment thành công!");
                                this._posts.Clear();

                                totalPost -= 1;
                                totalPostOfUser.Text = $"{totalPost} Bài viết";

                                flowLayoutLoadPostImages.Controls.Remove(panel);
                            }
                        };

                        panel.Controls.Add(deleteButton);
                        deleteButton.BringToFront();
                    }

                    flowLayoutLoadPostImages.Controls.Add(panel);
                }
            }
            finally
            {
                this.loadingPostForUser.Visible = false;
                this.flowLayoutLoadPostImages.Visible = true;
            }
        }

        // end of profile showing
        // end of profile showing
        // end of profile showing
    }
}
