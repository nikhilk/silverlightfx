// UriFormatter.cs
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
    /// a formatted URI representation.
    /// </summary>
    public sealed class UriFormatter : Formatter {

        /// <internalonly />
        protected override object Format(object value, string format, CultureInfo culture) {
            string url = (string)base.Format(value, format, culture);
            return new Uri(url, UriKind.RelativeOrAbsolute);
        }
    }
}
