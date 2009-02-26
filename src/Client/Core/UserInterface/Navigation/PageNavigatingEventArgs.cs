// PageNavigatingEventArgs.cs
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

namespace SilverlightFX.UserInterface.Navigation {

    /// <summary>
    /// The event arguments associated with the Navigating event.
    /// </summary>
    public sealed class PageNavigatingEventArgs : CancelEventArgs {

        private bool _canCancel;

        internal PageNavigatingEventArgs(bool canCancel) {
            _canCancel = canCancel;
        }

        /// <summary>
        /// Whether the navigation can be canceled or not.
        /// </summary>
        public bool CanCancel {
            get {
                return _canCancel;
            }
        }
    }
}
