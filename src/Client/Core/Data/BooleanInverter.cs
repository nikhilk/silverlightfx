// BooleanInverter.cs
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
    /// A value converter that inverts the value of a boolean property.
    /// </summary>
    public sealed class BooleanInverter : IValueConverter {

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(bool)) {
                throw new ArgumentOutOfRangeException("targetType", "BooleanInverter can only convert to Boolean");
            }

            if (!(value is bool)) {
                throw new ArgumentOutOfRangeException("value", "BooleanInverter can only convert from Boolean");
            }

            return !(bool)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(bool)) {
                throw new ArgumentOutOfRangeException("targetType", "BooleanInverter can only convert to Boolean");
            }

            if (!(value is bool)) {
                throw new ArgumentOutOfRangeException("value", "BooleanInverter can only convert from Boolean");
            }

            return !(bool)value;
        }
        #endregion
    }
}
