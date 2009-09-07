// StringTrimmer.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SilverlightFX.Data {

    /// <summary>
    /// A value converter that trims and optionally truncates strings.
    /// 
    /// If the ConverterParameter is set to a number the string is truncated so it has
    /// at most the specified length, and trailing ellipsis are appended.
    /// </summary>
    public sealed class StringTrimmer : IValueConverter {

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(string)) {
                throw new ArgumentOutOfRangeException("targetType", "StringTrimmer can only convert to String");
            }

            String s = value as string;
            if (String.IsNullOrEmpty(s)) {
                return String.Empty;
            }

            s = s.Trim();

            if (parameter != null) {
                int maxLength;
                if (Int32.TryParse(parameter.ToString(), out maxLength) &&
                    (s.Length > maxLength)) {
                    s = s.Substring(0, maxLength) + "...";
                }
            }

            return s;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
        #endregion
    }
}
