// FormResult.cs
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
using System.Windows;
using System.Windows.Controls;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents the result of a Form instance when it is closed.
    /// </summary>
    public enum FormResult {

        /// <summary>
        /// The Form was closed by clicking its OK button.
        /// </summary>
        OK = 0,

        /// <summary>
        /// The Form was closed by clicking its Cancel button.
        /// </summary>
        Cancel = 1
    }
}
