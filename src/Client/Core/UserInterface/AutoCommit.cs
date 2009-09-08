// AutoCommit.cs
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
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with TextBox and PasswordBox controls to add
    /// commit on enter keypress semantics.
    /// </summary>
    public class AutoCommit : Behavior<Control> {

        private string _buttonName;

        /// <summary>
        /// Initializes an instance of a TextFilter behavior.
        /// </summary>
        public AutoCommit() {
        }

        /// <summary>
        /// The name of a Button control.
        /// </summary>
        public string ButtonName {
            get {
                return _buttonName;
            }
            set {
                _buttonName = value;
            }
        }

        private bool HasInput {
            get {
                TextBox textBox = AssociatedObject as TextBox;
                if (textBox != null) {
                    return textBox.Text.Length != 0;
                }

                Debug.Assert(AssociatedObject is PasswordBox);
                return ((PasswordBox)AssociatedObject).Password.Length != 0;
            }
        }

        /// <internalonly />
        protected override void OnAttach() {
            if (!(AssociatedObject is TextBox) &&
                !(AssociatedObject is PasswordBox)) {
                throw new InvalidOperationException("AutoCommit can only be associated with TextBox or PasswordBox controls.");
            }
            AssociatedObject.KeyDown += OnTextBoxKeyDown;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.KeyDown -= OnTextBoxKeyDown;
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e) {
            if ((e.Handled == false) && (e.Key == Key.Enter) && HasInput) {
                Button targetButton = null;
                if (String.IsNullOrEmpty(_buttonName) == false) {
                    targetButton = AssociatedObject.FindName(_buttonName) as Button;
                }

                if (targetButton == null) {
                    throw new InvalidOperationException("ButtonName on AutoCommit must be set to a valid Button control name.");
                }

                if (targetButton.IsEnabled) {
                    targetButton.Focus();

                    AssociatedObject.Dispatcher.BeginInvoke(delegate() {
                        XButton xButton = targetButton as XButton;
                        if (xButton != null) {
                            xButton.PerformClick();
                        }
                        else {
                            ButtonAutomationPeer automationPeer = new ButtonAutomationPeer(targetButton);
                            IInvokeProvider invokeProvider = (IInvokeProvider)automationPeer.GetPattern(PatternInterface.Invoke);

                            invokeProvider.Invoke();
                        }
                    });
                }

                e.Handled = true;
            }
        }
    }
}
