// Watermark.cs
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
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with the TextBox control to add an in-place
    /// prompt.
    /// </summary>
    public class Watermark : Behavior<XTextBox> {

        private string _promptText;
        private Style _promptStyle;
        private TextBlock _prompt;

        private bool _hasFocus;

        /// <summary>
        /// Initializes an instance of a Watermark behavior.
        /// </summary>
        public Watermark() {
        }

        /// <summary>
        /// The style to apply to the prompt.
        /// </summary>
        public Style PromptStyle {
            get {
                return _promptStyle;
            }
            set {
                _promptStyle = value;
            }
        }

        /// <summary>
        /// The text to display as a prompt.
        /// </summary>
        public string PromptText {
            get {
                return _promptText;
            }
            set {
                _promptText = value;
            }
        }

        private void EnsurePrompt() {
            if (_prompt == null) {
                _prompt = new TextBlock();
                if (_promptStyle != null) {
                    _prompt.Style = _promptStyle;
                }
            }

            _prompt.Text = _promptText;
        }

        private void HideWatermark() {
            if (_prompt != null) {
                AssociatedObject.RemoveAdornment(_prompt);
            }
        }

        /// <internalonly />
        protected override void OnAttach() {
            AssociatedObject.Loaded += OnTextBoxLoad;
            AssociatedObject.TextChanged += OnTextBoxTextChanged;
            AssociatedObject.GotFocus += OnTextBoxGotFocus;
            AssociatedObject.LostFocus += OnTextBoxLostFocus;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.Loaded -= OnTextBoxLoad;
            AssociatedObject.TextChanged -= OnTextBoxTextChanged;
            AssociatedObject.GotFocus -= OnTextBoxGotFocus;
            AssociatedObject.LostFocus -= OnTextBoxLostFocus;
        }

        private void OnTextBoxGotFocus(object sender, EventArgs e) {
            _hasFocus = true;
            HideWatermark();
        }

        private void OnTextBoxLoad(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(delegate() {
                if (AssociatedObject.Text.Length == 0) {
                    ShowWatermark();
                }
            });
        }

        private void OnTextBoxLostFocus(object sender, EventArgs e) {
            _hasFocus = false;
            ShowWatermark();
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e) {
            if (_hasFocus == false) {
                UpdateWatermark();
            }
        }

        private void ShowWatermark() {
            if ((AssociatedObject.Text.Length == 0) && AssociatedObject.SupportsAdornments) {
                EnsurePrompt();
                AssociatedObject.AddAdornment(_prompt);
            }
        }

        private void UpdateWatermark() {
            if (AssociatedObject.Text.Length == 0) {
                ShowWatermark();
            }
            else {
                HideWatermark();
            }
        }
    }
}
