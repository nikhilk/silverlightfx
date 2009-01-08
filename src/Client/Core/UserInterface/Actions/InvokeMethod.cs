// InvokeMethod.cs
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
using System.Windows.Markup;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that invokes a method on the DataContext set on the
    /// associated object or a specified target.
    /// </summary>
    [ContentProperty("Parameters")]
    public sealed class InvokeMethod : InvokeMemberAction {

        private string _methodName;
        private ParameterCollection _parameters;

        /// <summary>
        /// Gets or sets the name of the method to invoke when this action is triggered.
        /// </summary>
        public string MethodName {
            get {
                return _methodName;
            }
            set {
                _methodName = value;
            }
        }

        /// <summary>
        /// Gets the collection of parameters associated with the method.
        /// </summary>
        public ParameterCollection Parameters {
            get {
                if (_parameters == null) {
                    _parameters = new ParameterCollection();
                }
                return _parameters;
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            if (String.IsNullOrEmpty(_methodName)) {
                throw new InvalidOperationException("The MethodName property must be set on an InvokeMethod action.");
            }

            object target = GetTarget();
            if (target == null) {
                throw new InvalidOperationException("There is no target object to invoke the specified method '" + _methodName + "'.");
            }

            MethodInfo targetMethod = target.GetType().GetMethod(_methodName);
            if (targetMethod == null) {
                throw new InvalidOperationException("The specified method '" + _methodName + "' was not found on an object of type '" + target.GetType().FullName + "'");
            }

            ParameterInfo[] targetParameters = targetMethod.GetParameters();

            object[] parameters = null;
            if ((_parameters != null) && (_parameters.Count != 0)) {
                parameters = new object[_parameters.Count];

                for (int i = 0; i < _parameters.Count; i++) {
                    object value = _parameters[i].GetValue(AssociatedObject);
                    parameters[i] = Convert.ChangeType(value, targetParameters[i].ParameterType, CultureInfo.CurrentCulture);
                }
            }

            targetMethod.Invoke(target, parameters);
        }
    }
}
