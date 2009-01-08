// TextFilter.cs
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
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with the TextBox control to filter input.
    /// </summary>
    public class TextFilter : Behavior<TextBox> {

        private TextFilterType _filter;

        /// <summary>
        /// Initializes an instance of a TextFilter behavior.
        /// </summary>
        public TextFilter() {
        }

        /// <summary>
        /// The type of filter applied to the textbox.
        /// </summary>
        public TextFilterType Filter {
            get {
                return _filter;
            }
            set {
                if ((value < TextFilterType.None) || (value > TextFilterType.Alphabets)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _filter = value;
            }
        }

        /// <internalonly />
        protected override void OnAttach() {
            AssociatedObject.KeyDown += OnTextBoxKeyDown;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.KeyDown -= OnTextBoxKeyDown;
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e) {
            if ((e.Key < Key.D0) ||
                ((e.Key > Key.Z) && (e.Key < Key.Multiply))) {
                return;
            }

            if (_filter == TextFilterType.Numbers) {
                if (e.Key > Key.D9) {
                    e.Handled = true;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) {
                    e.Handled = true;
                }
            }
            else if (_filter == TextFilterType.Alphabets) {
                if (e.Key < Key.A) {
                    e.Handled = true;
                }
            }
        }
    }
}
