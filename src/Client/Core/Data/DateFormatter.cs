// DateFormatter.cs
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
    /// a formatted representation of a DateTime or a part of a DateTime.
    /// A UTC DateTime is converted to a local DateTime in the process.
    /// </summary>
    public sealed class DateFormatter : Formatter {

        private bool _preserveUtcValues;

        /// <summary>
        /// Gets or sets whether to use UTC values in formatting or
        /// Local values. By default UTC values are first converted to local values.
        /// </summary>
        public bool PreserveUtcValues {
            get {
                return _preserveUtcValues;
            }
            set {
                _preserveUtcValues = value;
            }
        }

        /// <internalonly />
        protected override object Format(object value, string format, CultureInfo culture) {
            DateTime dateTime = (DateTime)value;

            if ((PreserveUtcValues == false) && (dateTime.Kind == DateTimeKind.Utc)) {
                dateTime = dateTime.ToLocalTime();
            }

            return base.Format(dateTime, format, culture);
        }
    }
}
