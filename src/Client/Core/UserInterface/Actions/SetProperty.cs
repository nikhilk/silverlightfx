// SetProperty.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that invokes a method on the DataContext set on the
    /// associated object or a specified target.
    /// </summary>
    [ContentProperty("Value")]
    public sealed class SetProperty : InvokeMemberAction {

        private string _propertyName;
        private Parameter _value;

        /// <summary>
        /// Gets or sets the name of the property to set when this action is triggered.
        /// </summary>
        public string PropertyName {
            get {
                return _propertyName;
            }
            set {
                _propertyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Value as an ActionParameter that will supply the actual value to set.
        /// </summary>
        public Parameter Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            if (String.IsNullOrEmpty(_propertyName)) {
                throw new InvalidOperationException("The PropertyName property must be set on a SetProperty action.");
            }

            object target = GetTarget();
            if (target == null) {
                throw new InvalidOperationException("There is no target object to set the specified property '" + _propertyName + "'.");
            }

            PropertyInfo targetProperty = target.GetType().GetProperty(_propertyName);
            if (targetProperty == null) {
                throw new InvalidOperationException("The specified property '" + _propertyName + "' was not found on an object of type '" + target.GetType().FullName + "'");
            }

            object value = AssociatedObject.DataContext;
            if (_value != null) {
                value = _value.GetValue(AssociatedObject);
            }
            if (targetProperty.PropertyType != typeof(Object)) {
                value = Convert.ChangeType(value, targetProperty.PropertyType, CultureInfo.CurrentCulture);
            }

            targetProperty.SetValue(target, value, null);
        }
    }
}
