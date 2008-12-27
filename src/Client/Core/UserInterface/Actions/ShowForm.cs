// ShowForm.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
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
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that transitions from one visual state to another.
    /// </summary>
    public sealed class ShowForm : TriggerAction<FrameworkElement> {

        /// <summary>
        /// Represents the FormModel property.
        /// </summary>
        public static readonly DependencyProperty FormModelProperty =
            DependencyProperty.Register("FormModel", typeof(object), typeof(ShowForm), null);

        /// <summary>
        /// Represents the FormType property.
        /// </summary>
        public static readonly DependencyProperty FormTypeProperty =
            DependencyProperty.Register("FormType", typeof(Type), typeof(ShowForm), null);

        /// <summary>
        /// Initializes an instance of a ShowForm action.
        /// </summary>
        public ShowForm() {
        }

        /// <summary>
        /// Gets or sets the model to associate with the Form when it is shown.
        /// </summary>
        public object FormModel {
            get {
                return GetValue(FormModelProperty);
            }
            set {
                SetValue(FormModelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Form to show.
        /// </summary>
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type FormType {
            get {
                return (Type)GetValue(FormTypeProperty);
            }
            set {
                SetValue(FormTypeProperty, value);
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            Type formType = FormType;
            if (formType == null) {
                throw new InvalidOperationException("The FormType property on a ShowForm action must be set.");
            }

            if (typeof(Form).IsAssignableFrom(formType) == false) {
                throw new InvalidOperationException("The FormType property specified must represent a Form.");
            }

            Form form = null;
            object formModel = FormModel;

            if (formModel == null) {
                form = (Form)Activator.CreateInstance(formType);
            }
            else {
                form = (Form)Activator.CreateInstance(formType, formModel);
            }

            form.Show();
        }
    }
}
