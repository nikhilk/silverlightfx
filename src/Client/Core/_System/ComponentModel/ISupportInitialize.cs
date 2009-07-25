// ISupportInitialize.cs
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
    /// Implemented by objects that supports a simple, transacted notification for batch
    /// initialization. 
    /// </summary>
    public interface ISupportInitialize {

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void BeginInit();

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void EndInit();
    }
}
