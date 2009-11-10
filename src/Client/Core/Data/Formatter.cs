// Formatter.cs
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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SilverlightFX.Data {

    /// <summary>
    /// A base class for formatting value converters.
    /// </summary>
    public abstract class Formatter : IValueConverter {

        private FormatType _formatType;
        private Type _supportedTargetType;

        /// <summary>
        /// Initializes an instance of a Formatter.
        /// </summary>
        protected Formatter()
            : this(typeof(string)) {
        }

        /// <summary>
        /// Initializes an instance of a Formatter with the specified supported target type.
        /// </summary>
        /// <param name="supportedTargetType">The type of value that the formatter converts values into.</param>
        protected Formatter(Type supportedTargetType) {
            _supportedTargetType = supportedTargetType;
        }

        /// <summary>
        /// Gets or sets the type of formatting to be done by the formatter. The default
        /// is to use a format with positional tokens.
        /// </summary>
        public FormatType FormatType {
            get {
                return _formatType;
            }
            set {
                _formatType = value;
            }
        }

        /// <summary>
        /// Formats the specified value using the specified format. The base class
        /// implementation simply performs a String.Format. Derived classes can choose
        /// to modify the value before it is formatted, or use the formatted value
        /// after calling into the base implementation.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <param name="format">The format string to use.</param>
        /// <param name="culture">The culture to use when formatting.</param>
        /// <returns>The formatted value.</returns>
        protected virtual object Format(object value, string format, CultureInfo culture) {
            if (FormatType == FormatType.NumberedTokens) {
                return String.Format(culture, format, value);
            }
            else {
                return NamedTokensFormatter.Format(culture, format, value);
            }
        }

        #region Implementation of IValueConverter
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != _supportedTargetType) {
                throw new ArgumentOutOfRangeException("targetType", GetType().Name + " can only convert to " + _supportedTargetType.Name);
            }

            if (value == null) {
                return null;
            }

            string format = (string)parameter;
            if (String.IsNullOrEmpty(format)) {
                format = "{0}";
            }

            return Format(value, format, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
        #endregion


        private static class NamedTokensFormatter {

            public static string Format(CultureInfo culture, string format, object value) {
                if (String.IsNullOrEmpty(format)) {
                    throw new ArgumentNullException("format");
                }

                IEnumerable<string> formattedStrings =
                    from expression in SplitFormat(format)
                    select expression.Format(value, culture);
                return String.Join(String.Empty, formattedStrings.ToArray());
            }

            private static IEnumerable<IString> SplitFormat(string format) {
                int exprEndIndex = -1;
                int expStartIndex;

                do {
                    expStartIndex = IndexOfExpressionStart(format, exprEndIndex + 1);
                    if (expStartIndex < 0) {
                        // Everything after last end brace index.
                        if (exprEndIndex + 1 < format.Length) {
                            yield return new LiteralText(format.Substring(exprEndIndex + 1));
                        }
                        break;
                    }

                    if (expStartIndex - exprEndIndex - 1 > 0) {
                        // Everything up to next start brace index
                        yield return new LiteralText(format.Substring(exprEndIndex + 1, expStartIndex - exprEndIndex - 1));
                    }

                    int endBraceIndex = IndexOfExpressionEnd(format, expStartIndex + 1);
                    if (endBraceIndex < 0) {
                        // Rest of string, no end brace (could be invalid expression)
                        yield return new FormattedText(format.Substring(expStartIndex));
                    }
                    else {
                        exprEndIndex = endBraceIndex;
                        // Everything from start to end brace.
                        yield return new FormattedText(format.Substring(expStartIndex, endBraceIndex - expStartIndex + 1));

                    }
                } while (expStartIndex > -1);
            }

            private static int IndexOfExpressionStart(string format, int startIndex) {
                int index = format.IndexOf('{', startIndex);
                if (index == -1) {
                    return index;
                }

                // Peek ahead.
                if (index + 1 < format.Length) {
                    char nextChar = format[index + 1];
                    if (nextChar == '{') {
                        return IndexOfExpressionStart(format, index + 2);
                    }
                }

                return index;
            }

            private static int IndexOfExpressionEnd(string format, int startIndex) {
                int endBraceIndex = format.IndexOf('}', startIndex);
                if (endBraceIndex == -1) {
                    return endBraceIndex;
                }

                // Start peeking ahead until there are no more braces...
                int braceCount = 0;
                for (int i = endBraceIndex + 1; i < format.Length; i++) {
                    if (format[i] == '}') {
                        braceCount++;
                    }
                    else {
                        break;
                    }
                }
                if (braceCount % 2 == 1) {
                    return IndexOfExpressionEnd(format, endBraceIndex + braceCount + 1);
                }

                return endBraceIndex;
            }


            private interface IString {
                string Format(object o, CultureInfo culture);
            }

            private sealed class LiteralText : IString {

                private string _literal;

                public LiteralText(string literalText) {
                    _literal = literalText;
                }

                public string Format(object o, CultureInfo culture) {
                    return _literal.Replace("{{", "{").Replace("}}", "}");
                }
            }

            private sealed class FormattedText : IString {

                private string _expression;
                private string _format;

                public FormattedText(string expression) {
                    string expressionWithoutBraces = expression.Substring(1, expression.Length - 2);
                    int colonIndex = expressionWithoutBraces.IndexOf(':');

                    if (colonIndex < 0) {
                        _expression = expressionWithoutBraces;
                        _format = "{0}";
                    }
                    else {
                        _expression = expressionWithoutBraces.Substring(0, colonIndex);
                        _format = expressionWithoutBraces.Substring(colonIndex + 1);
                    }
                }

                public string Format(object o, CultureInfo culture) {
                    try {
                        object value = o;
                        if ((o != null) && (String.CompareOrdinal(_expression, "0") != 0)) {
                            value = o.GetType().GetProperty(_expression).GetValue(o, null);
                        }

                        return String.Format(culture, _format, value);
                    }
                    catch (Exception e) {
                        throw new FormatException("Invalid object or format string", e);
                    }
                }
            }
        }
    }
}
