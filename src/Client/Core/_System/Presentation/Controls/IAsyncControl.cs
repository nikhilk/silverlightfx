// IAsyncControl.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
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
    /// Represents a control that performs asynchronous work that can
    /// be tracked.
    /// </summary>
    public interface IAsyncControl {

        /// <summary>
        /// The current async activity being performed by the control.
        /// </summary>
        Async AsyncActivity {
            get;
        }

        /// <summary>
        /// Indicates that the current async activity has changed.
        /// </summary>
        event EventHandler AsyncActivityChanged;
    }
}
