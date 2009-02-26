// PageNavigatedEventArgs.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.UserInterface.Navigation {

    /// <summary>
    /// The event arguments associated with the Navigated event.
    /// </summary>
    public sealed class PageNavigatedEventArgs : EventArgs {

        private bool _firstNavigation;

        internal PageNavigatedEventArgs(bool firstNavigation) {
            _firstNavigation = firstNavigation;
        }

        /// <summary>
        /// Whether this is the first time a page is being navigated to, or is
        /// being navigated to from a page cache.
        /// </summary>
        public bool IsFirstNavigation {
            get {
                return _firstNavigation;
            }
        }
    }
}
