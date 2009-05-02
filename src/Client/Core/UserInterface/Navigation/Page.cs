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
        public static readonly DependencyProperty CacheProperty =
            DependencyProperty.Register("Cache", typeof(bool?), typeof(Page),
                                        new PropertyMetadata((bool?)null));

        private Uri _uri;
        private Uri _originalUri;

        private EventHandler<PageNavigatedEventArgs> _navigatedHandler;
        private EventHandler<PageNavigatingEventArgs> _navigatingHandler;
        private EventHandler<PageStateEventArgs> _stateChangedHandler;

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
        public bool? Cache {
            get {
                return (bool?)GetValue(CacheProperty);
            }
            set {
                SetValue(CacheProperty, value);
            }
        }

        internal Uri OriginalUri {
            get {
                return _originalUri;
            }
            set {
                _originalUri = value;
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
        /// Raised when the page has been navigated to.
        /// </summary>
        public event EventHandler<PageNavigatedEventArgs> Navigated {
            add {
                _navigatedHandler = (EventHandler<PageNavigatedEventArgs>)Delegate.Combine(_navigatedHandler, value);
            }
            remove {
                _navigatedHandler = (EventHandler<PageNavigatedEventArgs>)Delegate.Remove(_navigatedHandler, value);
            }
        }

        /// <summary>
        /// Raised when the page is being navigated away from.
        /// </summary>
        public event EventHandler<PageNavigatingEventArgs> Navigating {
            add {
                _navigatingHandler = (EventHandler<PageNavigatingEventArgs>)Delegate.Combine(_navigatingHandler, value);
            }
            remove {
                _navigatingHandler = (EventHandler<PageNavigatingEventArgs>)Delegate.Remove(_navigatingHandler, value);
            }
        }

        /// <summary>
        /// Raised when the page state has changed.
        /// </summary>
        public event EventHandler<PageStateEventArgs> StateChanged {
            add {
                _stateChangedHandler = (EventHandler<PageStateEventArgs>)Delegate.Combine(_stateChangedHandler, value);
            }
            remove {
                _stateChangedHandler = (EventHandler<PageStateEventArgs>)Delegate.Remove(_stateChangedHandler, value);
            }
        }

        /// <summary>
        /// Raises the Navigated event.
        /// </summary>
        /// <param name="e">The data associated with the event.</param>
        protected internal virtual void OnNavigated(PageNavigatedEventArgs e) {
            if (_navigatedHandler != null) {
                _navigatedHandler(this, e);
            }
        }

        /// <summary>
        /// Raises the Navigating event.
        /// </summary>
        /// <param name="e">The data associated with the event.</param>
        protected internal virtual void OnNavigating(PageNavigatingEventArgs e) {
            if (e.CanCancel && e.Canceled) {
                return;
            }
            if (_navigatingHandler != null) {
                _navigatingHandler(this, e);
            }
        }

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        /// <param name="e">The data associated with the event.</param>
        protected internal virtual void OnStateChanged(PageStateEventArgs e) {
            if (_stateChangedHandler != null) {
                _stateChangedHandler(this, e);
            }
        }
    }
}
