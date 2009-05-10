// ViewActionResult.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.ComponentModel.Navigation {

    /// <summary>
    /// Represents a view and optionally its associated view model as the action result.
    /// </summary>
    public sealed class ViewActionResult : ActionResult {

        private string _viewName;
        private object _viewModel;

        /// <summary>
        /// Initializes an instance of a ViewActionResult with the specified view.
        /// </summary>
        /// <param name="viewName">The name of the view to create.</param>
        public ViewActionResult(string viewName)
            : this(viewName, null) {
        }

        /// <summary>
        /// Initializes an instance of a ViewActionResult with the specified view
        /// and associated view model.
        /// </summary>
        /// <param name="viewName">The name of the view to create.</param>
        /// <param name="viewModel">The view model to associated with the view.</param>
        public ViewActionResult(string viewName, object viewModel) {
            if (String.IsNullOrEmpty(viewName)) {
                throw new ArgumentNullException("viewName");
            }

            _viewName = viewName;
            _viewModel = viewModel;
        }

        /// <summary>
        /// Gets the view model if one is available that corresponds to the view.
        /// </summary>
        public object ViewModel {
            get {
                return _viewModel;
            }
        }

        /// <summary>
        /// Gets the name of the view to create to represent the action result.
        /// </summary>
        public string ViewName {
            get {
                return _viewName;
            }
        }
    }
}
