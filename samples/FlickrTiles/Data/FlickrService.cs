// FlickrService.cs
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using SilverlightFX.Applications;

namespace FlickrTiles.Data {

    public class FlickrService : IPhotoGallery {

        private const string SearchUrlFormat =
            "http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&tags={1}&tag_mode=all&sort=interestingness-desc&safe_search=1&extras=date_taken&per_page=20";
        private const string PhotoUrlFormat =
            "http://static.flickr.com/{0}/{1}_{2}.jpg";
        private const string ThumbnailUrlFormat =
            "http://static.flickr.com/{0}/{1}_{2}_s.jpg";
        private const string PageUrlFormat =
            "http://www.flickr.com/photos/{0}/{1}";

        public void SearchPhotos(string tag, Action<string, IEnumerable<Photo>> photosCallback) {
            string apiKey = ApplicationContext.Current.StartupArguments["ApiKey"];
            Uri searchUri = new Uri(String.Format(SearchUrlFormat, apiKey, tag));

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                if ((e.Cancelled == false) && (e.Error == null)) {
                    string xml = e.Result;

                    if (String.IsNullOrEmpty(xml) == false) {
                        XDocument xdoc = XDocument.Parse(e.Result);
                        var photosQuery =
                            from photo in xdoc.Descendants("photo")
                            select new Photo {
                                Title = photo.Attribute("title").Value,
                                ThumbnailUri = new Uri(String.Format(ThumbnailUrlFormat,
                                                                     photo.Attribute("server").Value,
                                                                     photo.Attribute("id").Value,
                                                                     photo.Attribute("secret").Value),
                                                       UriKind.Absolute),
                                PhotoUri = new Uri(String.Format(PhotoUrlFormat,
                                                                 photo.Attribute("server").Value,
                                                                 photo.Attribute("id").Value,
                                                                 photo.Attribute("secret").Value),
                                                    UriKind.Absolute),
                                PageUri = new Uri(String.Format(PageUrlFormat,
                                                                photo.Attribute("owner").Value,
                                                                photo.Attribute("id").Value),
                                                  UriKind.Absolute),
                                ShotOn = DateTime.SpecifyKind(DateTime.Parse(photo.Attribute("datetaken").Value), DateTimeKind.Utc)
                              };

                        Photo[] photos = photosQuery.ToArray();
                        photosCallback(tag, photos);

                        return;
                    }

                    photosCallback(tag, null);
                }
            };

            webClient.DownloadStringAsync(searchUri);
        }
    }
}
