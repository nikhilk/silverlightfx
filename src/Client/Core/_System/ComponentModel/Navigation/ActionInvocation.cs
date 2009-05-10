// ActionInvocation.cs
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
    /// Represents an action being invoked.
    /// </summary>
    public sealed class ActionInvocation {

        private string _actionName;
        private List<object> _parameters;
        private Dictionary<string, string> _namedParameters;

        /// <summary>
        /// Initializes an ActionInvocation with the name of the action to be invoked.
        /// </summary>
        /// <param name="actionName">The name of the action to be invoked.</param>
        public ActionInvocation(string actionName) {
            if (String.IsNullOrEmpty(actionName)) {
                throw new ArgumentNullException("actionName");
            }

            _actionName = actionName;
            _parameters = new List<object>();
            _namedParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the name of the action to be invoked.
        /// </summary>
        public string ActionName {
            get {
                return _actionName;
            }
        }

        /// <summary>
        /// Gets the list of named parameters to be supplied to the action when it
        /// is invoked.
        /// </summary>
        public IDictionary<string, string> NamedParameters {
            get {
                return _namedParameters;
            }
        }

        /// <summary>
        /// Gets the list of parameters to be supplied to the action when it is invoked.
        /// </summary>
        public IList<object> Parameters {
            get {
                return _parameters;
            }
        }
    }
}
