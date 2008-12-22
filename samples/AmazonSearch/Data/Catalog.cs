// Catalog.cs
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using SilverlightFX.Applications;

namespace AmazonSearch.Data {

    internal sealed class Catalog : ICatalog {

        private const string SearchUriFormat = "http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&Version=2005-03-23&Operation=ItemSearch&SubscriptionId={0}&AssociateTag=myamzn-20&SearchIndex=Books&Keywords={1}&Sort=relevancerank&ResponseGroup=Images,Small";

        public void SelectProducts(string keyword, Action<string, IEnumerable<Product>> productsCallback) {
            string subscriptionID = XApplication.Current.StartupArguments["SubscriptionID"];
            Uri searchUri = new Uri(String.Format(SearchUriFormat, subscriptionID, keyword));

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        // Remove the default xmlns, simply because it simplifies the node names
                        // we use in the XLINQ statement next.
                        xml = xml.Replace(@"xmlns=""http://webservices.amazon.com/AWSECommerceService/2005-03-23""", String.Empty);
                        XDocument xdoc = XDocument.Parse(xml);

                        IEnumerable<Product> productsQuery =
                            from item in xdoc.Descendants("Item")
                            where item.HasElements &&
                                  item.Element("ItemAttributes").HasElements &&
                                  item.Element("ItemAttributes").Element("Author") != null
                            select new Product {
                                ASIN = item.Element("ASIN").Value,
                                ItemUrl = item.Element("DetailPageURL").Value,
                                ImageUrl = item.Element("MediumImage").Element("URL").Value,
                                Title = item.Element("ItemAttributes").Element("Title").Value,
                                By = item.Element("ItemAttributes").Element("Author").Value
                            };

                        Product[] products = productsQuery.ToArray();
                        productsCallback(keyword, products);

                        return;
                    }

                    productsCallback(keyword, null);
                }
            };

            webClient.DownloadStringAsync(searchUri);
        }
    }
}
