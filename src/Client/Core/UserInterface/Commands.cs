// Commands.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Data;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Provides a set of command properties on various controls.
    /// </summary>
    public static class Commands {

        /// <summary>
        /// Represents the Checked attached property.
        /// </summary>
        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.RegisterAttached("Checked", typeof(ICommand), typeof(Commands),
                                                new PropertyMetadata(OnCheckedPropertyChanged));

        /// <summary>
        /// Represents the Click attached property.
        /// </summary>
        public static readonly DependencyProperty ClickProperty =
            DependencyProperty.RegisterAttached("Click", typeof(ICommand), typeof(Commands),
                                                new PropertyMetadata(OnClickPropertyChanged));

        /// <summary>
        /// Represents the Parameter attached property.
        /// </summary>
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(Commands), null);

        /// <summary>
        /// Represents the Selection attached property.
        /// </summary>
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.RegisterAttached("Selection", typeof(ICommand), typeof(Commands),
                                                new PropertyMetadata(OnSelectionPropertyChanged));

        /// <summary>
        /// Gets the command to execute when the button is checked or unchecked.
        /// </summary>
        /// <param name="button">The associated button.</param>
        /// <returns>The command if one has been assigned.</returns>
        public static ICommand GetChecked(ToggleButton button) {
            return (ICommand)button.GetValue(CheckedProperty);
        }

        /// <summary>
        /// Gets the command to execute when the button is clicked.
        /// </summary>
        /// <param name="button">The associated button.</param>
        /// <returns>The command if one has been assigned.</returns>
        public static ICommand GetClick(ButtonBase button) {
            return (ICommand)button.GetValue(ClickProperty);
        }

        /// <summary>
        /// Gets the parameter to use when the command is executed.
        /// </summary>
        /// <param name="element">The associated element.</param>
        /// <returns>The parameter value.</returns>
        public static object GetParameter(FrameworkElement element) {
            return element.GetValue(ParameterProperty);
        }

        /// <summary>
        /// Gets the command to execute when the selection is changed.
        /// </summary>
        /// <param name="selector">The associated selector.</param>
        /// <returns>The command if one has been assigned.</returns>
        public static ICommand GetSelection(Selector selector) {
            return (ICommand)selector.GetValue(SelectionProperty);
        }

        private static void OnCheckedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            OnCommandPropertyChanged<CheckedCommandBehavior>(o, e);
        }

        private static void OnClickPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            OnCommandPropertyChanged<ClickCommandBehavior>(o, e);
        }

        private static void OnSelectionPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            OnCommandPropertyChanged<SelectionCommandBehavior>(o, e);
        }

        private static void OnCommandPropertyChanged<TBehavior>(DependencyObject o, DependencyPropertyChangedEventArgs e) where TBehavior : CommandBehavior, new() {
            ICommand command = (ICommand)e.NewValue;

            BehaviorCollection behaviors = Interaction.GetBehaviors(o);
            CommandBehavior behavior = behaviors.GetBehavior<TBehavior>();

            if (behavior != null) {
                if (e.NewValue == null) {
                    behaviors.Remove(behavior);
                }
                else {
                    behavior.SetCommand(command);
                }
            }
            else if (command != null) {
                behavior = new TBehavior();
                behaviors.Add(behavior);
                behavior.SetCommand(command);
            }
        }

        /// <summary>
        /// Sets the command to execute when the button is checked or unchecked.
        /// </summary>
        /// <param name="button">The associated button.</param>
        /// <param name="command">The comand to execute.</param>
        public static void SetChecked(ToggleButton button, ICommand command) {
            button.SetValue(CheckedProperty, command);
        }

        /// <summary>
        /// Sets the command to execute when the button is clicked.
        /// </summary>
        /// <param name="button">The associated button.</param>
        /// <param name="command">The comand to execute.</param>
        public static void SetClick(ButtonBase button, ICommand command) {
            button.SetValue(ClickProperty, command);
        }

        /// <summary>
        /// Sets the parameter to use when the command is executed.
        /// </summary>
        /// <param name="element">The associated element.</param>
        /// <param name="parameter">The parameter value to use.</param>
        public static void SetParameter(FrameworkElement element, object parameter) {
            element.SetValue(ParameterProperty, parameter);
        }

        /// <summary>
        /// Sets the command to execute when the selection is changed.
        /// </summary>
        /// <param name="selector">The associated selector.</param>
        /// <param name="command">The comand to execute.</param>
        public static void SetSelection(Selector selector, ICommand command) {
            selector.SetValue(SelectionProperty, command);
        }


        private abstract class CommandBehavior : Behavior<FrameworkElement> {

            private ICommand _command;
            private BindingShim _binder;

            protected void ExecuteCommand(object parameter) {
                object specifiedParameter = Commands.GetParameter(AssociatedObject);
                if (specifiedParameter != null) {
                    parameter = specifiedParameter;
                }

                _command.Execute(parameter);
            }

            private void OnCommandCanExecuteChanged(object sender, EventArgs e) {
                Update();
            }

            protected abstract void OnCommandChanged(bool canExecute);

            public void SetCommand(ICommand command) {
                if (_command != null) {
                    if (_binder != null) {
                        _binder.Dispose();
                        _binder = null;
                    }
                    _command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                }

                _command = command;
                _command.CanExecuteChanged += OnCommandCanExecuteChanged;
                Update();

                Command commandObject = command as Command;
                if (commandObject != null) {
                    _binder = new BindingShim(AssociatedObject, new Binding(), UpdateCommandDataContext);
                    UpdateCommandDataContext();
                }
            }

            private void Update() {
                object parameter = Commands.GetParameter(AssociatedObject);
                bool canExecute = _command.CanExecute(parameter);

                OnCommandChanged(canExecute);
            }

            private void UpdateCommandDataContext() {
                Command commandObject = _command as Command;
                commandObject.DataContext = _binder.Value;
            }
        }

        private sealed class ClickCommandBehavior : CommandBehavior {

            protected override void OnAttach() {
                base.OnAttach();

                ButtonBase button = (ButtonBase)AssociatedObject;
                button.Click += OnButtonClick;
            }

            private void OnButtonClick(object sender, RoutedEventArgs e) {
                ExecuteCommand(null);
            }

            protected override void OnCommandChanged(bool canExecute) {
                ((ButtonBase)AssociatedObject).IsEnabled = canExecute;
            }

            protected override void OnDetach() {
                ButtonBase button = (ButtonBase)AssociatedObject;
                button.Click -= OnButtonClick;

                base.OnDetach();
            }
        }

        private sealed class CheckedCommandBehavior : CommandBehavior {

            protected override void OnAttach() {
                base.OnAttach();

                ToggleButton button = (ToggleButton)AssociatedObject;
                button.Checked += OnButtonToggled;
            }

            private void OnButtonToggled(object sender, RoutedEventArgs e) {
                ExecuteCommand(((CheckBox)AssociatedObject).IsChecked);
            }

            protected override void OnCommandChanged(bool canExecute) {
                ((ToggleButton)AssociatedObject).IsEnabled = canExecute;
            }

            protected override void OnDetach() {
                ToggleButton button = (ToggleButton)AssociatedObject;
                button.Checked -= OnButtonToggled;

                base.OnDetach();
            }
        }

        private sealed class SelectionCommandBehavior : CommandBehavior {

            protected override void OnAttach() {
                base.OnAttach();

                Selector selector = (Selector)AssociatedObject;
                selector.SelectionChanged += OnSelectorSelectionChanged;
            }

            protected override void OnCommandChanged(bool canExecute) {
                ((Selector)AssociatedObject).IsEnabled = canExecute;
            }

            private void OnSelectorSelectionChanged(object sender, SelectionChangedEventArgs e) {
                ExecuteCommand(((Selector)AssociatedObject).SelectedItem);
            }

            protected override void OnDetach() {
                Selector selector = (Selector)AssociatedObject;
                selector.SelectionChanged += OnSelectorSelectionChanged;

                base.OnDetach();
            }
        }
    }
}
