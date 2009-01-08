// Window.cs
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
    /// Represents the top-most window in an application.
    /// </summary>
    public class Window : View {

        /// <summary>
        /// Initializes an instance of a Window.
        /// </summary>
        public Window() :
            this(null) {
        }

        /// <summary>
        /// Initializes an instance of a Window with an associated view model.
        /// The view model is set as the DataContext of the Form.
        /// </summary>
        /// <param name="viewModel">The associated view model object.</param>
        public Window(object viewModel)
            : base(viewModel) {
        }

        internal Window(UIElement content) {
            Content = content;
        }
    }
}
