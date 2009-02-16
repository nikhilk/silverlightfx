// Page.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
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

namespace SilverlightFX.UserInterface.Navigation {

    /// <summary>
    /// Represents a page within the application's user interface.
    /// </summary>
    public class Page : View {

        /// <summary>
        /// Represents the Disposition property.
        /// </summary>
        public static readonly DependencyProperty KeepAliveProperty =
            DependencyProperty.Register("KeepAlive", typeof(bool?), typeof(Page),
                                        new PropertyMetadata((bool?)null));

        private Uri _uri;

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

        /// <summary>
        /// Gets or sets the how the page should be cached or preserved when it
        /// is no longer the active page the PageFrame it is hosted in.
        /// True implies it must be preserved. False implies it should not be
        /// preserved. The default, null, implies that the PageFrame can cache
        /// it according to its own policy.
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? KeepAlive {
            get {
                return (bool?)GetValue(KeepAliveProperty);
            }
            set {
                SetValue(KeepAliveProperty, value);
            }
        }

        /// <summary>
        /// Gets the URI associated with this page instance, if the Page is being
        /// hosted within a PageFrame.
        /// </summary>
        public Uri Uri {
            get {
                return _uri;
            }
            internal set {
                _uri = value;
            }
        }
    }
}
