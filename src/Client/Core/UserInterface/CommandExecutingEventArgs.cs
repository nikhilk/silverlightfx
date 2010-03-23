// CommandExecutingEventArgs.cs
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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// The event argument associated with the Executing event of a Command.
    /// </summary>
    public sealed class CommandExecutingEventArgs : CancelEventArgs {

        private object _parameter;

        internal CommandExecutingEventArgs(object parameter) {
            _parameter = parameter;
        }

        /// <summary>
        /// Gets ths parameter value associated with the command execution.
        /// </summary>
        public object Parameter {
            get {
                return _parameter;
            }
        }
    }
}
