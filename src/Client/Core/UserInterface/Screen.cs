// Screen.cs
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

    // TODO: Add visual states for empty and for main window
    // TODO: Add effect properties for showing/hiding forms and main window

    /// <summary>
    /// Represents the root visual of an application.
    /// </summary>
    [TemplatePart(Name = "RootElement", Type = typeof(Grid))]
    [TemplatePart(Name = "WindowPresenter", Type = typeof(ContentPresenter))]
    public sealed class Screen : ContentControl {

        /// <summary>
        /// Represents the FormBackground property.
        /// </summary>
        public static readonly DependencyProperty FormBackgroundProperty =
            DependencyProperty.Register("FormBackground", typeof(Brush), typeof(Screen), null);

        private ContentPresenter _windowPresenter;
        private Grid _rootElement;

        private Window _mainWindow;
        private Form _currentForm;

        /// <summary>
        /// Initializes an instance of a Screen control.
        /// </summary>
        public Screen() {
            DefaultStyleKey = typeof(Screen);
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
                _mainWindow.IsEnabled = true;
                _mainWindow.Focus();
            }
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _rootElement = GetTemplateChild("RootElement") as Grid;
            _windowPresenter = GetTemplateChild("WindowPresenter") as ContentPresenter;

            if (_mainWindow != null) {
                _windowPresenter.Content = _mainWindow;
            }
        }

        internal void Run(Window mainWindow) {
            if (_mainWindow != null) {
                throw new InvalidOperationException("The screen already contains content.");
            }
            _mainWindow = mainWindow;

            if (_windowPresenter == null) {
                ApplyTemplate();
            }
            else {
                _windowPresenter.Content = _mainWindow;
            }
        }

        /// <summary>
        /// Shows the specified content in the screen. Typically the Screen is created and its
        /// content is set by the framework internally when you call XApplication.Run.
        /// If you create the Screen yourself, you can initialize its content by calling this
        /// method.
        /// </summary>
        /// <param name="screenContent">The content to show on the screen.</param>
        public void Show(FrameworkElement screenContent) {
            if (screenContent == null) {
                throw new ArgumentNullException("screenContent");
            }

            Window window = screenContent as Window;
            if (window == null) {
                window = new Window(screenContent);
            }

            Run(window);
        }

        internal void Show(Form form) {
            if (_mainWindow == null) {
                return;
            }

            if (_currentForm == null) {
                _mainWindow.IsEnabled = false;
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
