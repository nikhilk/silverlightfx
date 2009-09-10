// ScriptExpression.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel {

    /// <summary>
    /// Represents a simple script-like expression consisting of at most one assignment expression (optional),
    /// and at most one method expression (also optional), whose parameters are simple property.subproperty paths or
    /// a single property.subproperty lookup.
    /// </summary>
    public sealed class ScriptExpression {

        private string _script;
        private IValueExpression _rhs;
        private IValueExpression _lhs;

        private ScriptExpression(string script, IValueExpression rhs) {
            _script = script;
            _rhs = rhs;
        }

        private ScriptExpression(string script, IValueExpression rhs, IValueExpression lhs) {
            _script = script;
            _rhs = rhs;
            _lhs = lhs;
        }

        /// <summary>
        /// Gets the original text used to create this ScriptExpression.
        /// </summary>
        public string Expression {
            get {
                return _script;
            }
        }

        /// <summary>
        /// Executes a script expression.
        /// </summary>
        /// <param name="nameResolver">An object that can resolve names in the script expression to object instances.</param>
        /// <returns>The resulting value from executing the expression if any.</returns>
        public object Execute(IScriptExpressionNameResolver nameResolver) {
            object value = _rhs.GetValue(nameResolver);
            if (_lhs != null) {
                ((PropertyAccessExpression)_lhs).SetValue(nameResolver, value);
            }

            return value;
        }

        private static bool IsValidIdentifier(string name) {
            if (Char.IsDigit(name[0])) {
                return false;
            }
            for (int i = 0; i < name.Length; i++) {
                char ch = name[i];
                if ((ch != '_') && (ch != '$') && (Char.IsLetterOrDigit(ch) == false)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Parses text representing a script expression into a ScriptExpression instance.
        /// </summary>
        /// <param name="expression">The text to parse.</param>
        /// <returns>The parsed script expression.</returns>
        public static ScriptExpression Parse(string expression) {
            if (String.IsNullOrEmpty(expression)) {
                throw new ArgumentNullException("expression");
            }

            string[] scriptParts = expression.Split('=');
            if ((scriptParts != null) && (scriptParts.Length == 1)) {
                IValueExpression rhs = ParseExpression(scriptParts[0].Trim(), /* allowMethod */ true);
                if (rhs != null) {
                    return new ScriptExpression(expression, rhs);
                }
            }
            else if (scriptParts.Length == 2) {
                IValueExpression rhs = ParseExpression(scriptParts[1].Trim(), /* allowMethod */ true);
                IValueExpression lhs = ParseExpression(scriptParts[0].Trim(), /* allowMethod */ false);

                if ((rhs != null) && (lhs != null) && (lhs is PropertyAccessExpression)) {
                    if (((PropertyAccessExpression)lhs).IsValidSetterExpression) {
                        return new ScriptExpression(expression, rhs, lhs);
                    }
                }
            }

            return null;
        }

        private static IValueExpression ParseExpression(string expression, bool allowMethod) {
            if (allowMethod && (expression.IndexOf("(", StringComparison.Ordinal) > 0)) {
                if (expression.EndsWith(")", StringComparison.Ordinal)) {
                    return ParseMethodCall(expression);
                }
                return null;
            }

            if (expression.StartsWith("'", StringComparison.Ordinal) &&
                expression.EndsWith("'", StringComparison.Ordinal)) {
                return new LiteralExpression(expression.Substring(1, expression.Length - 1));
            }

            bool boolValue;
            if (Boolean.TryParse(expression, out boolValue)) {
                return new LiteralExpression(boolValue);
            }

            int intValue;
            if (Int32.TryParse(expression, out intValue)) {
                return new LiteralExpression(intValue);
            }

            double doubleValue;
            if (Double.TryParse(expression, out doubleValue)) {
                return new LiteralExpression(doubleValue);
            }

            return ParsePropertyAccess(expression);
        }

        private static IValueExpression ParseMethodCall(string expression) {
            MethodCallExpression methodCall = null;

            int openParenIndex = expression.IndexOf("(", StringComparison.Ordinal);
            string memberExpression = expression.Substring(0, openParenIndex);

            string[] pathParts = ParseParts(memberExpression);
            if ((pathParts != null) && (pathParts.Length >= 2)) {
                methodCall = new MethodCallExpression(pathParts);
            }
            else {
                return null;
            }

            int paramsLength = expression.Length - openParenIndex - 2;
            if (paramsLength != 0) {
                string paramExpression = expression.Substring(openParenIndex + 1, paramsLength);
                string[] paramExprList = paramExpression.Split(',');

                for (int i = 0; i < paramExprList.Length; i++) {
                    IValueExpression expr = ParseExpression(paramExprList[i].Trim(), /* allowMethod */ false);
                    if (expr != null) {
                        methodCall.AddParameter(expr);
                    }
                    else {
                        return null;
                    }
                }
            }

            return methodCall;
        }

        private static string[] ParseParts(string expression) {
            string[] pathParts = expression.Split('.');

            if ((pathParts != null) && (pathParts.Length != 0)) {
                foreach (string part in pathParts) {
                    if (IsValidIdentifier(part) == false) {
                        return null;
                    }
                }

                return pathParts;
            }
            return null;
        }

        private static PropertyAccessExpression ParsePropertyAccess(string expression) {
            string[] pathParts = ParseParts(expression);

            if (pathParts != null) {
                return new PropertyAccessExpression(pathParts);
            }

            return null;
        }


        private interface IValueExpression {

            object GetValue(IScriptExpressionNameResolver nameResolver);
        }

        private sealed class LiteralExpression : IValueExpression {

            private object _value;

            public LiteralExpression(object value) {
                _value = value;
            }

            public object GetValue(IScriptExpressionNameResolver nameResolver) {
                return _value;
            }
        }

        private sealed class PropertyAccessExpression : IValueExpression {

            private string[] _parts;

            public PropertyAccessExpression(string[] parts) {
                _parts = parts;
            }

            public bool IsValidSetterExpression {
                get {
                    return (_parts.Length > 1);
                }
            }

            public object GetValue(IScriptExpressionNameResolver nameResolver) {
                object value = nameResolver.ResolveName(_parts[0]);

                if (_parts.Length != 1) {
                    BindingFlags flags = BindingFlags.FlattenHierarchy |
                                         BindingFlags.Public |
                                         BindingFlags.Instance;

                    for (int i = 1; i < _parts.Length; i++) {
                        PropertyInfo pi = value.GetType().GetProperty(_parts[i], flags);
                        if (pi != null) {
                            value = pi.GetValue(value, null);
                        }
                        else {
                            throw new InvalidOperationException("Unknown property '" + _parts[i] + "'");
                        }
                    }
                }

                return value;
            }

            public void SetValue(IScriptExpressionNameResolver nameResolver, object value) {
                BindingFlags flags = BindingFlags.FlattenHierarchy |
                                     BindingFlags.Public |
                                     BindingFlags.Instance;

                object obj = nameResolver.ResolveName(_parts[0]);

                for (int i = 1; i < _parts.Length; i++) {
                    PropertyInfo pi = obj.GetType().GetProperty(_parts[i], flags);
                    if (pi != null) {
                        if (i == _parts.Length - 1) {
                            if ((value == null) || (pi.PropertyType.IsAssignableFrom(value.GetType()) == false)) {
                                value = Convert.ChangeType(value, pi.PropertyType, CultureInfo.CurrentCulture);
                            }

                            pi.SetValue(obj, value, null);
                        }
                        else {
                            obj = pi.GetValue(obj, null);
                        }
                    }
                    else {
                        throw new InvalidOperationException("Unknown property '" + _parts[i] + "'");
                    }
                }
            }
        }

        private sealed class MethodCallExpression : IValueExpression {

            private List<IValueExpression> _parameters;
            private string[] _parts;

            public MethodCallExpression(string[] parts) {
                _parts = parts;
            }

            public void AddParameter(IValueExpression paramExpr) {
                if (_parameters == null) {
                    _parameters = new List<IValueExpression>();
                }
                _parameters.Add(paramExpr);
            }

            public object GetValue(IScriptExpressionNameResolver nameResolver) {
                object value = null;

                BindingFlags flags = BindingFlags.FlattenHierarchy |
                                     BindingFlags.Public |
                                     BindingFlags.Instance;

                object obj = nameResolver.ResolveName(_parts[0]);

                for (int i = 1; i < _parts.Length; i++) {
                    if (i == _parts.Length - 1) {
                        MethodInfo mi = obj.GetType().GetMethod(_parts[i], flags);
                        if (mi == null) {
                            throw new InvalidOperationException("Unknown method '" + _parts[i] + "'");
                        }

                        object[] paramValues = null;

                        if (_parameters != null) {
                            ParameterInfo[] paramInfos = mi.GetParameters();
                            if (_parameters.Count != paramInfos.Length) {
                                throw new InvalidOperationException("Parameter count mismatch for method '" + _parts[i] + "'");
                            }

                            paramValues = new object[_parameters.Count];

                            for (int j = 0; j < paramValues.Length; j++) {
                                object paramValue = _parameters[j].GetValue(nameResolver);
                                if ((paramValue == null) || (paramInfos[j].ParameterType.IsAssignableFrom(paramValue.GetType()) == false)) {
                                    paramValue = Convert.ChangeType(paramValue, paramInfos[j].ParameterType, CultureInfo.CurrentCulture);
                                }

                                paramValues[j] = paramValue;
                            }
                        }

                        value = mi.Invoke(obj, paramValues);
                    }
                    else {
                        PropertyInfo pi = obj.GetType().GetProperty(_parts[i], flags);
                        if (pi != null) {
                            obj = pi.GetValue(obj, null);
                        }
                        else {
                            throw new InvalidOperationException("Unknown property '" + _parts[i] + "'");
                        }
                    }
                }

                return value;
            }
        }
    }
}
