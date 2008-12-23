// AutoCommit.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with the TextBox control to add commit semantics.
    /// </summary>
    public class AutoCommit : Behavior<TextBox> {

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

        /// <internalonly />
        protected override void OnAttach() {
            AssociatedObject.KeyDown += OnTextBoxKeyDown;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.KeyDown -= OnTextBoxKeyDown;
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e) {
            if ((e.Handled == false) &&
                (e.Key == Key.Enter) &&
                (AssociatedObject.Text.Length != 0)) {
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
