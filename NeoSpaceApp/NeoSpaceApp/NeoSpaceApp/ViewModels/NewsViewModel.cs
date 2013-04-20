using HtmlAgilityPack;
using NeoSpaceApp.Extensions.Helpers;
using NeoSpaceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NeoSpaceApp.ViewModels
{
    public class NewsViewModel : ModelBase
    {
        private WebClient _wc = new WebClient();

        public event EventHandler LoadCompleted;
        public event EventHandler ErrorOccured;

        private NewsModel _model;
        public NewsModel Model
        {
            get { return _model; }
            set { _model = value; NotifyPropertyChanged("Model"); }
        }

        public void Load(string id)
        {
            if (!(id.StartsWith("http://www.jpl.nasa.gov/news/news.php?release=") || (id.StartsWith("http://neo.jpl.nasa.gov/news/") || !id.Contains("http://"))))
                return;
            if (!id.Contains("http://"))
                id = "http://neo.jpl.nasa.gov/news/" + id;
            _model = new NewsModel() { Id = id };

            _wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            _wc.DownloadStringAsync(new Uri(id));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    if (Model.Id.StartsWith("http://www.jpl.nasa.gov/news/news.php?release="))
                        ParseNews(e.Result);

                    if (Model.Id.StartsWith("http://neo.jpl.nasa.gov/news/"))
                        ParseNews2(e.Result);
                }
                catch { if (ErrorOccured != null)ErrorOccured(this, null); }
            }
            else
                if (ErrorOccured != null) ErrorOccured(this, null);
        }

        void ParseNews(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var title = doc.DocumentNode.SelectSingleNode("//h1[@class='h1news']");
            if (title != null)
                Model.Title = ParseHelper.Clean(title.InnerHtml);

            var imageContainer = doc.DocumentNode.SelectSingleNode("//span[@class='lead_image']");
            if (imageContainer != null)
            {
                var image = imageContainer.SelectSingleNode("descendant::img");
                if (image != null)
                    Model.Image = Model.LargeImage = image.Attributes["src"].Value;
            }

            var bodyPanel = doc.DocumentNode.SelectSingleNode("//div[@id='contentbox_inner_main']");
            if (bodyPanel != null)
            {
                var date = bodyPanel.SelectSingleNode("descendant::p[@class='bold']");
                if (date != null)
                    Model.Date = date.InnerText;
                var paragraphs = bodyPanel.SelectNodes("descendant::p");
                if (paragraphs != null)
                {
                    var orderedParagraphs = paragraphs.OrderByDescending(p => p.InnerHtml.Length).ToArray();
                    Model.Content = ParseHelper.Clean(orderedParagraphs[0].InnerHtml);
                    if (Model.Content != null && Model.Content.Length > 200)
                    {
                        var index = Model.Content.IndexOf("\n", 20);
                        if (index != -1)
                            Model.Synopsis = Model.Content.Substring(0, index);
                        else
                        {
                            index = Model.Synopsis.IndexOf(" ", 200);
                            if (index != -1)
                                Model.Synopsis = Model.Content.Substring(0, index);
                            else
                                Model.Synopsis = Model.Synopsis.Substring(0, 200);
                        }
                    }
                }
            }

            if (LoadCompleted != null)
                LoadCompleted(this, EventArgs.Empty);
        }

        void ParseNews2(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var title = doc.DocumentNode.SelectSingleNode("//h2");
            if (title != null)
                Model.Title = ParseHelper.Clean(title.InnerHtml);

            var bodyChild = doc.DocumentNode.SelectSingleNode("//a[@name='content']");
            if (bodyChild != null)
            {
                var bodyPanel = bodyChild.ParentNode;
                if (bodyPanel != null)
                {
                    var image = bodyPanel.SelectSingleNode("descendant::img");
                    if (image != null)
                    {
                        var temp = image.Attributes["src"].Value;

                        if (!temp.Contains("http://"))
                        {
                            var index = Model.Id.LastIndexOf('/');
                            if (index != -1)
                                temp = Model.Id.Substring(0, index) + '/' + temp;
                        }
                        
                        Model.Image = Model.LargeImage = temp;
                    }

                    Model.Content = ParseHelper.Clean(bodyPanel.InnerHtml);
                    if (Model.Content != null && Model.Content.Length > 200)
                    {
                        var index = Model.Content.IndexOf("\n", 20);
                        if (index != -1)
                            Model.Synopsis = Model.Content.Substring(0, index);
                        else
                        {
                            index = Model.Synopsis.IndexOf(" ", 200);
                            if (index != -1)
                                Model.Synopsis = Model.Content.Substring(0, index);
                            else
                                Model.Synopsis = Model.Synopsis.Substring(0, 200);
                        }
                    }
                }
            }

            if (LoadCompleted != null)
                LoadCompleted(this, EventArgs.Empty);
        }
    }
}
