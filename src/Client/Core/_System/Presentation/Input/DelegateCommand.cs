// DelegateCommand.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.ComponentModel;
using System.Windows;

namespace System.Windows.Input {

    /// <summary>
    /// Represents a specific command instance that can be invoked to do
    /// some work, or check if it is enabled.
    /// </summary>
    public class DelegateCommand : ICommand {

        private Action<object> _commandAction;
        private bool _canExecute;

        private EventHandler _changedHandler;

        /// <summary>
        /// Initializes a command as executable.
        /// </summary>
        protected DelegateCommand() {
            _canExecute = true;
        }

        /// <summary>
        /// Initializes a command with the associated action.
        /// </summary>
        /// <param name="commandAction">The action to invoke when the command is executed.</param>
        public DelegateCommand(Action<object> commandAction)
            : this(commandAction, true) {
        }

        /// <summary>
        /// Initializes a command with the associated action.
        /// </summary>
        /// <param name="commandAction">The action to invoke when the command is executed.</param>
        /// <param name="canExecute">The initial state of the command.</param>
        public DelegateCommand(Action<object> commandAction, bool canExecute) {
            if (commandAction == null) {
                throw new ArgumentNullException("commandAction");
            }
            _commandAction = commandAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Invokes the command to allow it to execute its associated functionality.
        /// </summary>
        /// <param name="parameter">Any parameter information associated with the command.</param>
        protected virtual void Execute(object parameter) {
            if (_commandAction != null) {
                _commandAction(parameter);
            }
        }

        /// <summary>
        /// Updates the status of the command.
        /// </summary>
        /// <param name="canExecute">Whether the command is executable.</param>
        public void UpdateStatus(bool canExecute) {
            if (_canExecute != canExecute) {
                _canExecute = canExecute;
                if (_changedHandler != null) {
                    _changedHandler(this, EventArgs.Empty);
                }
            }
        }

        #region ICommand Members
        event EventHandler ICommand.CanExecuteChanged {
            add {
                _changedHandler = (EventHandler)Delegate.Combine(_changedHandler, value);
            }
            remove {
                _changedHandler = (EventHandler)Delegate.Remove(_changedHandler, value);
            }
        }

        bool ICommand.CanExecute(object parameter) {
            return _canExecute;
        }

        void ICommand.Execute(object parameter) {
            Execute(parameter);
        }
        #endregion
    }
}
