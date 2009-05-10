// ErrorActionResult.cs
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
    /// Represents an error as the result of invoking a controller action.
    /// </summary>
    public sealed class ErrorActionResult : ActionResult {

        private Exception _error;

        /// <summary>
        /// Initializes an instance of a ErrorActionResult with the specified error.
        /// </summary>
        /// <param name="error">The error that occurred..</param>
        public ErrorActionResult(Exception error) {
            if (error == null) {
                throw new ArgumentNullException("error");
            }

            _error = error;
        }

        /// <summary>
        /// Gets the error that occurred.
        /// </summary>
        public Exception Error {
            get {
                return _error;
            }
        }
    }
}
