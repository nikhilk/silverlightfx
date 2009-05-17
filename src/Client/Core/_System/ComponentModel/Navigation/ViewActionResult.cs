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
using System.Collections.Generic;

namespace System.ComponentModel.Navigation {

    /// <summary>
    /// Represents a view and any view data to initialize an associated view model (if
    /// one exists) as the action result.
    /// </summary>
    public sealed class ViewActionResult : ActionResult {

        private string _viewName;
        private Dictionary<string, object> _viewData;

        /// <summary>
        /// Initializes an instance of a ViewActionResult with the specified view.
        /// </summary>
        /// <param name="viewName">The name of the view to create.</param>
        internal ViewActionResult(string viewName) {
            if (String.IsNullOrEmpty(viewName)) {
                throw new ArgumentNullException("viewName");
            }

            _viewName = viewName;
        }

        /// <summary>
        /// Gets whether there is any view data to initialize the associated
        /// view model with.
        /// </summary>
        public bool HasViewData {
            get {
                return (_viewData != null);
            }
        }

        /// <summary>
        /// Gets the set of name/value pairs representing data to initialize the
        /// associated ViewModel with.
        /// </summary>
        public IDictionary<string, object> ViewData {
            get {
                if (_viewData == null) {
                    _viewData = new Dictionary<string, object>(StringComparer.Ordinal);
                }
                return _viewData;
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
