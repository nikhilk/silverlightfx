// Command.cs
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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with a Button to invoke a command.
    /// </summary>
    public sealed class Command : Behavior<ButtonBase> {

        private string _commandTarget;
        private string _commandName;
        private object _commandParameter;

        private ICommand _command;
        private bool _unknownCommand;

        /// <summary>
        /// Gets or sets the name of the command to invoke.
        /// </summary>
        public string CommandName {
            get {
                return _commandName;
            }
            set {
                _commandName = value;
            }
        }

        /// <summary>
        /// Gets or sets the object to use as the parameter when invoking
        /// the command.
        /// </summary>
        public object CommandParameter {
            get {
                return _commandParameter;
            }
            set {
                _commandParameter = value;
                UpdateAssociatedObject();
            }
        }

        /// <summary>
        /// Gets or sets the object that surfaces the command to be invoke.
        /// By default this is empty, which is used to select the object with which
        /// this behavior is associated.
        /// </summary>
        public string CommandTarget {
            get {
                return _commandTarget;
            }
            set {
                _commandTarget = value;
            }
        }

        private ICommand GetCommand() {
            FrameworkElement target = AssociatedObject;
            if (String.IsNullOrEmpty(_commandTarget) == false) {
                target = target.FindNameRecursive(_commandTarget) as FrameworkElement;
                if (target == null) {
                    throw new InvalidOperationException("Could not find command target.");
                }
            }

            ICommand command = target.FindResource(_commandName + "Command") as ICommand;
            if (command == null) {
                command = target.FindResource(_commandName) as ICommand;
            }

            if (command == null) {
                // Since there was no static command available, find a command provider
                // in the visual tree.

                FrameworkElement element = target.Parent as FrameworkElement;
                while (element != null) {
                    ICommandContainer commandProvider = element as ICommandContainer;
                    if (commandProvider != null) {
                        command = commandProvider.GetCommand(_commandName);
                        if (command != null) {
                            break;
                        }
                    }

                    element = element.Parent as FrameworkElement;
                }
            }

            return command;
        }

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            AssociatedObject.Dispatcher.BeginInvoke(delegate() {
                if (String.IsNullOrEmpty(_commandName) == false) {
                    _command = GetCommand();
                    if (_command != null) {
                        _command.CanExecuteChanged += OnCommandCanExecuteChanged;
                        UpdateAssociatedObject();
                    }

                    AssociatedObject.Click += OnClick;
                }
            });
        }

        private void OnClick(object sender, RoutedEventArgs e) {
            if ((_command == null) && (_unknownCommand == false)) {
                _command = GetCommand();
                if (_command == null) {
                    _unknownCommand = true;
                }
            }
            if (_command != null) {
                _command.Execute(_commandParameter);
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e) {
            UpdateAssociatedObject();
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.Click -= OnClick;

            if (_command != null) {
                _command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                _command = null;
            }
            _unknownCommand = false;

            base.OnDetach();
        }

        private void UpdateAssociatedObject() {
            if (AssociatedObject != null) {
                bool isEnabled = true;

                if (_command != null) {
                    isEnabled = _command.CanExecute(_commandParameter);
                }

                AssociatedObject.IsEnabled = isEnabled;
            }
        }
    }
}
