// DataItemContentControl.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace System.Windows.Data {

    /// <summary>
    /// Represents a single item within a data-bound control.
    /// </summary>
    public abstract class DataItemContentControl : ContentControl, ICommandContainer {

        /// <summary>
        /// Gets a command by name.
        /// </summary>
        /// <param name="commandName">The name of the command to lookup.</param>
        /// <returns>The command if its supported; null otherwise.</returns>
        protected virtual ICommand GetCommand(string commandName) {
            object dataItem = DataContext;
            if (dataItem == null) {
                return null;
            }

            return DataCommand.GetDataItemCommand(dataItem, commandName);
        }

        #region ICommandHandler Members
        ICommand ICommandContainer.GetCommand(string commandName) {
            return GetCommand(commandName);
        }
        #endregion
    }
}
