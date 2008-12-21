// Screen.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// You are free to:
// - use this framework as part of your app
// - make use of the framework in a commercial app
// as long as your app or product is itself not a framework, control pack or
// developer toolkit of any sort under the following conditions:
// Attribution. You must attribute the original work in your
//              product or release.
// Share Alike. If you alter, transform, or build as-is upon this work,
//              you may only distribute the resulting app source under
//              a license identical to this one.
//

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        internal void Run(Window mainWindow) {
            _mainWindow = mainWindow;

            if (_windowPresenter == null) {
                ApplyTemplate();
            }
            else {
                _windowPresenter.Content = _mainWindow;
            }
        }
    }
}
