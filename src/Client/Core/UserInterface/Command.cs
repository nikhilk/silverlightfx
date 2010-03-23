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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CommandExecutingEventHandler = System.EventHandler<SilverlightFX.UserInterface.CommandExecutingEventArgs>;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A declarative implementation of an ICommand that binds to a method on
    /// the specified target object.
    /// </summary>
    public class Command : FrameworkElement, ICommand {

        /// <summary>
        /// Represents the Target property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(Command),
                                        new PropertyMetadata(OnViewModelPropertyChanged));

        private EventHandler _canExecuteChangedHandler;

        private object _target;
        private string _method;
        private bool? _enabled;

        private bool _listeningForChanges;
        private PropertyInfo _canExecuteProperty;
        private MethodInfo _executeMethod;
        private bool _executeWithParameter;

        private CommandExecutingEventHandler _executingHandler;
        private EventHandler _executedHandler;

        /// <summary>
        /// Initializes an instance of a Command.
        /// </summary>
        public Command() {
            Binding binding = new Binding();
            BindingOperations.SetBinding(this, TargetProperty, binding);
        }

        /// <summary>
        /// Gets or sets the method to invoke when the command is executed.
        /// The command also looks for a property named the same as the method name
        /// with a 'Can' prefix to determine when the command should be enabled.
        /// </summary>
        public string Method {
            get {
                return _method;
            }
            set {
                _method = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the target object whose method is to be invoked when the
        /// command is executed. By default, this is the current DataContext.
        /// </summary>
        public object Target {
            get {
                return GetValue(TargetProperty);
            }
            set {
                SetValue(TargetProperty, value);
            }
        }

        /// <summary>
        /// The event raised after the method has been invoked.
        /// </summary>
        public event EventHandler Executed {
            add {
                _executedHandler = (EventHandler)Delegate.Combine(_executedHandler, value);
            }
            remove {
                _executedHandler = (EventHandler)Delegate.Remove(_executedHandler, value);
            }
        }

        /// <summary>
        /// The event raised before the method is invoked. The event handler can cancel
        /// invokation of the method.
        /// </summary>
        public event CommandExecutingEventHandler Executing {
            add {
                _executingHandler = (CommandExecutingEventHandler)Delegate.Combine(_executingHandler, value);
            }
            remove {
                _executingHandler = (CommandExecutingEventHandler)Delegate.Remove(_executingHandler, value);
            }
        }

        /// <summary>
        /// Invokes the method on the current target.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the method if any.</param>
        public void Execute(object parameter) {
            if ((_target != null) && (_executeMethod != null)) {
                CommandExecutingEventArgs ce = new CommandExecutingEventArgs(parameter);
                OnExecuting(ce);
                if (ce.Cancel) {
                    return;
                }

                if (_executeWithParameter) {
                    _executeMethod.Invoke(_target, new object[] { parameter });
                }
                else {
                    _executeMethod.Invoke(_target, null);
                }

                OnExecuted(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the Executed event.
        /// </summary>
        /// <param name="e">The event argument associated with the event.</param>
        protected virtual void OnExecuted(EventArgs e) {
            if (_executedHandler != null) {
                _executedHandler(this, e);
            }
        }

        /// <summary>
        /// Raises the Executing event.
        /// </summary>
        /// <param name="e">The event argument associated with the event.</param>
        protected virtual void OnExecuting(CommandExecutingEventArgs e) {
            if (_executingHandler != null) {
                _executingHandler(this, e);
            }
        }

        private void OnTargetChanged(object newTarget) {
            if (DesignerProperties.IsInDesignTool) {
                return;
            }

            if (_listeningForChanges) {
                ((INotifyPropertyChanged)_target).PropertyChanged -= OnTargetPropertyChanged;
                _listeningForChanges = false;
            }

            _target = newTarget;
            if (_target != null) {
                INotifyPropertyChanged inpc = _target as INotifyPropertyChanged;
                if (inpc != null) {
                    inpc.PropertyChanged += OnTargetPropertyChanged;
                    _listeningForChanges = true;
                }
            }

            Update();
        }

        private void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (String.CompareOrdinal(e.PropertyName, "Can" + _method) == 0) {
                RaiseCanExecuteChanged();
            }
        }

        private static void OnViewModelPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((Command)o).OnTargetChanged(e.NewValue);
        }

        private void RaiseCanExecuteChanged() {
            if (_canExecuteChangedHandler != null) {
                _canExecuteChangedHandler(this, EventArgs.Empty);
            }
        }

        private void Update() {
            if (_target != null) {
                Type viewModelType = _target.GetType();
                _executeMethod = viewModelType.GetMethod(_method);
                _canExecuteProperty = viewModelType.GetProperty("Can" + _method);

                ParameterInfo[] parameters = _executeMethod.GetParameters();
                _executeWithParameter = ((parameters != null) && (parameters.Length == 1));
            }

            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Updates the enabled status of a command. By default the command's status
        /// is based on the target object's state. Calling this method causes the enabled
        /// state to be a combination of the value passed in and the target object's state.
        /// </summary>
        /// <param name="enabled">Whether the command is enabled or disabled.</param>
        public void UpdateStatus(bool enabled) {
            _enabled = enabled;
            RaiseCanExecuteChanged();
        }


        #region Implementation of ICommand
        bool ICommand.CanExecute(object parameter) {
            if (DesignerProperties.IsInDesignTool) {
                return true;
            }

            if (_target == null) {
                return false;
            }

            if (_canExecuteProperty != null) {
                bool targetEnabled = (bool)_canExecuteProperty.GetValue(_target, null);
                if (targetEnabled == false) {
                    return false;
                }
            }

            if (_enabled.HasValue) {
                return _enabled.Value;
            }

            return true;
        }

        event EventHandler ICommand.CanExecuteChanged {
            add {
                _canExecuteChangedHandler = (EventHandler)Delegate.Combine(_canExecuteChangedHandler, value);
            }
            remove {
                _canExecuteChangedHandler = (EventHandler)Delegate.Remove(_canExecuteChangedHandler, value);
            }
        }

        void ICommand.Execute(object parameter) {
            Execute(parameter);
        }
        #endregion
    }
}
