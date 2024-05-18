using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Firebase.Storage;
using System.Net;
using System.Globalization;

namespace iConnect
{
    public partial class Home : Form
    {
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
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + Username);
            Data result = response.ResultAs<Data>();
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
            changeBDay.Value = DateTime.ParseExact(result.dateofb, dateFormat, CultureInfo.InvariantCulture);

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
            reload();
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
        }

        private void notiBtn_Click(object sender, EventArgs e)
        {
            reload();
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
            notiPnl.Visible = true;
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            reload();
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
            notiPnl.Visible = false;
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
        }

        private void msgBtn_Click(object sender, EventArgs e)
        {
            reload();
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
            notiPnl.Visible = false;
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
            reload();
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
            notiPnl.Visible = false;
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
            reload();
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
            notiPnl.Visible = false;

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
    }
}
