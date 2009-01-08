// XButton.cs
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
using System.Windows.Controls;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// An extended Button control with more functionality.
    /// </summary>
    public class XButton : Button {

        /// <summary>
        /// Allows invoking the associated Click event handler programmatically.
        /// </summary>
        public void PerformClick() {
            OnClick();
        }
    }
}
