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

        /// <summary>
        /// Creates a popup that is correctly parented to the root visual of the
        /// application.
        /// </summary>
        /// <returns>A new Popup instance.</returns>
        public Popup CreatePopup() {
            Popup popup = new Popup();
            _rootElement.Children.Add(popup);

            return popup;
        }

        /// <summary>
        /// Disposes a popup that was created through a call to CreatePopup.
        /// </summary>
        /// <param name="popup">The popup to dispose.</param>
        public void DisposePopup(Popup popup) {
            _rootElement.Children.Remove(popup);
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
