using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageUploader
{
    public partial class MainWindow : Window
    {
        private string? _base64Photo; 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                SelectedImage.Source = bitmap;

                _base64Photo = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName));
            }
        }

        private async void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_base64Photo))
            {
                MessageBox.Show("Спочатку виберіть фото.");
                return;
            }

            try
            {
                var client = new RestClient("http://localhost:5000");
                var request = new RestRequest("api/photos/upload", Method.Post);

                request.AddJsonBody(new { Photo = $"data:image/jpeg;base64,{_base64Photo}" });

                var response = await client.ExecuteAsync(request);

                if (response.Content != null && response.IsSuccessful)
                {
                    dynamic? result = JsonConvert.DeserializeObject(response.Content);
                    string? imageUrl = result?.imageUrl;

                    if (imageUrl != null)
                    {
                        MessageBox.Show($"Фото завантажено: {imageUrl}");
                        SelectedImage.Source = new BitmapImage(new Uri($"http://localhost:5000{imageUrl}"));
                    }
                }
                else
                {
                    MessageBox.Show($"Помилка: {response.Content ?? "Невідома помилка"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка: {ex.Message}");
            }
        }
    }
}