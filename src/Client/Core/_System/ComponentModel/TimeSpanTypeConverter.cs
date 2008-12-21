// TimeSpanTypeConverter.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Globalization;

namespace System.ComponentModel {

    /// <summary>
    /// Converts between TimeSpan and String.
    /// </summary>
    public sealed class TimeSpanTypeConverter : TypeConverter {

        /// <internalonly />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return (sourceType == typeof(string));
        }

        /// <internalonly />
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return (destinationType == typeof(string));
        }

        /// <internalonly />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                return TimeSpan.Parse((string)value);
            }
            return base.ConvertFrom(value);
        }

        /// <internalonly />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                return value.ToString();
            }
            return base.ConvertTo(value, destinationType);
        }
    }
}
