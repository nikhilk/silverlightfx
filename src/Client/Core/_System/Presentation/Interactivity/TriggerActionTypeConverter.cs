// TriggerActionTypeConverter.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Converts strings to Action instances. Specifically, a string is always
    /// interpreted as a script statement and corresponding an InvokeScript
    /// action is created.
    /// </summary>
    public sealed class TriggerActionTypeConverter : TypeConverter {

        /// <internalonly />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return (sourceType == typeof(string));
        }

        /// <internalonly />
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return false;
        }

        /// <internalonly />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            string s = value as string;
            if (String.IsNullOrEmpty(s) == false) {
                ScriptTriggerAction scriptAction = ScriptTriggerAction.Parse(s);
                if (scriptAction == null) {
                    throw new InvalidOperationException("Unable to parse the script expression '" + s + "' as a valid script action.");
                }

                return scriptAction;
            }

            return null;
        }

        /// <internalonly />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            return base.ConvertTo(value, destinationType);
        }
    }
}
