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
        }

        // Call this method when the form loads to populate the blocked users panel
        private async Task Form_Load(object sender, EventArgs e)
        {
            await LoadBlockedUsers();
        }

        private async void reload()
        {
            this.panelDetailPost.Visible = false;

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = await client.GetAsync("Users/" + Username);
            Data result = response.ResultAs<Data>();

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

            proBtn.Text = "@" + result.username;
            usrName.Text = result.username;
            fullName.Text = result.name;
            dateofBirth.Text = "Ngày sinh: " + result.dateofb;
            changeUsrname.Text = result.username;
            proPnlAddress.Text = "Sinh sống tại: " + result.city;
            proPnlFromLbl.Text = "Đến từ: " + result.country;
            bioLbl.Text = result.bio;

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

            await LoadAvatarAsync(Username);

            // Retrieve and set the account type
            string accountType = await GetAccountType(result.username);
            privateAccountBtn.Checked = (accountType == "private");

            // Check and print removed posts
            await CheckAndPrintRemovedPosts();
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
            notiPnl.Visible = false;
            postBtn.Checked = false;
            addPostPannel.Visible = false;

            reload();
            this.clearPickerUpload();
        }

        private void notiBtn_Click(object sender, EventArgs e)
        {
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
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
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
        }

        private void msgBtn_Click(object sender, EventArgs e)
        {
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
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {

        }

        private void followerBtn_Click(object sender, EventArgs e)
        {
          //  followerPnl.Visible = true;
        }

        private void followingBtn_Click(object sender, EventArgs e)
        {
          //  followingPnl.Visible = true;
        }

        private void profileBtn_Click(object sender, EventArgs e)
        {
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
        }

        private async Task DeleteComment(string postId, string keyId)
        {
            await client.DeleteAsync($"comments/{postId}/{keyId}");
        }

        private async Task DeleteCommentByPost(string postId)
        {
            await client.DeleteAsync($"comments/{postId}");
        }

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
                posts = posts.FindAll((t) => t.user.Equals(Username)).OrderByDescending(p => DateTime.ParseExact(p.created_at, "dd/MM/yyyy HH:mm:ss", null)).ToList();

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

        private void settingBtn_Click(object sender, EventArgs e)
        {
            homeBtn.Checked = false;

            reload();
            this.clearPickerUpload();

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

        private void pnlPost1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ImageButton5_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton5_Click_1(object sender, EventArgs e)
        {

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

        private void allnotiBtn_Click(object sender, EventArgs e)
        {
            allnotiBtn.Checked = true;
            mentionnotiBtn.Checked = false;
            recnotiBtn.Checked = false;
        }

        private void mentionnotiBtn_Click(object sender, EventArgs e)
        {
            allnotiBtn.Checked = false;
            mentionnotiBtn.Checked = true;
            recnotiBtn.Checked = false;
        }

        private void recnotiBtn_Click(object sender, EventArgs e)
        {
            allnotiBtn.Checked = false;
            mentionnotiBtn.Checked = false;
            recnotiBtn.Checked = true;
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
                if (newUsername != usrName.Text)
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
                FirebaseResponse response = await client.GetAsync("Users/" + usrName.Text);

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
                    if (newUsername != usrName.Text)
                    {
                        existingData.username = newUsername;
                        Username = newUsername;
                    }

                    // Retain other fields such as AvatarUrl and email
                    string avatarUrl = existingData.AvatarUrl;
                    string email = existingData.email;
                    string password = existingData.password;

                    // Delete old data entry
                    FirebaseResponse deleteResponse = await client.DeleteAsync("Users/" + usrName.Text);

                    if (deleteResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
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
                            usrName.Text = newUsername; // Update the username in the UI
                            MessageBox.Show("Thông tin người dùng đã được cập nhật thành công.");
                        }
                        else
                        {
                            MessageBox.Show("Không thể cập nhật thông tin người dùng.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa thông tin người dùng cũ.");
                    }
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy thông tin người dùng.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin người dùng: {ex.Message}");
            }
        }

        private void postBtn_Click(object sender, EventArgs e)
        {
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

                foreach (var item in jsonPosts)
                {
                    Post post = await this.mapperPost(item.Value);
                    posts.Add(post);
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

            if (response != null)
            {
                MessageBox.Show("Đăng bình luận thành công!");
                txbComment.Text = "";
                if (!string.IsNullOrEmpty(parentId)) this.selectedCommentId = string.Empty;
                this.HiddenReply();
            }

            // Update count comment in post
            Post post = await this.getPostById(postId);

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
                Width = 200
            };

            Guna.UI2.WinForms.Guna2HtmlLabel usernameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = user.username,
                Left = 60,
                Top = 25,
                Width = 200
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

        private async void SearchPostsByUsername(string username)
        {
            //if (string.IsNullOrEmpty(username))
            //{
            //    MessageBox.Show("Please enter a username to search for.");
            //    return;
            //}

            //try
            //{
            //    List<Post> posts = await this.getPostsByUsername(username);
            //    this.RenderPosts(posts);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"An error occurred while searching for posts: {ex.Message}");
            //}

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username to search for.");
                return;
            }

            try
            {
                List<Post> posts = await this.getPostsByUsername(username);
                if (posts.Count > 0)
                {
                    this.RenderPosts(posts);
                }
                else
                {
                    MessageBox.Show("No posts found for the specified username.");
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

        public static Bitmap ResizeImage1(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public static async Task LoadImageAsync(string imageUrl, PictureBox pictureBox)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageUrl);
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (Bitmap bitmap = new Bitmap(stream))
                        {
                            Bitmap resizedImage = ResizeImage1(bitmap, pictureBox.Width, pictureBox.Height);
                            pictureBox.Image = resizedImage;
                        }
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Error loading image: Out of memory. Try using smaller images.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the image: {ex.Message}");
            }
        }


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
    }
}
