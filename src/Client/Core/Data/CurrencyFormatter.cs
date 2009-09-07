// CurrencyFormatter.cs
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
    /// a formatted string representation for a currency value.
    /// 
    /// The ConverterParameter can be used to specify a custom format string.
    /// </summary>
    public sealed class CurrencyFormatter : IValueConverter {

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(string)) {
                throw new ArgumentOutOfRangeException("targetType", "CurrencyFormatter can only convert to String.");
            }

            decimal d = Convert.ToDecimal(value);
            if (d == 0m) {
                return String.Empty;
            }

            string format = parameter as string;
            if (String.IsNullOrEmpty(format)) {
                format = "{0:C}";
            }

            return String.Format(culture, format, value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(decimal)) {
                throw new ArgumentOutOfRangeException("targetType", "CurrencyFormatter can only convert back to Decimal");
            }

            if (value == null) {
                return 0m;
            }

            if (!(value is string)) {
                throw new ArgumentException("value", "CurrencyFormatter can only converter back from String.");
            }

            string s = (string)value;
            string prefix = parameter as string;
            if ((String.IsNullOrEmpty(prefix) == false) && s.StartsWith(prefix)) {
                s = s.Substring(prefix.Length);
            }
            if (s.StartsWith(culture.NumberFormat.CurrencySymbol)) {
                s = s.Substring(culture.NumberFormat.CurrencySymbol.Length);
            }
            if (s.Length == 0) {
                return 0m;
            }

            decimal convertedValue;
            if (Decimal.TryParse(s, out convertedValue)) {
                return convertedValue;
            }

            throw new ArgumentException("Invalid currency value", "value");
        }
        #endregion
    }
}
