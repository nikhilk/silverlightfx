// CancelEventArgs.cs
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
    /// Represents the event argument associated with a cancelable action.
    /// </summary>
    public class CancelEventArgs : EventArgs {

        private bool _canceled;

        /// <summary>
        /// Whether the action is to be canceled.
        /// </summary>
        public bool Canceled {
            get {
                return _canceled;
            }
            set {
                _canceled = value;
            }
        }
    }

    /// <summary>
    /// Represents the signature of an cancelable event handler.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event argument associated with the event.</param>
    public delegate void CancelEventHandler(object sender, CancelEventArgs e);
}
