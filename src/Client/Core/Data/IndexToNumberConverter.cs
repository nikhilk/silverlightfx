// IndexToNumberConverter.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
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
    /// A value converter that converts an 0-based index value into a
    /// 1-based numeric value.
    /// </summary>
    public sealed class IndexToNumberConverter : IValueConverter {

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(int)) {
                throw new ArgumentOutOfRangeException("targetType", "IndexToNumberConverter can only convert to an Integer");
            }

            if (!(value is int)) {
                throw new ArgumentOutOfRangeException("value", "IndexToNumberConverter can only convert from an Integer");
            }

            return (int)value + 1;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(int)) {
                throw new ArgumentOutOfRangeException("targetType", "BooleanInverter can only convert to an Integer");
            }

            if (!(value is int)) {
                throw new ArgumentOutOfRangeException("value", "BooleanInverter can only convert from an Integer");
            }

            return (int)value - 1;
        }
        #endregion
    }
}
