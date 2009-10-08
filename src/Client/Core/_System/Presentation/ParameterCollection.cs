// ParameterCollection.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;

namespace System.Windows {

    /// <summary>
    /// Represents a collection of Parameter objects.
    /// </summary>
    public sealed class ParameterCollection : ObservableCollection<Parameter> {

        private FrameworkElement _owner;
        private Dictionary<string, Parameter> _parameterMap;
        private EventHandler _parametersChangedHandler;

        internal FrameworkElement AssociatedElement {
            get {
                return _owner;
            }
        }

        /// <summary>
        /// Raised when the list of parameters changes or the value of a parameter has changed.
        /// </summary>
        public event EventHandler ParametersChanged {
            add {
                _parametersChangedHandler = (EventHandler)Delegate.Combine(_parametersChangedHandler, value);
            }
            remove {
                _parametersChangedHandler = (EventHandler)Delegate.Remove(_parametersChangedHandler, value);
            }
        }

        /// <summary>
        /// Gets the list of parameter values corresponding to the specified method signature.
        /// </summary>
        /// <param name="method">The method whose parameters are to be looked up.</param>
        /// <returns>The list of parameter values for the specified method.</returns>
        public object[] GetParameterValues(MethodInfo method) {
            bool dummyHasIgnoredValues;
            int dummyOutParameterIndex;
            return GetParameterValues(method, /* honorIgnoreValues */ false,
                                      out dummyHasIgnoredValues, out dummyOutParameterIndex);
        }

        /// <summary>
        /// Gets the list of parameter values corresponding to the specified method signature.
        /// </summary>
        /// <param name="method">The method whose parameters are to be looked up.</param>
        /// <param name="honorIgnoreValues">Whether to compare values against the Parameter's IgnoredValue property.</param>
        /// <param name="hasIgnoredValues">Whether any of the returned parameter values matches an ignored value.</param>
        /// <param name="outParameterIndex">The index of the last out parameter if one exists.</param>
        /// <returns>The list of parameter values for the specified method.</returns>
        public object[] GetParameterValues(MethodInfo method, bool honorIgnoreValues, out bool hasIgnoredValues, out int outParameterIndex) {
            hasIgnoredValues = false;
            outParameterIndex = -1;

            if (_parameterMap == null) {
                // Not yet initialized...
                return null;
            }

            ParameterInfo[] parameterInfos = method.GetParameters();
            if (parameterInfos.Length != Count) {
                return null;
            }

            List<object> parameterValuesList = new List<object>();
            foreach (ParameterInfo pi in parameterInfos) {
                if (pi.IsOut) {
                    parameterValuesList.Add(null);
                    outParameterIndex = parameterValuesList.Count - 1;
                    continue;
                }

                Parameter parameterObject = _parameterMap[pi.Name];
                if (parameterObject == null) {
                    return null;
                }

                object value = parameterObject.GetValue();
                value = Convert.ChangeType(value, pi.ParameterType, CultureInfo.CurrentCulture);

                if (honorIgnoreValues && (hasIgnoredValues == false) &&
                    (parameterObject.IgnoredValue != null)) {
                    object ignoredValue = Convert.ChangeType(parameterObject.IgnoredValue, pi.ParameterType, CultureInfo.CurrentCulture);

                    if (Object.Equals(value, ignoredValue)) {
                        hasIgnoredValues = true;
                    }
                }

                parameterValuesList.Add(value);
            }

            return parameterValuesList.ToArray();
        }

        /// <summary>
        /// Initializes a parameter collection with the associated owner element as its context.
        /// </summary>
        /// <param name="element">The element to use to find related objects in the visual tree.</param>
        public void Initialize(FrameworkElement element) {
            _owner = element;
            _parameterMap = new Dictionary<string, Parameter>();

            foreach (Parameter p in this) {
                OnParameterAdded(p);
            }
        }

        /// <internalonly />
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            base.OnCollectionChanged(e);

            if (_owner != null) {
                if (e.Action == NotifyCollectionChangedAction.Add) {
                    foreach (Parameter p in e.NewItems) {
                        OnParameterAdded(p);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove) {
                    foreach (Parameter p in e.OldItems) {
                        OnParameterRemoved(p);
                    }
                }

                if (_parametersChangedHandler != null) {
                    _parametersChangedHandler(this, EventArgs.Empty);
                }
            }
        }

        private void OnParameterAdded(Parameter parameter) {
            string name = parameter.ParameterName;
            if (String.IsNullOrEmpty(name)) {
                throw new InvalidOperationException("A parameter must have its Name property set.");
            }
            if (_parameterMap.ContainsKey(name)) {
                throw new InvalidOperationException("A parameter with the name '" + name + "' already exists in the collection.");
            }

            _parameterMap[name] = parameter;
            parameter.SetOwner(this);
        }

        internal void OnParameterChanged(Parameter parameter) {
            if (_parametersChangedHandler != null) {
                _parametersChangedHandler(this, EventArgs.Empty);
            }
        }

        private void OnParameterRemoved(Parameter parameter) {
            string name = parameter.ParameterName;
            if ((String.IsNullOrEmpty(name) == false) && _parameterMap.ContainsKey(name)) {
                _parameterMap.Remove(name);
            }

            parameter.SetOwner(null);
        }
    }
}
