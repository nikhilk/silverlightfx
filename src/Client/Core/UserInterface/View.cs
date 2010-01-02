// View.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents the top-most view in an application.
    /// </summary>
    public class View : UserControl {

        /// <summary>
        /// Represents the Model attached property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached("Model", typeof(object), typeof(View),
                                                new PropertyMetadata(OnModelPropertyChanged));

        private IDictionary<string, object> _viewData;

        /// <summary>
        /// Initializes an instance of a View.
        /// </summary>
        public View() {
            this.Loaded += OnLoaded;
        }

        internal View(UIElement content)
            : this() {
            Content = content;
        }

        /// <summary>
        /// Gets the view model instance attached to the specified control.
        /// </summary>
        /// <param name="userControl">The control to lookup.</param>
        /// <returns>The view model if one is associated with the control; null otherwise.</returns>
        public static object GetViewModel(UserControl userControl) {
            return userControl.GetValue(ModelProperty);
        }

        /// <summary>
        /// Initializes the view with any view data. The View data is used to initialize
        /// the associated view model.
        /// </summary>
        /// <param name="viewData"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void InitializeViewData(IDictionary<string, object> viewData) {
            _viewData = viewData;
        }

        /// <summary>
        /// Performs initialization in response to the Loaded event.
        /// </summary>
        protected virtual void OnLoaded() {
            object viewModel = View.GetViewModel(this);
            if (viewModel == null) {
                viewModel = ViewModelAttribute.CreateViewModel(this);
                if (viewModel != null) {
                    View.SetViewModel(this, viewModel);
                }
            }

            if ((viewModel != null) && (_viewData != null)) {
                ISupportInitialize batchInitialize = viewModel as ISupportInitialize;
                if (batchInitialize != null) {
                    batchInitialize.BeginInit();
                }

                Type viewModelType = viewModel.GetType();
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance |
                                            BindingFlags.FlattenHierarchy;

                foreach (KeyValuePair<string, object> viewDataItem in _viewData) {
                    PropertyInfo pi = viewModelType.GetProperty(viewDataItem.Key, bindingFlags);
                    if ((pi != null) && pi.CanWrite) {
                        pi.SetValue(viewModel, viewDataItem.Value, null);
                    }
                }

                if (batchInitialize != null) {
                    batchInitialize.EndInit();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            OnLoaded();
        }

        private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ViewModelAttribute.SetViewModel((UserControl)d, e.NewValue);
        }

        /// <summary>
        /// Sets the view model instance attached to the specified control.
        /// The view model is also used as the DataContext assigned to the control.
        /// </summary>
        /// <param name="userControl">The control to associated the view model with.</param>
        /// <param name="value">The view model instance.</param>
        public static void SetViewModel(UserControl userControl, object value) {
            userControl.SetValue(ModelProperty, value);
        }
    }
}
