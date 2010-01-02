// ViewModelAttribute.cs
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
using System.Linq;
using System.Reflection;
using System.Windows;

namespace System.Windows.Controls {

    /// <summary>
    /// Allows specifying the type of the view model to use for a view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ViewModelAttribute : Attribute {

        private static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached("ViewModel", typeof(object), typeof(ViewModelAttribute), null);

        private Type _viewModelType;

        /// <summary>
        /// Initializes an instance of a ViewModelAttribute.
        /// </summary>
        /// <param name="viewModelType">The type of the associated view model.</param>
        public ViewModelAttribute(Type viewModelType) {
            _viewModelType = viewModelType;
        }

        /// <summary>
        /// Gets the type of the associated view model.
        /// </summary>
        public Type ViewModelType {
            get {
                return _viewModelType;
            }
        }

        /// <summary>
        /// Creates a view model for the specified view. This uses the view model type
        /// specified using a ViewModelAttribute if one is specified, and falls back to
        /// convention.
        /// The convention locates a type in the same namespace/assembly as the view, with
        /// a type name formed by adding the 'Model' or 'ViewModel' suffix to the view's type name.
        /// </summary>
        /// <param name="userControl">The control for which the view model must be created.</param>
        /// <returns>The view model instance if one could be located and instantiated.</returns>
        public static object CreateViewModel(UserControl userControl) {
            if (userControl == null) {
                throw new ArgumentNullException("userControl");
            }

            Type viewType = userControl.GetType();
            Type viewModelType = null;

            ViewModelAttribute vmAttribute =
                viewType.GetCustomAttributes(typeof(ViewModelAttribute), /* inherit */ true).
                         OfType<ViewModelAttribute>().FirstOrDefault();
            if (vmAttribute != null) {
                viewModelType = vmAttribute.ViewModelType;
            }

            if (viewModelType == null) {
                string viewModelTypeName = viewType.FullName + "Model";
                viewModelType = viewType.Assembly.GetType(viewModelTypeName, /* throwOnError */ false);

                if ((viewModelType == null) &&
                    (viewType.Name.EndsWith("View", StringComparison.Ordinal) == false)) {
                    viewModelTypeName = viewType.FullName + "ViewModel";
                    viewModelType = viewType.Assembly.GetType(viewModelTypeName, /* throwOnError */ false);
                }
            }

            if (viewModelType != null) {
                if (ComponentContainer.Global != null) {
                    return ComponentContainer.Global.GetObject(viewModelType);
                }

                return Activator.CreateInstance(viewModelType);
            }

            return null;
        }

        /// <summary>
        /// Gets the model associated with the specified framework element.
        /// This will walk up the parent hierarchy to find a model if the specified
        /// element does not have a model.
        /// </summary>
        /// <param name="element">The element to lookup.</param>
        /// <returns>The associated model object if any.</returns>
        public static object GetCurrentViewModel(FrameworkElement element) {
            object model = element.GetValue(ViewModelProperty);

            if (model == null) {
                element = element.GetParentVisual();
                while (element != null) {
                    model = element.GetValue(ViewModelProperty);
                    if (model != null) {
                        break;
                    }

                    element = element.GetParentVisual();
                }
            }

            if (model == null) {
                IApplicationContext appContext = null;
                if (ComponentContainer.Global != null) {
                    appContext = ComponentContainer.Global.GetObject<IApplicationContext>();
                }

                if (appContext != null) {
                    model = appContext.Model;
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the view model instance attached to the specified control.
        /// </summary>
        /// <param name="userControl">The control to lookup.</param>
        /// <returns>The view model if one is associated with the control; null otherwise.</returns>
        public static object GetViewModel(UserControl userControl) {
            return userControl.GetValue(ViewModelProperty);
        }

        /// <summary>
        /// Sets the view model instance attached to the specified control.
        /// The view model is also used as the DataContext assigned to the control.
        /// </summary>
        /// <param name="userControl">The control to associated the view model with.</param>
        /// <param name="value">The view model instance.</param>
        public static void SetViewModel(UserControl userControl, object value) {
            userControl.SetValue(ViewModelProperty, value);
            userControl.DataContext = value;
        }
    }
}
