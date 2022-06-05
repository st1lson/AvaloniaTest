using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TestTask.Models;

namespace TestTask
{
    public partial class MainWindow : Window
    {
        private const string DefaultUrl = "https://www.globalunderground.co.uk/music/patrice-baumel-gu42-berlin";

        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context)
            {
                return;
            }

            string url = context.Url;
            if (string.IsNullOrEmpty(url))
            {
                url = DefaultUrl;
            }

            PlaylistViewModel newContext = LoadPlaylist(url);

            Window infoPage = new InfoPage()
            {
                DataContext = newContext
            };
            infoPage.Show();
            Close();
        }

        private static PlaylistViewModel LoadPlaylist(in string url)
        {
            HtmlWeb web = new();
            HtmlDocument htmlDoc = web.Load(url);

            var images = htmlDoc.DocumentNode.SelectNodes("//img")
                .Select(img => new
                {
                    Link = img.Attributes["src"].Value
                })
                .ToList();

            string image = images[15].Link;

            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(
                "/html/body/div/div[1]/div/div/div/article/div/div/div/div/div[3]/div/ul[2]/li");

            List<StackPanel> controls = new();
            foreach (var node in nodes)
            {
                List<string> items = new();
                foreach (var htmlNode in node.DescendantNodesAndSelf())
                {
                    if (htmlNode.HasChildNodes)
                    {
                        continue;
                    }

                    string text = htmlNode.InnerText;
                    if (!string.IsNullOrEmpty(text))
                    {
                        items.Add(text.Trim());
                    }
                }

                string playlistName = items[0];
                List<(string, string)> data = new();
                for (int i = 1; i < items.Count; i++)
                {
                    string item = items[i];
                    if (item.Contains("CHAPTER"))
                    {
                        continue;
                    }

                    Regex regex = new(@"[\d]");
                    if (regex.IsMatch(item))
                    {
                        item = item.Remove(0, 3);
                    }

                    string[] values = item.Split('–', 2, StringSplitOptions.TrimEntries);
                    string author = values[0];
                    string songName = values[1];
                    data.Add(($"Author: {author}", $"Song name: {songName}"));
                }

                StackPanel tab = new();

                List<ListBoxItem> listItems = data.Select(tuple => new ListBoxItem()
                {
                    Content = $"{tuple.Item1}\n{tuple.Item2}",
                    Margin = new Thickness(10, 0, 0, 0)
                }).ToList();

                StackPanel expanderChildren = new();
                expanderChildren.Children.AddRange(listItems);
                Expander expander = new()
                {
                    Header = $"Playlist name: {playlistName}",
                    Content = expanderChildren,
                };
                tab.Children.Add(expander);
                tab.Background = Brushes.Transparent;
                controls.Add(tab);
            }

            return new PlaylistViewModel(controls, image);
        }
    }
}
