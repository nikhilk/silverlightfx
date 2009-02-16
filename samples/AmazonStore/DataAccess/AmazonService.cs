// AmazonService.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SilverlightFX.Applications;

namespace Store {

    [Service(typeof(IStore))]
    public sealed class AmazonService : IStore {

        private const string SearchUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=ItemSearch&SearchIndex=All&SubscriptionId={0}&AssociateTag=myamzn-20&Keywords={1}&ResponseGroup=Images,Small,EditorialReview,ItemAttributes,OfferSummary";
        private const string LookupUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=ItemLookup&SubscriptionId={0}&AssociateTag=myamzn-20&ItemId={1}&&ResponseGroup=Images,Small,EditorialReview,ItemAttributes,OfferSummary";
        private const string CartUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=CartCreate&SubscriptionId={0}&AssociateTag=myamzn-20{1}";
        private static readonly string[] PopularFeedUrls =
            new string[] {
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopbooks",
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopmusic",
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopdvd",
                "http://pipes.yahooapis.com/pipes/pipe.run?_id=YAchPx7R3RG_yuOK1JzWFw&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Ftopelectronics"
            };
        private static readonly string BargainFeedUrl =
            "http://pipes.yahooapis.com/pipes/pipe.run?_id=e2b4c973b81b4963b60060d6a32a3f40&_render=rss&feedUrl=http%3A%2F%2Frssfeeds.s3.amazonaws.com%2Fgoldbox&count=5";

        private static readonly Regex TagRegex = new Regex("<.*?>", RegexOptions.Multiline);
        private static readonly Regex AsinRegex = new Regex("https?://www\\.amazon\\.[^/]+.*/([A-Z0-9]{10})/?.*", RegexOptions.Singleline);

        [Dependency]
        public IApplicationIdentity Application {
            get;
            set;
        }

        public void GetBargainProducts(Action<IEnumerable<Product>, bool> productsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;
                    try {
                        List<string> ids = new List<string>();

                        ParseProductIDs(xml, ids);
                        GetProductsByIDs(ids.ToArray(), /* lookupOffers */ true, (productResults) => {
                            if (productResults != null) {
                                productsCallback(productResults, true);
                            }
                        });
                    }
                    catch {
                    }
                }
            };

            webClient.DownloadStringAsync(new Uri(BargainFeedUrl));
        }

        public void GetPopularProducts(Action<IEnumerable<Product>, bool> productsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    int feedIndex = (int)e.UserState;
                    feedIndex++;

                    string xml = e.Result;
                    try {
                        List<string> ids = new List<string>();

                        ParseProductIDs(xml, ids);
                        GetProductsByIDs(ids.ToArray(), /* lookupOffers */ false, (productResults) => {
                            bool completed = feedIndex == PopularFeedUrls.Length;

                            if (productResults != null) {
                                productsCallback(productResults, completed);
                            }

                            if (completed == false) {
                                ((WebClient)sender).DownloadStringAsync(new Uri(PopularFeedUrls[feedIndex]), feedIndex);
                            }
                        });
                    }
                    catch {
                    }
                }
            };

            webClient.DownloadStringAsync(new Uri(PopularFeedUrls[0]), 0);
        }

        public void GetProducts(string keyword, Action<IEnumerable<Product>, bool> productsCallback) {
            try {
                GetProductsByKeyword(keyword, productsCallback);
            }
            catch {
                productsCallback(null, true);
            }
        }

        private void GetProductsByIDs(string[] ids, bool lookupOffers, Action<IEnumerable<Product>> productsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                List<Product> products = new List<Product>();

                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        ParseProducts(xml, lookupOffers, products);
                    }
                }

                productsCallback(products);
            };

            Uri requestUri = new Uri(String.Format(LookupUriFormat, SubscriptionID, String.Join(",", ids)));
            webClient.DownloadStringAsync(requestUri);
        }

        private void GetProductsByKeyword(string keyword, Action<IEnumerable<Product>, bool> productsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                List<Product> products = new List<Product>();

                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        ParseProducts(xml, /* lookupOffers */ false, products);
                    }
                }

                productsCallback(products, true);
            };

            Uri searchUri = new Uri(String.Format(SearchUriFormat, SubscriptionID, keyword));
            webClient.DownloadStringAsync(searchUri);
        }

        private string SubscriptionID {
            get {
                return Application.StartupArguments["SubscriptionID"];
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

        private void ParseProducts(string xml, bool lookupOffers, List<Product> products) {
            xml = xml.Replace(@"xmlns=""http://webservices.amazon.com/AWSECommerceService/2005-03-23""", String.Empty);
            XDocument doc = XDocument.Parse(xml);

            foreach (XElement itemElement in doc.Descendants("Item")) {
                try {
                    XElement imageElement = itemElement.Element("MediumImage");
                    XElement priceElement = itemElement.Element("ItemAttributes").Element("ListPrice");
                    XElement reviewsElement = itemElement.Element("EditorialReviews");
                    XElement descriptionElement = (reviewsElement != null) ? reviewsElement.Descendants("Content").FirstOrDefault() : null;

                    Product p = new Product {
                        ID = itemElement.Element("ASIN").Value,
                        Title = itemElement.Element("ItemAttributes").Element("Title").Value,
                        Price = (priceElement != null) ? Int32.Parse(priceElement.Element("Amount").Value) / 100m : 0m,
                        ProductUri = itemElement.Element("DetailPageURL").Value,
                        ImageUri = (imageElement != null) ? imageElement.Element("URL").Value : null,
                        Description = (descriptionElement != null) ? TagRegex.Replace(descriptionElement.Value, String.Empty) : null
                    };

                    if (lookupOffers || (priceElement == null)) {
                        XElement offerSummaryElement = itemElement.Element("OfferSummary");
                        priceElement = (offerSummaryElement != null) ? offerSummaryElement.Element("LowestNewPrice") : null;

                        if (priceElement != null) {
                            p.Price = Int32.Parse(priceElement.Element("Amount").Value) / 100m;
                        }
                    }

                    if (p.Price != 0m) {
                        products.Add(p);
                    }
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
                                itemNumber, item.Product.ID, item.Quantity);
                itemNumber++;
            }

            Uri requestUri = new Uri(String.Format(CartUriFormat, SubscriptionID, sb.ToString()));
            webClient.DownloadStringAsync(requestUri);
        }
    }
}
