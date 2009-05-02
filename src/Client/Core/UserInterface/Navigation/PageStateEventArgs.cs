// .cs
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
    /// The event arguments associated with the StateChanged event.
    /// </summary>
    public sealed class PageStateEventArgs : EventArgs {

        private string _state;

        internal PageStateEventArgs(string state) {
            _state = state;
        }

        /// <summary>
        /// Gets the state of the page.
        /// </summary>
        public string State {
            get {
                return _state;
            }
        }
    }
}
