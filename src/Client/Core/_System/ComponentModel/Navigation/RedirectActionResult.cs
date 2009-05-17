// RedirectActionResult.cs
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
    /// Represents a redirect as the result of invoking a controller action.
    /// </summary>
    public sealed class RedirectActionResult : ActionResult {

        private Type _controllerType;
        private string _actionName;
        private string[] _parameters;
        private Dictionary<string, string> _namedParameters;

        internal RedirectActionResult(Type controllerType, string actionName, params string[] parameters) {
            if (controllerType == null) {
                throw new ArgumentNullException("controllerType");
            }
            if (typeof(Controller).IsAssignableFrom(controllerType) == false) {
                throw new ArgumentException("The specified type is not a Controller type.", "controllerType");
            }
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentNullException("actionName");
            }

            _controllerType = controllerType;
            _actionName = actionName;
            _parameters = parameters;
        }

        /// <summary>
        /// Gets the name of the action to redirect to.
        /// </summary>
        public string ActionName {
            get {
                return _actionName;
            }
        }

        /// <summary>
        /// Gets the type of the controller to redirect to.
        /// </summary>
        public Type ControllerType {
            get {
                return _controllerType;
            }
        }

        /// <summary>
        /// Gets whether there are any named parameters.
        /// </summary>
        public bool HasNamedParameters {
            get {
                return _namedParameters != null;
            }
        }

        /// <summary>
        /// Gets the set of named parameters that should be passed in into the redirected
        /// action.
        /// </summary>
        public IDictionary<string, string> NamedParmeters {
            get {
                if (_namedParameters == null) {
                    _namedParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _namedParameters;
            }
        }

        /// <summary>
        /// Gets the list of parameters that should be passed in into the redirected
        /// action.
        /// </summary>
        public string[] Parameters {
            get {
                return _parameters;
            }
        }
    }
}
