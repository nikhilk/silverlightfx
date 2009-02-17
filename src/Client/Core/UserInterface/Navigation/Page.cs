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

        /// <summary>
        /// Allows a page to perform initialization upon being activated as a result
        /// of being navigated to. A page may be activated multiple times as a result
        /// of being cached and navigated to again.
        /// </summary>
        /// <param name="firstNavigation">Whether this is the first time the page is being activated.</param>
        protected internal virtual void OnActivated(bool firstNavigation) {
            // TODO: Forward to view model - how?
        }

        /// <summary>
        /// Allows a page to perform any work before it is deactivated as a result
        /// of being navigated away from. A page may cancel deactivation if the
        /// hosting PageFrame supports it. The default implementation does not cancel
        /// deactivation, and lets the PageFrame proceed with navigation.
        /// </summary>
        /// <param name="canCancelNavigation">Whether deactivation can be canceled.</param>
        /// <returns>true if the page can be deactivated; false otherwise.</returns>
        protected internal virtual bool OnDeactivating(bool canCancelNavigation) {
            // TODO: Forward to view model - how?
            return true;
        }
    }
}
