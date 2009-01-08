// ElementProperty.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Reflection;
using System.Windows;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// An ActionParameter that provides access to a property off a specified element.
    /// </summary>
    public sealed class ElementProperty : Parameter {

        private string _elementName;
        private string _propertyName;

        /// <summary>
        /// Gets or sets the name of the element to retrieve the parameter value.
        /// </summary>
        public string ElementName {
            get {
                return _elementName;
            }
            set {
                _elementName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the property to retrieve the parameter value.
        /// </summary>
        public string PropertyName {
            get {
                return _propertyName;
            }
            set {
                _propertyName = value;
            }
        }

        /// <internalonly />
        public override object GetValue(FrameworkElement element) {
            if (String.IsNullOrEmpty(_propertyName)) {
                throw new InvalidOperationException("The PropertyName on an ElementProperty must be specified.");
            }

            object source = null;

            if (String.IsNullOrEmpty(_elementName)) {
                source = element.DataContext;
            }
            else if (String.CompareOrdinal(_elementName, "$self") == 0) {
                source = element;
            }
            else {
                source = element.FindName(_elementName);
            }

            if (source == null) {
                throw new InvalidOperationException("The ElementProperty could not find the specified element to use as the source of its value.");
            }

            PropertyInfo sourceProperty = source.GetType().GetProperty(_propertyName);
            if (sourceProperty == null) {
                throw new InvalidOperationException("The specified property '" + _propertyName + "' was not found on an object of type '" + source.GetType().FullName + "'");
            }

            return sourceProperty.GetValue(source, null);
        }
    }
}
