// ICommandContainer.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;

namespace System.Windows.Input {

    /// <summary>
    /// Represents a object that contains commands that can be looked up
    /// dynamically.
    /// </summary>
    public interface ICommandContainer {

        /// <summary>
        /// Returns the command corresponding to the specified command name.
        /// </summary>
        /// <param name="commandName">The name of command.</param>
        /// <returns>The command if it exists; null otherwise.</returns>
        ICommand GetCommand(string commandName);
    }
}
