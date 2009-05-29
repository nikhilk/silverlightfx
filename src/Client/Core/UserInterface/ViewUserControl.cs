// ViewUserControl.cs
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
    /// Represents a user control within the application's user interface.
    /// </summary>
    public class ViewUserControl : View {

        /// <summary>
        /// Initializes an instance of a ViewUserControl.
        /// </summary>
        public ViewUserControl() {
        }

        /// <summary>
        /// Initializes an instance of a ViewUserControl with an associated view model.
        /// The view model is set as the DataContext of the Form.
        /// </summary>
        /// <param name="viewModel">The associated view model object.</param>
        public ViewUserControl(object viewModel)
            : base(viewModel) {
        }
    }
}
