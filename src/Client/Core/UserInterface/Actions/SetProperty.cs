// SetProperty.cs
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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that invokes a method on the DataContext set on the
    /// associated object or a specified target.
    /// </summary>
    [ContentProperty("Value")]
    public sealed class SetProperty : InvokeMemberAction {

        private Binding _valueBinding;
        private BindingShim _binder;
        private string _propertyName;

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
        /// Gets or sets the binding that is used to indicate the value that will be used
        /// to set the property.
        /// </summary>
        public Binding ValueBinding {
            get {
                return _valueBinding;
            }
            set {
                _valueBinding = value;
            }
        }

        /// <internalonly />
        protected override void OnDetach() {
            if (_binder != null) {
                _binder.Dispose();
                _binder = null;
            }
            base.OnDetach();
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            if (String.IsNullOrEmpty(_propertyName)) {
                throw new InvalidOperationException("The PropertyName property must be set on a SetProperty action.");
            }
            if (_valueBinding == null) {
                throw new InvalidOperationException("The ValueBinding property must be set on a SetProperty action.");
            }

            object target = GetTarget();
            if (target == null) {
                throw new InvalidOperationException("There is no target object to set the specified property '" + _propertyName + "'.");
            }

            PropertyInfo targetProperty = target.GetType().GetProperty(_propertyName);
            if (targetProperty == null) {
                throw new InvalidOperationException("The specified property '" + _propertyName + "' was not found on an object of type '" + target.GetType().FullName + "'");
            }

            if (_binder == null) {
                _binder = new BindingShim(AssociatedObject, _valueBinding, null);
            }

            object value = _binder.Value;
            if (targetProperty.PropertyType != typeof(Object)) {
                value = Convert.ChangeType(value, targetProperty.PropertyType, CultureInfo.CurrentCulture);
            }

            targetProperty.SetValue(target, value, null);
        }
    }
}
