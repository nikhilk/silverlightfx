// ErrorEventArgs.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.ComponentModel {

    /// <summary>
    /// Provides a standard EventArgs class for error events.
    /// </summary>
    public class ErrorEventArgs : EventArgs {

        private Exception _error;
        private bool _handled;

        /// <summary>
        /// Initializes an instance of an ErrorEventArgs.
        /// </summary>
        /// <param name="error">The exception containing error information.</param>
        public ErrorEventArgs(Exception error) {
            if (error == null) {
                throw new ArgumentNullException("error");
            }
            _error = error;
        }

        /// <summary>
        /// Gets the error information contained within this event.
        /// </summary>
        public Exception Error {
            get {
                return _error;
            }
        }

        /// <summary>
        /// Gets whether the error has been handled.
        /// </summary>
        public bool IsHandled {
            get {
                return _handled;
            }
        }

        /// <summary>
        /// Marks the error has handled.
        /// </summary>
        public void MarkAsHandled() {
            _handled = true;
        }
    }
}
