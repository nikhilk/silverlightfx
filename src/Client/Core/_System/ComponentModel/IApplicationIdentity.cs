// IApplicationIdentity.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;

namespace System.ComponentModel {

    /// <summary>
    /// Defines an application's identity.
    /// </summary>
    public interface IApplicationIdentity {

        /// <summary>
        /// Gets the model associated with the application instance.
        /// </summary>
        object Model {
            get;
        }

        /// <summary>
        /// Any startup arguments associated with the application instance.
        /// </summary>
        IDictionary<string, string> StartupArguments {
            get;
        }

        /// <summary>
        /// The source URI associated with the application instance.
        /// </summary>
        Uri Uri {
            get;
        }
    }
}
