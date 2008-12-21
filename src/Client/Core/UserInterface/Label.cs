// Label.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

// TODO: Look at automation and see if something interesting can be done once
//       we introduce an AssociatedControlName property on Label.

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A label control that represents some text on a form.
    /// </summary>
    public class Label : Control {

        /// <summary>
        /// Represents the Text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Label), null);

        /// <summary>
        /// Represents the TextAlignment property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(Label), null);

        /// <summary>
        /// Represents the TextDecorations property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(Label), null);

        /// <summary>
        /// Represents the TextWrapping property.
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(Label), null);

        /// <summary>
        /// Initializes an instance of a Label.
        /// </summary>
        public Label() {
            DefaultStyleKey = typeof(Label);
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Gets or sets the text displayed within a Label.
        /// </summary>
        public string Text {
            get {
                return (string)GetValue(TextProperty);
            }
            set {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text within the label.
        /// </summary>
        public TextAlignment TextAlignment {
            get {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }
            set {
                SetValue(TextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the decorations applied to the text within the label.
        /// </summary>
        public TextDecorationCollection TextDecorations {
            get {
                return GetValue(TextDecorationsProperty) as TextDecorationCollection;
            }
            set {
                SetValue(TextDecorationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the wrapping of the text within the label.
        /// </summary>
        public TextWrapping TextWrapping {
            get {
                return (TextWrapping)GetValue(TextWrappingProperty);
            }
            set {
                SetValue(TextWrappingProperty, value);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            ApplyTemplate();
        }
    }
}
