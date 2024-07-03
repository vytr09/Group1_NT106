using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using iConnect.Helpers;
using iConnect.UserControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
        }

        private async void reload()
        {
            this.panelDetailPost.Visible = false;

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + Username);
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

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
           // followingPnl.Visible = false;
        }

        private void guna2ControlBox3_Click(object sender, EventArgs e)
        {
          //  followerPnl.Visible = false;
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
            this.Hide();
            Login login = new Login();
            login.Show();
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
                // Lấy thông tin mới từ các thành phần giao diện người dùng
                string bio = bioTxtChange.Text;
                string name = changeFullName.Text;
                string dateofb = changeBDay.Text;
                string country = countryTxt.Text;
                string city = currentCity.Text;
                string gender = genderCmb.Text;
                string relationship = relaCmb.Text;

                // Lấy thông tin cũ từ cơ sở dữ liệu Firebase
                FirebaseResponse response = await client.GetAsync("Users/" + usrName.Text);

                // Kiểm tra xem thông tin cũ đã có hay chưa
                if (response != null && response.Body != "null")
                {
                    // Lấy thông tin cũ từ kết quả truy vấn
                    Data existingData = response.ResultAs<Data>();

                    // Cập nhật thông tin mới vào thông tin cũ
                    existingData.bio = bio;
                    existingData.name = name;
                    existingData.dateofb = dateofb;
                    existingData.country = country;
                    existingData.city = city;
                    existingData.gender = gender;
                    existingData.relationship = relationship;

                    // Sử dụng phương thức UpdateAsync của client Firebase để cập nhật dữ liệu người dùng
                    FirebaseResponse updateResponse = await client.UpdateAsync("Users/" + usrName.Text, existingData);

                    // Kiểm tra xem cập nhật có thành công không
                    if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Thông tin người dùng đã được cập nhật thành công.");
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật thông tin người dùng.");
                    }
                }
                else
                {
                    // Tạo một đối tượng Data mới với thông tin mới
                    Data newData = new Data
                    {
                        bio = bio,
                        name = name,
                        dateofb = dateofb,
                        country = country,
                        city = city,
                        gender = gender,
                        relationship = relationship
                    };

                    // Sử dụng phương thức PushAsync của client Firebase để thêm dữ liệu mới
                    FirebaseResponse pushResponse = await client.PushAsync("Users", newData);

                    // Kiểm tra xem thêm dữ liệu mới có thành công không
                    if (pushResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Thông tin người dùng đã được cập nhật thành công.");
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật thông tin người dùng.");
                    }
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
                return "vừa xong";
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
                return "hôm qua";
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

                this.labelAuthorDetail.Text = post.userData.name;
                await LoadImageHttp.LoadImageAsync(post.imageUrl, this.picturePostDetail);
                this.labelPostCaptionDetail.Text = post.caption;
                this.labelCreatedAt.Text = this.FormatFacebookTime(post.created_at);

                foreach (var item in comments)
                {
                    UC_Comment comment = new UC_Comment();

                    comment.Dock = DockStyle.Top;
                    comment.AuthorText = item.userData.name;
                    comment.CommentText = item.comment;
                    comment.CreatedAtText = this.FormatFacebookTime(item.created_at);
                    comment.Padding = new Padding(0, 0, 0, 10);
                    comment.ShowDelete = item.user.Equals(Username) ? true : false;

                    comment.ButtonDeleteClick = new System.EventHandler((object sender, EventArgs e) =>
                    {
                        this.handleDeleteComment(item.postId, item.key, item.id.ToString(), item.children);
                    });

                    comment.ButtonReplyClick = new System.EventHandler((object sender, EventArgs e) =>
                    {
                        this.handleReplayComment(item.id.ToString(), item.user);
                    });

                    if (!string.IsNullOrEmpty(item.userData.AvatarUrl))
                    {
                        comment.LoadAvatarAuthor(item.userData.AvatarUrl);
                    }

                    if (item.children.Count > 0)
                    {
                        foreach (var child in item.children)
                        {
                            UC_Comment commentChild = new UC_Comment();

                            Padding padding = new Padding(40, 0, 0, 10);

                            commentChild.Dock = DockStyle.Top;
                            commentChild.Padding = padding;
                            commentChild.AuthorText = child.userData.name;
                            commentChild.CommentText = child.comment;
                            commentChild.CreatedAtText = this.FormatFacebookTime(child.created_at);
                            commentChild.ShowDelete = child.user.Equals(Username) ? true : false;

                            commentChild.ButtonDeleteClick = new System.EventHandler((object sender, EventArgs e) =>
                            {
                                this.handleDeleteComment(child.postId, child.key, child.id.ToString(), new List<Comment>());
                            });

                            commentChild.ButtonReplyClick = new System.EventHandler((object sender, EventArgs e) =>
                            {
                                this.handleReplayComment(item.id.ToString(), child.user);
                            });

                            if (!string.IsNullOrEmpty(child.userData.AvatarUrl))
                            {
                                commentChild.LoadAvatarAuthor(child.userData.AvatarUrl);
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

        private void profilePnl_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
