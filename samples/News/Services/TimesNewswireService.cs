// TimesNewswireService.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.IO;
using System.Net;
using System.Windows.Browser;
using System.Xml.Linq;
using SilverlightFX.Applications;
using NewsWidget.Data;

namespace NewsWidget.Services {

    [Service(typeof(INewsService))]
    public class TimesNewswireService : INewsService {

        private const string RecentNewsUriFormat = "http://api.nytimes.com/svc/news/v2/all/{0}.xml?api-key={1}";
        private const string SearchUriFormat = "http://api.nytimes.com/svc/search/v1/article?query={0}&api-key={1}";

        [Dependency]
        public IApplicationIdentity Application {
            get;
            set;
        }

        public void GetNews(bool limitToLastDay, Action<IEnumerable<NewsItem>, object> newsItemsCallback, object userState) {
            string timeFrame = limitToLastDay ? "last24hours" : "recent";
            string url = String.Format(RecentNewsUriFormat, timeFrame, Application.StartupArguments["NewswireApiKey"]);

            Uri requestUri = new Uri(url, UriKind.Absolute);

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        XDocument xdoc = XDocument.Parse(e.Result);
                        var newsItemsQuery =
                            from node in xdoc.Descendants("news_item")
                            select new NewsItem
                            {
                                Headline = node.Element("headline").Value,
                                Summary = ParseContent(node.Element("summary").Value),
                                Section = node.Element("section").Value,
                                PublishDate = DateTime.SpecifyKind(DateTime.Parse(node.Element("pubdate").Value), DateTimeKind.Utc),
                                Uri = new Uri(node.Attribute("url").Value, UriKind.Absolute)
                            };

                        NewsItem[] newsItems = newsItemsQuery.ToArray();
                        newsItemsCallback(newsItems, userState);

                        return;
                    }

                    newsItemsCallback(null, userState);
                }
            };

            webClient.DownloadStringAsync(requestUri);
        }

        private string ParseContent(string value) {
            return HttpUtility.HtmlDecode(value);
        }

        private DateTime ParseJsonDate(string value) {
            return new DateTime(Int32.Parse(value.Substring(0, 4)),
                                Int32.Parse(value.Substring(4, 2)),
                                Int32.Parse(value.Substring(6)),
                                0, 0, 0, DateTimeKind.Utc);
        }

        public void Search(string query, Action<IEnumerable<NewsItem>, object> newsItemsCallback, object userState) {
            string url = String.Format(SearchUriFormat, query, Application.StartupArguments["SearchApiKey"]);

            Uri requestUri = new Uri(url, UriKind.Absolute);

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string json = e.Result;

                    if (String.IsNullOrEmpty(json) == false) {
                        JsonValue jsonDoc = JsonObject.Parse(json);
                        var newsItemsQuery = from obj in (JsonArray)jsonDoc["results"]
                                             select new NewsItem
                                             {
                                                 Headline = (string)obj["title"],
                                                 Summary = ParseContent((string)obj["body"]),
                                                 Uri = new Uri((string)obj["url"], UriKind.Absolute),
                                                 PublishDate = ParseJsonDate((string)obj["date"])
                                             };

                        NewsItem[] newsItems = newsItemsQuery.ToArray();
                        newsItemsCallback(newsItems, userState);

                        return;
                    }

                    newsItemsCallback(null, userState);
                }
            };

            webClient.DownloadStringAsync(requestUri);
        }
    }
}
