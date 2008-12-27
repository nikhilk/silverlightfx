// Page.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
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
    /// Represents a page within the application's user interface.
    /// </summary>
    public class Page : View {

        /// <summary>
        /// Initializes an instance of a Page.
        /// </summary>
        public Page()
            : this(null) {
        }

        /// <summary>
        /// Initializes an instance of a Page with an associated view model.
        /// The view model is set as the DataContext of the Form.
        /// </summary>
        /// <param name="viewModel">The associated view model object.</param>
        public Page(object viewModel)
            : base(viewModel) {
        }
    }
}
