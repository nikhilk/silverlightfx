// AmazonService.cs
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SilverlightFX.Applications;

namespace Store {

    internal sealed class AmazonService : IStore {

        private const string SearchUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=ItemSearch&SearchIndex=All&SubscriptionId={0}&AssociateTag=myamzn-20&Keywords={1}&ResponseGroup=Images,Small,EditorialReview,ItemAttributes";
        private const string LookupUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=ItemLookup&SubscriptionId={0}&AssociateTag=myamzn-20&ItemId={1}&&ResponseGroup=Images,Small,EditorialReview,ItemAttributes";
        private const string CartUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=CartCreate&SubscriptionId={0}&AssociateTag=myamzn-20{1}";
        private static readonly string[] PopularFeedUrls =
            new string[] {
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopbooks",
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopmusic",
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopdvd",
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopelectronics"
            };

        private static readonly Regex TagRegex = new Regex("<.*?>", RegexOptions.Multiline);
        private static readonly Regex AsinRegex = new Regex("https?://www\\.amazon\\.[^/]+.*/([A-Z0-9]{10})/?.*", RegexOptions.Singleline);

        public void GetPopularProducts(Action<IEnumerable<Product>> productsCallback) {
            List<Product> products = new List<Product>();

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    int feedIndex = (int)e.UserState;
                    feedIndex++;

                    string xml = e.Result;
                    try {
                        List<string> ids = new List<string>();

                        ParseProductIDs(xml, ids);
                        GetProductsByIDs(ids.ToArray(), (productResults) => {
                            if (productResults != null) {
                                products.AddRange(productResults);
                            }

                            if (feedIndex < PopularFeedUrls.Length) {
                                ((WebClient)sender).DownloadStringAsync(new Uri(PopularFeedUrls[feedIndex]), feedIndex);
                            }
                            else {
                                productsCallback(products);
                            }
                        });
                    }
                    catch {
                    }
                }
            };

            webClient.DownloadStringAsync(new Uri(PopularFeedUrls[0]), 0);
        }

        public void GetProducts(string keyword, Action<IEnumerable<Product>> productsCallback) {
            try {
                GetProductsByKeyword(keyword, productsCallback);
            }
            catch {
                productsCallback(null);
            }
        }

        private void GetProductsByIDs(string[] ids, Action<IEnumerable<Product>> productsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                List<Product> products = new List<Product>();

                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        ParseProducts(xml, products);
                    }
                }

                productsCallback(products);
            };

            Uri requestUri = new Uri(String.Format(LookupUriFormat, SubscriptionID, String.Join(",", ids)));
            webClient.DownloadStringAsync(requestUri);
        }

        private void GetProductsByKeyword(string keyword, Action<IEnumerable<Product>> productsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                List<Product> products = new List<Product>();

                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        ParseProducts(xml, products);
                    }
                }

                productsCallback(products);
            };

            Uri searchUri = new Uri(String.Format(SearchUriFormat, SubscriptionID, keyword));
            webClient.DownloadStringAsync(searchUri);
        }

        private string SubscriptionID {
            get {
                return XApplication.Current.StartupArguments["SubscriptionID"];
            }
        }

        private void ParseProductIDs(string feedXml, List<string> ids) {
            XDocument doc = XDocument.Parse(feedXml);

            foreach (XElement itemElement in doc.Descendants("item")) {
                try {
                    XElement linkElement = itemElement.Element("link");
                    if (linkElement != null) {
                        string link = linkElement.Value;

                        Match match = AsinRegex.Match(link);
                        if ((match != null) && (match.Groups.Count == 2)) {
                            string asin = match.Groups[1].Value;
                            ids.Add(asin);

                            if (ids.Count == 10) {
                                break;
                            }
                        }
                    }
                }
                catch {
                }
            }
        }

        private void ParseProducts(string xml, List<Product> products) {
            xml = xml.Replace(@"xmlns=""http://webservices.amazon.com/AWSECommerceService/2005-03-23""", String.Empty);
            XDocument doc = XDocument.Parse(xml);

            foreach (XElement itemElement in doc.Descendants("Item")) {
                try {
                    XElement imageElement = itemElement.Element("MediumImage");
                    XElement reviewsElement = itemElement.Element("EditorialReviews");
                    XElement descriptionElement = (reviewsElement != null) ? reviewsElement.Descendants("Content").FirstOrDefault() : null;

                    Product p = new Product {
                        ASIN = itemElement.Element("ASIN").Value,
                        Title = itemElement.Element("ItemAttributes").Element("Title").Value,
                        Price = Int32.Parse(itemElement.Element("ItemAttributes").Element("ListPrice").Element("Amount").Value) / 100m,
                        ProductUri = itemElement.Element("DetailPageURL").Value,
                        ImageUri = (imageElement != null) ? imageElement.Element("URL").Value : null,
                        Description = (descriptionElement != null) ? TagRegex.Replace(descriptionElement.Value, String.Empty) : null
                    };

                    products.Add(p);
                }
                catch {
                }
            }
        }

        public void SubmitOrder(Order order, Action<string> purchaseUrlCallback) {
            try {
                SubmitOrderCore(order, purchaseUrlCallback);
            }
            catch {
                purchaseUrlCallback(null);
            }
        }

        private void SubmitOrderCore(Order order, Action<string> purchaseUrlCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result.Replace(@"xmlns=""http://webservices.amazon.com/AWSECommerceService/2005-03-23""", String.Empty);

                    if (String.IsNullOrEmpty(xml) == false) {
                        XDocument doc = XDocument.Parse(xml);
                        XElement purchaseUrlElement = doc.Descendants("PurchaseURL").Single();

                        if (purchaseUrlElement != null) {
                            string purchaseUrl = purchaseUrlElement.Value;
                            purchaseUrlCallback(purchaseUrl);

                            return;
                        }
                    }
                }
                purchaseUrlCallback(null);
            };

            StringBuilder sb = new StringBuilder();

            int itemNumber = 1;
            foreach (OrderItem item in order.Items) {
                sb.AppendFormat(CultureInfo.InvariantCulture, "&Item.{0}.ASIN={1}&Item.{0}.Quantity={2}",
                                itemNumber, item.Product.ASIN, item.Quantity);
                itemNumber++;
            }

            Uri requestUri = new Uri(String.Format(CartUriFormat, SubscriptionID, sb.ToString()));
            webClient.DownloadStringAsync(requestUri);
        }
    }
}
