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

        private const string SearchQueryFormat = "Service=AWSECommerceService&Version=2009-03-31&Operation=ItemSearch&SearchIndex=Books&AssociateTag=myamzn-20&Keywords={0}&ResponseGroup=Images,Small&Sort=relevancerank";
        private const string ServiceDomain = "ecs.amazonaws.com";

        public void SelectProducts(string keyword, Action<string, IEnumerable<Product>> productsCallback) {
            SignedRequestHelper signer =
                new SignedRequestHelper(ApplicationContext.Current.StartupArguments["AccessKey"],
                                        ApplicationContext.Current.StartupArguments["SecretKey"],
                                        ServiceDomain);

            string url = signer.Sign(String.Format(SearchQueryFormat, keyword.Replace(' ', '+')));
            Uri searchUri = new Uri(url, UriKind.Absolute);

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        // Remove the default xmlns, simply because it simplifies the node names
                        // we use in the XLINQ statement next.
                        xml = xml.Replace(@"xmlns=""http://webservices.amazon.com/AWSECommerceService/2009-03-31""", String.Empty);
                        XDocument xdoc = XDocument.Parse(xml);

                        IEnumerable<Product> productsQuery =
                            from item in xdoc.Descendants("Item")
                            where item.HasElements &&
                                  item.Element("ItemAttributes").HasElements &&
                                  item.Element("ItemAttributes").Element("Author") != null
                            select new Product {
                                ASIN = item.Element("ASIN").Value,
                                ItemUri = new Uri(item.Element("DetailPageURL").Value, UriKind.Absolute),
                                ImageUri = new Uri(item.Element("MediumImage").Element("URL").Value, UriKind.Absolute),
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
