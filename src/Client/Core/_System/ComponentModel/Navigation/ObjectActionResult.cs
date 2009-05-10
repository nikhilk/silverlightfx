// ObjectActionResult.cs
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
    /// Represents an arbitrary object as the action result.
    /// </summary>
    public sealed class ObjectActionResult : ActionResult {

        private object _value;

        /// <summary>
        /// Initializes an instance of an ObjectActionResult with the specified value.
        /// </summary>
        /// <param name="value">The value representing the result.</param>
        public ObjectActionResult(object value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }

            _value = value;
        }

        /// <summary>
        /// Gets the value representing the result of an action.
        /// </summary>
        public object Value {
            get {
                return _value;
            }
        }
    }
}
