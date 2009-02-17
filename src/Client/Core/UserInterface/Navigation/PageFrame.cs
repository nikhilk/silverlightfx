// PageFrame.cs
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
using System.Windows.Input;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Navigation {

    /// <summary>
    /// A derived ContentControl that supports transitions to animate from
    /// initial content to another content.
    /// </summary>
    [TemplatePart(Name = "ContentView", Type = typeof(ContentView))]
    [TemplateVisualState(GroupName = "NavigationStates", Name = "Navigating")]
    [TemplateVisualState(GroupName = "NavigationStates", Name = "Navigated")]
    public class PageFrame : Control, INavigationTarget {

        /// <summary>
        /// Represents the DefaultUri property.
        /// </summary>
        public static readonly DependencyProperty DefaultUriProperty =
            DependencyProperty.Register("DefaultUri", typeof(Uri), typeof(PageFrame),
                                        new PropertyMetadata(new Uri("Default", UriKind.Relative)));

        /// <summary>
        /// Represents the ErrorPageType property.
        /// </summary>
        public static readonly DependencyProperty ErrorPageTypeProperty =
            DependencyProperty.Register("ErrorPageType", typeof(Type), typeof(PageFrame), null);

        /// <summary>
        /// Represents the Transition property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(Transition), typeof(PageFrame), null);

        /// <summary>
        /// Represents the Uri property.
        /// </summary>
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(Uri), typeof(PageFrame),
                                        new PropertyMetadata(OnUriPropertyChanged));

        private bool _loaded;
        private bool _redirecting;
        private ContentView _contentView;
        private PageLoader _loader;
        private PageJournal _journal;
        private PageCache _cache;

        private DelegateCommand _backCommand;
        private DelegateCommand _forwardCommand;

        private EventHandler<NavigatingEventArgs> _navigatingHandler;
        private EventHandler<NavigatedEventArgs> _navigatedHandler;

        private IAsyncResult _navigateResult;

        /// <summary>
        /// Initializes an instance of a PageFrame.
        /// </summary>
        public PageFrame() {
            _journal = new PageJournal();
            _cache = new PageCache(10);

            _backCommand = new DelegateCommand(OnBackCommand, /* canExecute */ false);
            _forwardCommand = new DelegateCommand(OnForwardCommand, /* canExecute */ false);

            Resources.Add("BackCommand", _backCommand);
            Resources.Add("ForwardCommand", _forwardCommand);

            DefaultStyleKey = typeof(PageFrame);
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Gets or sets the default URI to navigate to when the Uri property
        /// is not set. By default, this is simply "Default".
        /// </summary>
        public Uri DefaultUri {
            get {
                return (Uri)GetValue(DefaultUriProperty);
            }
            set {
                SetValue(DefaultUriProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the error page to use when there is an error.
        /// If there is no error page, a default one is used.
        /// </summary>
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type ErrorPageType {
            get {
                return (Type)GetValue(ErrorPageTypeProperty);
            }
            set {
                SetValue(ErrorPageTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the loader used to load pages to be shown in the PageFrame.
        /// </summary>
        public PageLoader Loader {
            get {
                return _loader;
            }
            set {
                if (_loaded) {
                    throw new InvalidOperationException("Loader can only be set declaratively.");
                }

                _loader = value;
                if (_loader != null) {
                    _loader.Initialize(this);
                }
            }
        }

        /// <summary>
        /// Gets the currently loaded page if there is one; null otherwise.
        /// </summary>
        public Page Page {
            get {
                if (_contentView != null) {
                    return (Page)_contentView.Content;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the transition to play when navigating from one page to another.
        /// </summary>
        public Transition Transition {
            get {
                return (Transition)GetValue(TransitionProperty);
            }
            set {
                SetValue(TransitionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current Uri property.
        /// </summary>
        public Uri Uri {
            get {
                return (Uri)GetValue(UriProperty);
            }
        }

        /// <summary>
        /// Raised when a new Uri has been navigated to by the frame.
        /// </summary>
        public event EventHandler<NavigatedEventArgs> Navigated {
            add {
                _navigatedHandler = (EventHandler<NavigatedEventArgs>)Delegate.Combine(_navigatedHandler, value);
            }
            remove {
                _navigatedHandler = (EventHandler<NavigatedEventArgs>)Delegate.Remove(_navigatedHandler, value);
            }
        }

        /// <summary>
        /// Raised when a new Uri is being navigated to by the frame.
        /// </summary>
        public event EventHandler<NavigatingEventArgs> Navigating {
            add {
                _navigatingHandler = (EventHandler<NavigatingEventArgs>)Delegate.Combine(_navigatingHandler, value);
            }
            remove {
                _navigatingHandler = (EventHandler<NavigatingEventArgs>)Delegate.Remove(_navigatingHandler, value);
            }
        }

        private ErrorPage GetErrorPage(Exception error) {
            Type errorPageType = ErrorPageType;
            if ((errorPageType != null) && typeof(ErrorPage).IsAssignableFrom(errorPageType)) {
                ErrorPage errorPage = (ErrorPage)Activator.CreateInstance(errorPageType);
                errorPage.Error = error;

                return errorPage;
            }

            return new ErrorPage() {
                Error = error
            };
        }

        private void InitiateNavigation(NavigationState navigationState) {
            try {
                _navigateResult = _loader.BeginLoadPage(navigationState.uri, Page, OnPageLoadCallback, navigationState);
            }
            catch (Exception e) {
                Page page = GetErrorPage(e);

                Dispatcher.BeginInvoke(delegate() {
                    OnNavigationCompleted(navigationState, page);
                });
            }
        }

        /// <summary>
        /// Naviates the frame to the specified URI.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        public void Navigate(Uri uri) {
            SetValue(UriProperty, uri);
        }

        private void NavigateInternal(NavigationState navigationState) {
            if (_contentView == null) {
                return;
            }

            // TODO: False if we don't own journal - later
            bool canCancel = true;

            Page currentPage = Page;
            if (currentPage != null) {
                bool cancel = !currentPage.OnDeactivating(canCancel);
                if (canCancel && cancel) {
                    return;
                }
            }

            NavigatingEventArgs ne = new NavigatingEventArgs(navigationState.uri, canCancel);
            OnNavigating(ne);
            if (ne.CanCancel && ne.Canceled) {
                return;
            }

            Page page = _cache.GetPage(navigationState.uri);
            if (page != null) {
                navigationState.cachedPage = true;
                Dispatcher.BeginInvoke(delegate() {
                    OnNavigationCompleted(navigationState, page);
                });
                return;
            }

            VisualStateManager.GoToState(this, "Navigating", /* useTransitions */ true);
            InitiateNavigation(navigationState);
        }

        private void NavigateInternal(Page page) {
            _contentView.Content = page;
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _contentView = (ContentView)GetTemplateChild("ContentView");
            if (_loaded) {
                NavigateInternal(new NavigationState(Uri));
            }
        }

        private void OnBackCommand() {
            Uri backUri = _journal.GoBack();
            if (backUri != null) {
                NavigateInternal(new NavigationState(backUri) { journalNavigation = false });
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (_loader == null) {
                Loader = PageLoader.Default;
            }
            if (Uri == null) {
                SetValue(UriProperty, DefaultUri);
            }

            ApplyTemplate();

            _loaded = true;
            NavigateInternal(new NavigationState(Uri));
        }

        private void OnForwardCommand() {
            Uri forwardUri = _journal.GoForward();
            if (forwardUri != null) {
                NavigateInternal(new NavigationState(forwardUri) { journalNavigation = false });
            }
        }

        /// <summary>
        /// Raises the Navigated event.
        /// </summary>
        /// <param name="e">The data associated with the event.</param>
        protected virtual void OnNavigated(NavigatedEventArgs e) {
            if (_navigatedHandler != null) {
                _navigatedHandler(this, e);
            }
        }

        /// <summary>
        /// Raises the Navigating event.
        /// </summary>
        /// <param name="e">The data associated with the event.</param>
        protected virtual void OnNavigating(NavigatingEventArgs e) {
            if (_navigatingHandler != null) {
                _navigatingHandler(this, e);
            }
        }

        private void OnNavigationCompleted(NavigationState navigationState, Page page) {
            Page currentPage = Page;
            if (currentPage != null) {
                _cache.RemovePageReference(currentPage);
            }

            page.Uri = navigationState.uri;

            if (navigationState.cachedPage == false) {
                if (!(page is ErrorPage)) {
                    _cache.AddPageReference(page);
                }
                VisualStateManager.GoToState(this, "Navigated", /* useTransitions */ true);
            }
            NavigateInternal(page);
            if (navigationState.journalNavigation) {
                _journal.AddEntry(navigationState.uri);
            }

            _backCommand.UpdateStatus(_journal.CanGoBack);
            _forwardCommand.UpdateStatus(_journal.CanGoForward);

            page.OnActivated(!navigationState.cachedPage);

            OnNavigated(new NavigatedEventArgs(navigationState.uri, page is ErrorPage));
        }

        private void OnPageLoadCallback(IAsyncResult asyncResult) {
            if (asyncResult != _navigateResult) {
                return;
            }

            Uri redirectUri = null;

            NavigationState navigationState = (NavigationState)asyncResult.AsyncState;
            Page page = null;
            try {
                page = _loader.EndLoadPage(asyncResult, out redirectUri);
            }
            catch (Exception e) {
                page = GetErrorPage(e);
            }

            if (redirectUri == null) {
                OnNavigationCompleted(navigationState, page);
            }
            else {
                Dispatcher.BeginInvoke(delegate() {
                    _redirecting = true;
                    SetValue(UriProperty, redirectUri);
                    _redirecting = false;

                    navigationState.uri = redirectUri;
                    InitiateNavigation(navigationState);
                });
            }
        }

        private static void OnUriPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            PageFrame frame = (PageFrame)o;
            if (frame._loaded && !frame._redirecting) {
                frame.NavigateInternal(new NavigationState((Uri)e.NewValue));
            }
        }

        #region INavigationTarget Members
        Uri INavigationTarget.Uri {
            get {
                return Uri;
            }
        }

        event EventHandler<NavigatedEventArgs> INavigationTarget.Navigated {
            add {
                Navigated += value;
            }
            remove {
                Navigated -= value;
            }
        }

        event EventHandler<NavigatingEventArgs> INavigationTarget.Navigating {
            add {
                Navigating += value;
            }
            remove {
                Navigating -= value;
            }
        }

        void INavigationTarget.Navigate(Uri uri) {
            Navigate(uri);
        }
        #endregion


        private sealed class NavigationState {

            public Uri uri;
            public bool journalNavigation;
            public bool cachedPage;

            public NavigationState(Uri uri) {
                this.uri = uri;
                this.journalNavigation = true;
            }
        }
    }
}
