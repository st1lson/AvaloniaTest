using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace TestTask.Models
{
    internal class PlaylistViewModel : ViewModelBase
    {
        private List<StackPanel> _items;

        public List<StackPanel> Items
        {
            get => _items;
            set
            {
                if (value.SequenceEqual(_items))
                {
                    return;
                }

                _items = value;
                RaisePropertyChanged(nameof(Items));
            }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                RaiseAndSetIfChanged(ref _imageUrl, value);
                DownloadImage(ImageUrl);
                Debug.WriteLine(ImageUrl);
            }
        }

        private Bitmap _image = null;
        public Bitmap Image
        {
            get => _image;
            set => RaiseAndSetIfChanged(ref _image, value);
        }

        public PlaylistViewModel(List<StackPanel> controls, string imageUrl)
        {
            _imageUrl = imageUrl;
            _items = controls;
            DownloadImage(ImageUrl);
        }

        public void DownloadImage(string url)
        {
            using WebClient client = new();
            client.DownloadDataAsync(new Uri(url));
            client.DownloadDataCompleted += DownloadComplete;
        }

        private void DownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                byte[] bytes = e.Result;

                Stream stream = new MemoryStream(bytes);

                Bitmap image = new(stream);
                Image = image;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Image = null;
            }
        }
    }
}
