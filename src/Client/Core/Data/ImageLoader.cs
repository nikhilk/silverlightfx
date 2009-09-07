// ImageLoader.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightFX.Data {

    /// <summary>
    /// A value converter that can be used in a binding that loads an image
    /// to convert a string or Uri to an ImageSource.
    /// </summary>
    public sealed class ImageLoader : IValueConverter {

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(ImageSource)) {
                throw new ArgumentOutOfRangeException("targetType", "ImageLoader can only convert to ImageSource");
            }

            Uri imageUri = value as Uri;
            if (imageUri == null) {
                string url = value as string;
                if (url == null) {
                    throw new ArgumentOutOfRangeException("The value must either be an absolute Uri or a String.");
                }

                imageUri = new Uri(url, UriKind.Absolute);
            }

            return new BitmapImage(imageUri);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
        #endregion
    }
}
