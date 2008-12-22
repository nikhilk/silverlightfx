// TypeTypeConverter.cs
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

namespace System.ComponentModel {

    /// <summary>
    /// Converts between Type and String.
    /// </summary>
    public sealed class TypeTypeConverter : TypeConverter {

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
                string name = (string)value;

                Type type = ParseTypeName(Application.Current, name);
                if (type != null) {
                    return type;
                }
                throw new ArgumentException("The specified value is not a valid type name.");
            }
            return base.ConvertFrom(value);
        }

        /// <internalonly />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                return ((Type)value).FullName;
            }
            return base.ConvertTo(value, destinationType);
        }

        internal static Type ParseTypeName(Application app, string typeName) {
            if (typeName.IndexOf(",") > 0) {
                return Type.GetType(typeName, /* throwOnError */ false, /* ignoreCase */ false);
            }
            else {
                Type appType = app.GetType();

                if (typeName.IndexOf(".") < 0) {
                    typeName = appType.Namespace + "." + typeName;
                }
                return appType.Assembly.GetType(typeName, /* throwOnError */ false);
            }
        }
    }
}
