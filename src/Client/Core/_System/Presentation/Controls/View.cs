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
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace System.Windows.Controls {

    /// <summary>
    /// Represents a base class for views that are associated with a Model.
    /// The associated model is surfaced as the View's DataContext.
    /// </summary>
    public abstract class View : UserControl {

        /// <summary>
        /// Represents the Model attached property.
        /// </summary>
        private static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached("Model", typeof(object), typeof(View), null);

        private Type _modelType;

        /// <summary>
        /// Initializes an instance of a View.
        /// </summary>
        protected View() {
        }

        /// <summary>
        /// Initializes an instance of a View with the specified model.
        /// </summary>
        /// <param name="viewModel">The associated model.</param>
        protected View(object viewModel) {
            if (viewModel != null) {
                Model = viewModel;
            }
        }

        /// <summary>
        /// Gets or sets the model associated with the view.
        /// </summary>
        public object Model {
            get {
                return DataContext;
            }
            set {
                SetValue(ModelProperty, value);
                DataContext = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the model associated with the view.
        /// This is used to create an instance of a model via the container
        /// associated with the current application.
        /// </summary>
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type ModelType {
            get {
                if (_modelType != null) {
                    return _modelType;
                }
                if (DataContext != null) {
                    return DataContext.GetType();
                }
                return null;
            }
            set {
                _modelType = value;
                if ((_modelType != null) && (DataContext == null)) {
                    Model = CreateModel(_modelType);
                }
            }
        }

        private static object CreateModel(Type type) {
            if (ComponentContainer.Global != null) {
                return ComponentContainer.Global.GetObject(type);
            }

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the model associated with the specified framework element.
        /// This will walk up the parent hierarchy to find a model if the specified
        /// element does not have a model.
        /// </summary>
        /// <param name="element">The element to lookup.</param>
        /// <returns>The associated model object if any.</returns>
        public static object GetModel(FrameworkElement element) {
            object model = element.GetValue(ModelProperty);

            if (model == null) {
                element = element.GetParentVisual();
                while (element != null) {
                    model = element.GetValue(ModelProperty);
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
    }
}
