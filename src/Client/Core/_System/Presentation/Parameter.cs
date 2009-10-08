// Parameter.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows.Data;

namespace System.Windows {

    /// <summary>
    /// Represents the base class for declarative parameters.
    /// </summary>
    public abstract class Parameter {

        private ParameterCollection _owner;
        private string _parameterName;
        private object _ignoredValue;

        /// <summary>
        /// Gets the element that owns this parameter.
        /// </summary>
        protected FrameworkElement AssociatedElement {
            get {
                if (_owner == null) {
                    return null;
                }
                return _owner.AssociatedElement;
            }
        }

        /// <summary>
        /// Gets or sets the value to be considered as the ignored or not-set
        /// value.
        /// </summary>
        public object IgnoredValue {
            get {
                return _ignoredValue;
            }
            set {
                _ignoredValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the paramter.
        /// </summary>
        public string ParameterName {
            get {
                return _parameterName ?? String.Empty;
            }
            set {
                if (_owner != null) {
                    throw new InvalidOperationException("ParameterName can only be set before it is added to a ParameterCollection.");
                }
                _parameterName = value;
            }
        }

        /// <summary>
        /// Activates the parameter with the specified element as visual tree context.
        /// </summary>
        protected abstract void Activate();

        /// <summary>
        /// Deactivates a parameter, when it is no longer in use.
        /// </summary>
        protected abstract void Deactivate();

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        /// <returns>The value of the parameter.</returns>
        public abstract object GetValue();

        /// <summary>
        /// Notifies that this parameter's value has changed.
        /// </summary>
        protected void OnValueChanged() {
            if (_owner != null) {
                _owner.OnParameterChanged(this);
            }
        }

        internal void SetOwner(ParameterCollection owner) {
            if (owner != null) {
                _owner = owner;
                Activate();
            }
            else {
                Deactivate();
                _owner = null;
            }
        }
    }
}
