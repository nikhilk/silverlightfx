// BoundParameter.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Data;

namespace SilverlightFX.Data {

    /// <summary>
    /// Represents a parameter whose value is specified using a binding.
    /// </summary>
    public sealed class BoundParameter : Parameter {

        private Binding _valueBinding;
        private BindingShim _binder;

        /// <summary>
        /// Gets or sets the binding used to determine the value of the parameter.
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
        protected override void Activate() {
            if (_valueBinding == null) {
                throw new InvalidOperationException("The ValueBinding property on BoundParameter must be set.");
            }

            _binder = new BindingShim(AssociatedElement, _valueBinding, OnValueBindingChanged);
        }

        /// <internalonly />
        protected override void Deactivate() {
            _binder.Dispose();
            _binder = null;
        }

        /// <internalonly />
        public override object GetValue() {
            return _binder.Value;
        }

        private void OnValueBindingChanged() {
            OnValueChanged();
        }
    }
}
