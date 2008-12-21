// StringFormatter.cs
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

namespace SilverlightFX.Data {

    /// <summary>
    /// A value converter that can be used in a binding to generate
    /// a formatted string representation.
    /// </summary>
    public sealed class StringFormatter : IValueConverter {

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(string)) {
                throw new ArgumentOutOfRangeException("targetType", "StringFormatter can only convert to String");
            }

            if (value == null) {
                return String.Empty;
            }

            string format = parameter as string;
            if (String.IsNullOrEmpty(format) == false) {
                return String.Format(culture, format, value);
            }

            return String.Format(culture, "{0}", value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
