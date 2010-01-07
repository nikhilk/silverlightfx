// ViewWorkspace.cs
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SilverlightFX.UserInterface {

    // TODO: Add effect properties for showing/hiding forms and main view

    /// <summary>
    /// Represents the workspace containing the main view of the application.
    /// </summary>
    [TemplatePart(Name = "RootElement", Type = typeof(Grid))]
    [TemplatePart(Name = "ViewPresenter", Type = typeof(ContentPresenter))]
    public sealed class ViewWorkspace : ContentControl {

        /// <summary>
        /// Represents the FormBackground property.
        /// </summary>
        public static readonly DependencyProperty FormBackgroundProperty =
            DependencyProperty.Register("FormBackground", typeof(Brush), typeof(ViewWorkspace), null);

        private ContentPresenter _viewPresenter;
        private Grid _rootElement;

        private View _mainView;
        private Form _currentForm;

        /// <summary>
        /// Initializes an instance of a ViewWorkspace control.
        /// </summary>
        public ViewWorkspace() {
            DefaultStyleKey = typeof(ViewWorkspace);
        }

        /// <summary>
        /// The background brush applied as an overlay on top of the previous form
        /// when a new form is displayed.
        /// </summary>
        public Brush FormBackground {
            get {
                return (Brush)GetValue(FormBackgroundProperty);
            }
            set {
                SetValue(FormBackgroundProperty, value);
            }
        }

        internal void Close(Form form, Form previousForm) {
            _currentForm = previousForm;
            if (_currentForm == null) {
                _mainView.IsEnabled = true;
                _mainView.Focus();
            }
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _rootElement = GetTemplateChild("RootElement") as Grid;
            _viewPresenter = GetTemplateChild("ViewPresenter") as ContentPresenter;

            if (_mainView != null) {
                _viewPresenter.Content = _mainView;
            }
        }

        internal void Run(View mainView) {
            if (_mainView != null) {
                throw new InvalidOperationException("The workspace already contains a view.");
            }
            _mainView = mainView;

            if (_viewPresenter == null) {
                ApplyTemplate();
            }
            else {
                _viewPresenter.Content = _mainView;
            }
        }

        /// <summary>
        /// Shows the specified content in the workspace. Typically the Workspace is created and its
        /// content is set by the framework internally by ApplicationContext.
        /// If you create the Workspace yourself, you can initialize its content by calling this
        /// method.
        /// </summary>
        /// <param name="content">The content to show on the workspace.</param>
        public void Show(FrameworkElement content) {
            if (content == null) {
                throw new ArgumentNullException("content");
            }

            View view = content as View;
            if (view == null) {
                view = new View(content);
            }

            Run(view);
        }

        internal void Show(Form form) {
            if (_mainView == null) {
                return;
            }

            if (_currentForm == null) {
                _mainView.IsEnabled = false;
            }

            Grid overlayElement = null;

            Brush overlayBrush = FormBackground;
            if (overlayBrush != null) {
                overlayElement = new Grid();
                overlayElement.Background = overlayBrush;
            }

            form.Show(_rootElement, _currentForm, overlayElement);
        }
    }
}
