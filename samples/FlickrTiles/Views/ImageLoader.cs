// ImageLoader.cs
//

using System;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlickrTiles.Views {

    internal static class ImageLoader {

        public static void LoadImage(Uri uri, Action<ImageSource, object> imageCallback, object userContext) {
            WebClient imageLoader = new WebClient();
            imageLoader.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e) {
                try {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(e.Result);

                    imageCallback(bitmapImage, userContext);
                }
                catch {
                    imageCallback(null, userContext);
                }
            };
            imageLoader.OpenReadAsync(uri);
        }
    }
}
