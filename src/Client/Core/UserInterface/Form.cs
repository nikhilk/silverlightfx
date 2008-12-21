// Form.cs
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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents the root visual of the application.
    /// </summary>
    public class Form : View {

        private FormResult _formResult;
        private EventHandler _closedHandler;

        private Form _parentForm;
        private FrameworkElement _overlayElement;

        /// <summary>
        /// Initializes an instance of a Form.
        /// </summary>
        public Form() {
        }

        /// <summary>
        /// Initializes an instance of a Form with an associated view model.
        /// The view model is set as the DataContext of the Form.
        /// </summary>
        /// <param name="viewModel">The associated view model object.</param>
        public Form(Model viewModel)
            : base(viewModel) {
        }

        /// <summary>
        /// Gets the parent Form that logically owns this Form.
        /// </summary>
        public Form ParentForm {
            get {
                return _parentForm;
            }
        }

        /// <summary>
        /// The result of Form selected when it was closed.
        /// </summary>
        public FormResult Result {
            get {
                return _formResult;
            }
        }

        /// <summary>
        /// Raised when the Form is closed.
        /// </summary>
        public event EventHandler Closed {
            add {
                _closedHandler = (EventHandler)Delegate.Combine(_closedHandler, value);
            }
            remove {
                _closedHandler = (EventHandler)Delegate.Remove(_closedHandler, value);
            }
        }

        /// <summary>
        /// Closes the Form with the specified FormResult code.
        /// </summary>
        /// <param name="result">The result of the Form.</param>
        public void Close(FormResult result) {
            _formResult = result;

            Panel parentPanel = (Panel)Parent;

            Visibility = Visibility.Collapsed;
            parentPanel.Children.Remove(this);
            if (_overlayElement != null) {
                parentPanel.Children.Remove(_overlayElement);
            }

            if (_parentForm != null) {
                _parentForm.IsEnabled = true;
                _parentForm.Focus();
            }

            Screen currentScreen = Application.Current.RootVisual as Screen;
            currentScreen.Close(this, _parentForm);

            if (_closedHandler != null) {
                _closedHandler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Shows the Form as a modal dialog.
        /// </summary>
        public void Show() {
            Screen currentScreen = Application.Current.RootVisual as Screen;
            if (currentScreen != null) {
                currentScreen.Show(this);
            }
        }

        internal void Show(Panel parentPanel, Form parentForm, FrameworkElement overlayElement) {
            _parentForm = parentForm;
            _overlayElement = overlayElement;

            if (parentForm != null) {
                parentForm.IsEnabled = false;
            }

            if (_overlayElement != null) {
                parentPanel.Children.Add(_overlayElement);
            }
            parentPanel.Children.Add(this);

            Visibility = Visibility.Visible;
            Focus();
        }
    }
}
