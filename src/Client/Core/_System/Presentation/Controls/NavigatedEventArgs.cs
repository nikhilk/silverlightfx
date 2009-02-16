// NavigatingEventArgs.cs
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

namespace System.Windows.Controls {

    /// <summary>
    /// Represents the event information for a Navigated event.
    /// </summary>
    public sealed class NavigatedEventArgs : EventArgs {

        private Uri _uri;
        private bool _hasError;

        internal NavigatedEventArgs(Uri uri, bool hasError) {
            _uri = uri;
            _hasError = hasError;
        }

        /// <summary>
        /// Gets whether an error was encountered navigating to the URI.
        /// </summary>
        public bool HasError {
            get {
                return _hasError;
            }
        }

        /// <summary>
        /// Gets the URI that was navigated to.
        /// </summary>
        public Uri Uri {
            get {
                return _uri;
            }
        }
    }
}
