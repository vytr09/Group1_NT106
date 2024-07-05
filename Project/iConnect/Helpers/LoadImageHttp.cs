using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iConnect.Helpers
{
    internal class LoadImageHttp
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async static Task LoadImageAsync(string url, PictureBox pictureBox)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var originalImage = Image.FromStream(stream))
                    {
                        var resizedImage = ResizeImage(originalImage, pictureBox.Width, pictureBox.Height);
                        pictureBox.Image = resizedImage;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var resizedImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }
    }
}
