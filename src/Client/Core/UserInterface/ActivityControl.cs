// ActivityControl.cs
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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A control that can visualize an asynchronous activity to show progress, errors,
    /// and the ability to cancel the activity.
    /// </summary>
    [TemplateVisualState(Name = "Untracked", GroupName = "ProgressStates")]
    [TemplateVisualState(Name = "Tracking", GroupName = "ProgressStates")]
    [TemplateVisualState(Name = "Empty", GroupName = "StatusStates")]
    [TemplateVisualState(Name = "Message", GroupName = "StatusStates")]
    [TemplateVisualState(Name = "NonCancelable", GroupName = "CancelStates")]
    [TemplateVisualState(Name = "Cancelable", GroupName = "CancelStates")]
    public class ActivityControl : Control {

        /// <summary>
        /// Represents the AsyncActivity property.
        /// </summary>
        public static readonly DependencyProperty AsyncActivityProperty =
            DependencyProperty.Register("AsyncActivity", typeof(Async), typeof(ActivityControl),
                                        new PropertyMetadata(OnAsyncActivityPropertyChanged));

        /// <summary>
        /// Represents the CancelButtonStyle property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonStyleProperty =
            DependencyProperty.Register("CancelButtonStyle", typeof(Style), typeof(ActivityControl), null);

        /// <summary>
        /// Represents the ProgressBarStyle property.
        /// </summary>
        public static readonly DependencyProperty ProgressBarStyleProperty =
            DependencyProperty.Register("ProgressBarStyle", typeof(Style), typeof(ActivityControl), null);

        /// <summary>
        /// Represents the StatusLabelStyle property.
        /// </summary>
        public static readonly DependencyProperty StatusLabelStyleProperty =
            DependencyProperty.Register("StatusLabelStyle", typeof(Style), typeof(ActivityControl), null);

        private Visibility _progressBarVisibility;
        private Visibility _statusLabelVisibility;
        private Visibility _cancelButtonVisibility;
        private bool _showErrors;

        private DelegateCommand _cancelCommand;

        /// <summary>
        /// Initializes an instance of an ActivityControl.
        /// </summary>
        public ActivityControl() {
            DefaultStyleKey = typeof(ActivityControl);
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Gets or sets the async activity to track in the control.
        /// </summary>
        public Async AsyncActivity {
            get {
                return (Async)GetValue(AsyncActivityProperty);
            }
            set {
                SetValue(AsyncActivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the contained cancel button.
        /// </summary>
        public Style CancelButtonStyle {
            get {
                return (Style)GetValue(CancelButtonStyleProperty);
            }
            set {
                SetValue(CancelButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the cancel button should be visible when there
        /// is an async activity that supports cancelation.
        /// </summary>
        public Visibility CancelButtonVisibility {
            get {
                return _cancelButtonVisibility;
            }
            set {
                _cancelButtonVisibility = value;
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the contained progressbar.
        /// </summary>
        public Style ProgressBarStyle {
            get {
                return (Style)GetValue(ProgressBarStyleProperty);
            }
            set {
                SetValue(ProgressBarStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the progress bar should be visible when there
        /// is an async activity.
        /// </summary>
        public Visibility ProgressBarVisibility {
            get {
                return _progressBarVisibility;
            }
            set {
                _progressBarVisibility = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to show error messages when there is an error in
        /// the async activity.
        /// </summary>
        public bool ShowErrors {
            get {
                return _showErrors;
            }
            set {
                _showErrors = value;
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the status label.
        /// </summary>
        public Style StatusLabelStyle {
            get {
                return (Style)GetValue(StatusLabelStyleProperty);
            }
            set {
                SetValue(StatusLabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the status label should be visible when there is
        /// an async activity.
        /// </summary>
        public Visibility StatusLabelVisibility {
            get {
                return _statusLabelVisibility;
            }
            set {
                _statusLabelVisibility = value;
            }
        }

        private void Cancel() {
            Async asyncActivity = AsyncActivity;
            if ((asyncActivity != null) && asyncActivity.CanCancel) {
                asyncActivity.Cancel();
            }
        }

        private void OnAsyncActivityChanged(DependencyPropertyChangedEventArgs e) {
            Async oldAsyncValue = (Async)e.OldValue;
            if (oldAsyncValue != null) {
                oldAsyncValue.Completed -= OnAsyncActivityCompleted;
            }

            Async newAsyncValue = (Async)e.NewValue;
            if ((newAsyncValue != null) && (newAsyncValue.IsCompleted == false)) {
                newAsyncValue.Completed += OnAsyncActivityCompleted;
            }

            UpdateVisualState(newAsyncValue);
        }

        private void OnAsyncActivityCompleted(object sender, EventArgs e) {
            Async asyncActivity = (Async)sender;
            if (asyncActivity == AsyncActivity) {
                asyncActivity.Completed -= OnAsyncActivityCompleted;

                if ((asyncActivity.IsCanceled == false) &&
                    ShowErrors &&
                    asyncActivity.HasError && (asyncActivity.IsErrorHandled == false)) {
                    MessageBox.Show(asyncActivity.Error.Message, "Error", MessageBoxButton.OK);
                    asyncActivity.MarkErrorAsHandled();
                }

                UpdateVisualState(null);
            }
        }

        private static void OnAsyncActivityPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ActivityControl)o).OnAsyncActivityChanged(e);
        }

        private void OnAsyncControlAsyncActivityChanged(object sender, EventArgs e) {
            AsyncActivity = ((IAsyncControl)sender).AsyncActivity;
        }

        private void OnLoaded(object sender, EventArgs e) {
            _cancelCommand = new DelegateCommand(Cancel);
            Resources.Add("CancelCommand", _cancelCommand);

            ApplyTemplate();

            IAsyncControl asyncControl = Parent as IAsyncControl;
            if (asyncControl != null) {
                asyncControl.AsyncActivityChanged += OnAsyncControlAsyncActivityChanged;
            }
        }

        private void UpdateVisualState(Async asyncActivity) {
            if ((asyncActivity != null) && (asyncActivity.IsCompleted == false)) {
                string progressState = "Untracked";
                if (_progressBarVisibility == Visibility.Visible) {
                    progressState = "Tracking";
                }
                VisualStateManager.GoToState(this, progressState, /* useTransitions */ true);

                string statusState = "Empty";
                if ((_statusLabelVisibility == Visibility.Visible) &&
                    (String.IsNullOrEmpty(asyncActivity.Message) == false)) {
                    statusState = "Message";
                }
                VisualStateManager.GoToState(this, statusState, /* useTransitions */ true);

                string cancelState = "NonCancelable";
                if ((_cancelButtonVisibility == Visibility.Visible) &&
                    asyncActivity.CanCancel) {
                    cancelState = "Cancelable";
                }
                VisualStateManager.GoToState(this, cancelState, /* useTransitions */ true);
            }
            else {
                VisualStateManager.GoToState(this, "Untracked", /* useTransitions */ true);
                VisualStateManager.GoToState(this, "Empty", /* useTransitions */ true);
                VisualStateManager.GoToState(this, "NonCancelable", /* useTransitions */ true);
            }
        }
    }
}
