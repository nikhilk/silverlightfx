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
using System.Windows.Input;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents the root visual of the application.
    /// </summary>
    public class Form : View {

        /// <summary>
        /// Represents the CloseEffect property.
        /// </summary>
        public static readonly DependencyProperty CloseEffectProperty =
            DependencyProperty.Register("CloseEffect", typeof(AnimationEffect), typeof(Form), null);

        /// <summary>
        /// Represents the ShowEffect property.
        /// </summary>
        public static readonly DependencyProperty ShowEffectProperty =
            DependencyProperty.Register("ShowEffect", typeof(AnimationEffect), typeof(Form), null);

        private FormResult _formResult;
        private EventHandler _closedHandler;
        private bool _canClose;

        private Form _parentForm;
        private FrameworkElement _overlayElement;

        private DelegateCommand _cancelCommand;
        private DelegateCommand _okCommand;

        /// <summary>
        /// Initializes an instance of a Form.
        /// </summary>
        public Form() :
            this(null) {
        }

        /// <summary>
        /// Initializes an instance of a Form with an associated view model.
        /// The view model is set as the DataContext of the Form.
        /// </summary>
        /// <param name="viewModel">The associated view model object.</param>
        public Form(object viewModel)
            : base(viewModel) {
            _cancelCommand = new DelegateCommand(OnCancelCommand, /* canExecute */ true);
            _okCommand = new DelegateCommand(OnOKCommand, /* canExecute */ true);

            Resources.Add("CancelCommand", _cancelCommand);
            Resources.Add("OKCommand", _okCommand);
        }

        /// <summary>
        /// Gets or sets the effect to be played when the form is closing.
        /// </summary>
        public AnimationEffect CloseEffect {
            get {
                return (AnimationEffect)GetValue(CloseEffectProperty);
            }
            set {
                SetValue(CloseEffectProperty, value);
            }
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
        /// Gets or sets the effect to be played when the form is being shown.
        /// </summary>
        public AnimationEffect ShowEffect {
            get {
                return (AnimationEffect)GetValue(ShowEffectProperty);
            }
            set {
                SetValue(ShowEffectProperty, value);
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
            if (_canClose == false) {
                return;
            }

            _formResult = result;
            _canClose = false;

            AnimationEffect closeEffect = CloseEffect;
            if (closeEffect == null) {
                CloseCore();
            }
            else {
                if (((IAttachedObject)closeEffect).AssociatedObject != this) {
                    ((IAttachedObject)closeEffect).Attach(this);
                    closeEffect.Completed += OnCloseEffectCompleted;
                }
                closeEffect.PlayEffect(AnimationEffectDirection.Forward);
            }
        }

        private void CloseCore() {
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

        private void OnCancelCommand() {
            TaskViewModel model = View.GetModel(this) as TaskViewModel;
            if (model != null) {
                model.Cancel(delegate() {
                    Close(FormResult.Cancel);
                });
                return;
            }

            Close(FormResult.Cancel);
        }

        private void OnCloseEffectCompleted(object sender, EventArgs e) {
            CloseCore();
        }

        private void OnOKCommand() {
            TaskViewModel model = View.GetModel(this) as TaskViewModel;
            if (model != null) {
                model.Commit(delegate() {
                    Close(FormResult.OK);
                });
                return;
            }

            Close(FormResult.OK);
        }

        private void OnShowEffectCompleted(object sender, EventArgs e) {
            _canClose = true;
            Focus();
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

            AnimationEffect showEffect = ShowEffect;
            if (showEffect == null) {
                _canClose = true;
                Focus();
            }
            else {
                _canClose = false;

                if (((IAttachedObject)showEffect).AssociatedObject != this) {
                    ((IAttachedObject)showEffect).Attach(this);
                    showEffect.Completed += OnShowEffectCompleted;
                }
                showEffect.PlayEffect(AnimationEffectDirection.Forward);
            }
        }
    }
}
