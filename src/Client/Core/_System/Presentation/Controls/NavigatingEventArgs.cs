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
    /// Represents the event information for a Navigating event.
    /// </summary>
    public sealed class NavigatingEventArgs : CancelEventArgs {

        private Uri _uri;
        private bool _canCancel;

        internal NavigatingEventArgs(Uri uri, bool canCancel) {
            _uri = uri;
            _canCancel = canCancel;
        }

        /// <summary>
        /// Gets whether the navigation can be canceled or not.
        /// A navigation might not be cancelable if an external journal
        /// is causing the navigation.
        /// </summary>
        public bool CanCancel {
            get {
                return _canCancel;
            }
        }

        /// <summary>
        /// Gets the URI being navigated to.
        /// </summary>
        public Uri Uri {
            get {
                return _uri;
            }
        }
    }
}
