// ScriptTriggerAction.cs
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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Represents an action that contains a simple script expression.
    /// </summary>
    public class ScriptTriggerAction : TriggerAction<FrameworkElement>, IScriptExpressionNameResolver {

        private ScriptExpression _script;
        private EventArgs _eventArgs;

        /// <summary>
        /// Initializes an instance of a ScriptTriggerAction.
        /// </summary>
        protected ScriptTriggerAction() {
        }

        private ScriptTriggerAction(ScriptExpression script) {
            _script = script;
        }

        /// <summary>
        /// Gets or sets the script expression to evaluate.
        /// </summary>
        public string Expression {
            get {
                if (_script != null) {
                    return _script.Expression;
                }
                return String.Empty;
            }
            set {
                ScriptExpression script = ScriptExpression.Parse(value);
                if (script == null) {
                    throw new ArgumentException("value");
                }

                _script = script;
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            if (_script != null) {
                _eventArgs = e;
                _script.Execute(this);
            }
        }

        internal static ScriptTriggerAction Parse(string expression) {
            ScriptExpression script = ScriptExpression.Parse(expression);
            if (script != null) {
                return new ScriptTriggerAction(script);
            }

            return null;
        }

        #region Implementation of IScriptExpressionNameResolver
        object IScriptExpressionNameResolver.ResolveName(string name) {
            object value = null;

            if (name == "$dataContext") {
                return AssociatedObject.DataContext;
            }
            else if (name == "$model") {
                return ViewModelAttribute.GetCurrentViewModel(AssociatedObject);
            }
            else if (name == "$element") {
                return AssociatedObject;
            }
            else if (name == "$event") {
                return _eventArgs;
            }
            else {
                // First see if the name exists as a public property on the associated
                // object
                BindingFlags flags = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
                PropertyInfo pi = AssociatedObject.GetType().GetProperty(name, flags);
                if (pi != null) {
                    return pi.GetValue(AssociatedObject, null);
                }

                // Lookup a named object in XAML
                value = AssociatedObject.FindNameRecursive(name);
                if (value != null) {
                    return value;
                }

                // Lookup a resource
                value = AssociatedObject.FindResource(name);
                if (value != null) {
                    return value;
                }
            }

            return null;
        }
        #endregion
    }
}
