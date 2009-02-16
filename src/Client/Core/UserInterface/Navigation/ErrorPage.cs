// ErrorPage.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
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

namespace SilverlightFX.UserInterface.Navigation {

    // TODO: Make Error a DependencyProperty with change notifications so that
    //       it can be bound two-way to a view model so as to facilitate
    //       propagating it from the view when the frame sets the property to
    //       the view model.

    /// <summary>
    /// Represents a page that can display error information.
    /// </summary>
    public class ErrorPage : Page {

        private Exception _error;

        /// <summary>
        /// Initializes an instance of a ErrorPage.
        /// </summary>
        public ErrorPage()
            : this(null) {
        }

        /// <summary>
        /// Initializes an instance of a Page with an associated view model.
        /// The view model is set as the DataContext of the Form.
        /// </summary>
        /// <param name="viewModel">The associated view model object.</param>
        public ErrorPage(object viewModel)
            : base(viewModel) {
        }

        /// <summary>
        /// Gets the error associated with this ErrorPage instance.
        /// </summary>
        protected internal Exception Error {
            get {
                return _error;
            }
            internal set {
                _error = value;
            }
        }
    }
}
