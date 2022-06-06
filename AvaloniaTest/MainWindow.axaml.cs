using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaTest.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace AvaloniaTest
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

            string image = htmlDoc.DocumentNode.SelectSingleNode(
                    "//div[@class='x-column x-sm x-2-3']/img").Attributes["data-lazy-src"].Value;

            string author = RemoveTags(WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode(
                "//h2[@class='h-custom-headline h2']/span").InnerHtml).Trim());

            string playlist = RemoveTags(WebUtility.HtmlDecode(htmlDoc.DocumentNode.SelectSingleNode(
                "//h3[@class='h-custom-headline h5']/span").InnerHtml).Trim());

            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(
                "//ul[@class='x-block-grid two-up']/li");

            List<StackPanel> controls = new();
            StackPanel playlistInfo = new();
            playlistInfo.Children.Add(new TabStripItem()
            {
                Content = $"{author}\n{playlist}",
                IsEnabled = false,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            });
            controls.Add(playlistInfo);
            foreach (var node in nodes)
            {
                List<string> items = new();
                foreach (var htmlNode in node.DescendantNodesAndSelf())
                {
                    if (htmlNode.HasChildNodes)
                    {
                        continue;
                    }

                    string text = RemoveTags(WebUtility.HtmlDecode(htmlNode.InnerText).Trim());

                    if (!string.IsNullOrEmpty(text) && !text.Contains("CHAPTER"))
                    {
                        items.Add(text);
                    }
                }

                string cdName = items[0];
                List<(string, string)> data = new();
                for (int i = 1; i < items.Count; i++)
                {
                    string item = items[i];
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }

                    const string pattern = @"^\d.";
                    if (Regex.IsMatch(item, pattern))
                    {
                        item = Regex.Replace(item, pattern, string.Empty);
                    }

                    string[] values = item.Split(new[] { '–' }, 2, StringSplitOptions.TrimEntries);
                    if (values.Length < 2)
                    {
                        continue;
                    }

                    string songAuthor = values[0];
                    string songName = values[1];
                    data.Add(($"Author: {songAuthor}", $"Song name: {songName}"));
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
                    Header = cdName,
                    Content = expanderChildren,
                };
                tab.Children.Add(expander);
                tab.Background = Brushes.Transparent;
                controls.Add(tab);
            }

            return new PlaylistViewModel(controls, image);
        }

        private static string RemoveTags(string input)
        {
            string pattern = @"\<\s*br\s*\/?\s*\>";
            return Regex.IsMatch(input, pattern) ? Regex.Replace(input, pattern, string.Empty) : input;
        }
    }
}
