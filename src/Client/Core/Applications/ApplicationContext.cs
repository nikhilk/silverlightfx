// ApplicationContext.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using SilverlightFX.UserInterface;

namespace SilverlightFX.Applications {

    /// <summary>
    /// Represents Application-level functionality including support including the
    /// main window, theming, settings, components and composition and more.
    /// </summary>
    [Service(typeof(IApplicationContext))]
    [Service(typeof(IExternalNavigationService))]
    [ContentProperty("Components")]
    public class ApplicationContext : IApplicationService, IApplicationLifetimeAware, IServiceProvider, IApplicationContext, IExternalNavigationService {

        private static ApplicationContext _current;

        private SynchronizationContext _uiContext;

        private IDictionary<string, string> _startupArguments;
        private string _themeName;
        private string _windowName;
        private Style _screenStyle;
        private IComponentContainer _componentContainer;
        private ComponentCollection _components;
        private object _model;

        private Window _mainWindow;
        private bool _started;

        /// <summary>
        /// Initializes an instance of ApplicationContext.
        /// </summary>
        public ApplicationContext() {
            _current = this;

            _componentContainer = new ComponentContainer(this);
            _componentContainer.RegisterObject(this);

            _uiContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Gets the collection of components declared at the application level.
        /// </summary>
        public ComponentCollection Components {
            get {
                if (_components == null) {
                    _components = new ComponentCollection();
                    _components.CollectionChanged += OnComponentCollectionChanged;
                }
                return _components;
            }
        }

        /// <summary>
        /// Gets the current ApplicationContext instance for the running application.
        /// </summary>
        public static ApplicationContext Current {
            get {
                return _current;
            }
        }

        /// <summary>
        /// Gets or sets the application-wide model.
        /// </summary>
        public object Model {
            get {
                return _model;
            }
            set {
                _model = value;
            }
        }

        /// <summary>
        /// The style applied to the screen or the root visual of the application.
        /// </summary>
        public Style ScreenStyle {
            get {
                return _screenStyle;
            }
            set {
                EnsureUnstarted();
                _screenStyle = value;
            }
        }

        /// <summary>
        /// Gets the settings associated with the running application.
        /// </summary>
        public IsolatedStorageSettings Settings {
            get {
                return IsolatedStorageSettings.ApplicationSettings;
            }
        }

        /// <summary>
        /// Gets the settings associated with the running application that are shared
        /// amongst other applications on the same site.
        /// </summary>
        public IsolatedStorageSettings SiteSettings {
            get {
                return IsolatedStorageSettings.SiteSettings;
            }
        }

        /// <summary>
        /// Gets the list of name/value pairs passed in into the application as initialization
        /// or startup arguments.
        /// </summary>
        public IDictionary<string, string> StartupArguments {
            get {
                return _startupArguments;
            }
        }

        /// <summary>
        /// Gets or sets the name of the selected theme. The name of the theme is used to construct
        /// a URI of the form: Themes/{name}/Theme.xaml which is then loaded to load the
        /// theme and add the theme items to the associated Resources dictionary.
        /// The name can either be a simple identifier, or be of the form $initParamName|defaultName,
        /// which allows selection of the theme from within the HTML page.
        /// </summary>
        public string ThemeName {
            get {
                return _themeName ?? String.Empty;
            }
            set {
                EnsureUnstarted();
                _themeName = value;
            }
        }

        /// <summary>
        /// Represents the SynchronizationContext representing the main UI thread associated
        /// with the application.
        /// </summary>
        public SynchronizationContext UISynchronizationContext {
            get {
                return _uiContext;
            }
        }

        /// <summary>
        /// Gets the URI that represents the identity of this application.
        /// </summary>
        public Uri Uri {
            get {
                return Application.Current.Host.Source;
            }
        }

        /// <summary>
        /// Gets the top-level window associated with the application.
        /// </summary>
        public Window Window {
            get {
                return _mainWindow;
            }
        }

        /// <summary>
        /// Gets or sets the type name of the top-level window. The name is used to instantiate
        /// the top-level window on startup.
        /// The name can be one of the following:
        /// - A simple identifier which is treated as the name of a type alongside the application
        /// - A namespace qualified type name treated as the name of a type in the application assembly
        /// - An assembly qualified type name
        /// - $initParamName|defaultName which allows the selection of a type name from the HTML page
        /// </summary>
        public string WindowName {
            get {
                return _windowName ?? String.Empty;
            }
            set {
                EnsureUnstarted();
                _windowName = value;
            }
        }

        internal void EnsureUnstarted() {
            if (_started) {
                throw new InvalidOperationException("This property can only be set declaratively in XAML or in the Application's constructor.");
            }
        }

        /// <summary>
        /// Gets a service that implements the specified service contract.
        /// </summary>
        /// <param name="serviceType">The type representing the service contract.</param>
        /// <returns>A service instance if the service is available; null otherwise.</returns>
        protected virtual object GetService(Type serviceType) {
            return null;
        }

        private void InitializeTheme() {
            if (String.IsNullOrEmpty(_themeName)) {
                return;
            }

            string name = null;

            if (_themeName.StartsWith("$")) {
                string[] nameParts = _themeName.Split('|');
                if ((nameParts.Length > 2) || String.IsNullOrEmpty(nameParts[0])) {
                    throw new InvalidOperationException("Invalid theme name. Either the name must be a simple name, or must be in the form $<ThemeInitParam>|<DefaultName>.");
                }

                if (nameParts.Length == 2) {
                    name = nameParts[1];
                }
                if (_startupArguments != null) {
                    string selectedName;
                    string argName = nameParts[0].Substring(1);
                    if (_startupArguments.TryGetValue(argName, out selectedName)) {
                        name = selectedName;
                    }
                }
            }
            else {
                name = _themeName;
            }

            if (String.IsNullOrEmpty(name)) {
                return;
            }

            Theme.LoadTheme(Application.Current.Resources, name);
        }

        private void InitializeWindow() {
            if (String.IsNullOrEmpty(_windowName)) {
                return;
            }

            string name = null;

            if (_windowName.StartsWith("$")) {
                string[] nameParts = _windowName.Split('|');
                if ((nameParts.Length > 2) || String.IsNullOrEmpty(nameParts[0])) {
                    throw new InvalidOperationException("Invalid window name. Either the name must be a type name, or must be in the form $<WindowNameParam>|<DefaultName>.");
                }

                if (nameParts.Length == 2) {
                    name = nameParts[1];
                }
                if (_startupArguments != null) {
                    string selectedName;
                    string argName = nameParts[0].Substring(1);
                    if (_startupArguments.TryGetValue(argName, out selectedName)) {
                        name = selectedName;
                    }
                }
            }
            else {
                name = _windowName;
            }

            Window mainWindow = null;
            if (String.IsNullOrEmpty(name) == false) {
                try {
                    Type windowType = TypeTypeConverter.ParseTypeName(Application.Current, name);
                    if (windowType == null) {
                        throw new InvalidOperationException("The window named '" + name + "' could not be found.");
                    }
                    if (typeof(UIElement).IsAssignableFrom(windowType) == false) {
                        throw new InvalidOperationException("The window named '" + name + "' does not derive from UIElement.");
                    }

                    UIElement uiElement = (UIElement)Activator.CreateInstance(windowType);
                    mainWindow = uiElement as Window;

                    if (mainWindow == null) {
                        mainWindow = new Window(uiElement);
                    }
                }
                catch (Exception e) {
                    if (e is TargetInvocationException) {
                        e = e.InnerException;
                    }

                    TextBlock errorText = new TextBlock();
                    errorText.Inlines.Add(new Run() {
                        Text = "The window named '" + name + "' could not be instantiated.",
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Red)
                    });
                    errorText.Inlines.Add(new LineBreak());
                    errorText.Inlines.Add(new Run() {
                        Text = e.Message
                    });

                    mainWindow = new Window(errorText);
                    if (Debugger.IsAttached) {
                        throw;
                    }
                }
            }

            if (mainWindow != null) {
                Run(mainWindow);
            }
        }

        private void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
            e.Handled = OnError(e.ExceptionObject);
        }

        /// <summary>
        /// Allows the application to perform any processing work necessary before the application closes.
        /// </summary>
        protected virtual void OnClosing() {
        }

        private void OnComponentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                object component = e.NewItems[0];

                _componentContainer.InitializeObject(component);
                _componentContainer.RegisterObject(component);
            }
        }

        /// <summary>
        /// Allows the application to perform custom error handling of unhandled exceptions.
        /// </summary>
        /// <param name="unhandledException">The exception that was not handled.</param>
        /// <returns>true if the exception was handled; false otherwise.</returns>
        protected virtual bool OnError(Exception unhandledException) {
            if (Debugger.IsAttached == false) {
                if (HtmlPage.IsEnabled) {
                    // Get the actual user application type, hence GetType()
                    Type appType = this.GetType();
                    Assembly appAssembly = appType.Assembly;

                    object[] attrs = appAssembly.GetCustomAttributes(typeof(DebuggableAttribute), /* inherit */ false);
                    bool isDebuggable = ((attrs != null) && (attrs.Length != 0));

                    try {
                        string errorMessage = unhandledException.Message + unhandledException.StackTrace;
                        errorMessage = errorMessage.Replace("\r\n", "\n");

                        HtmlPage.Window.Eval("throw new Error('Unhandled Error: " + errorMessage + "');");
                    }
                    catch {
                    }
                }

                // If the app is not running in the debugger, then handle the error, so it doesn't
                // stop the app. The assumption is the app has done what it needs to do to handle
                // the error including stopping itself by overriding OnError.
                return true;
            }

            return false;
        }

        /// <summary>
        /// Allows the application to perform any processing work before the application starts.
        /// </summary>
        protected virtual void OnStarting() {
            InitializeTheme();
            InitializeWindow();
        }

        /// <summary>
        /// Runs the application using the specified main window as the top-most user interface element.
        /// </summary>
        /// <param name="mainWindow">The main window to use to represent the application's user interface.</param>
        public void Run(Window mainWindow) {
            if (mainWindow == null) {
                throw new ArgumentNullException("mainWindow");
            }
            if (_mainWindow != null) {
                throw new InvalidOperationException("Run can only be called once.");
            }

            _mainWindow = mainWindow;

            Screen screen = new Screen();
            screen.Style = ScreenStyle;

            Application.Current.RootVisual = screen;
            screen.Run(mainWindow);
        }


        #region Implementation of IServiceProvider
        object IServiceProvider.GetService(Type serviceType) {
            if (serviceType == null) {
                throw new ArgumentNullException("serviceType");
            }

            if (serviceType == typeof(IComponentContainer)) {
                return _componentContainer;
            }

            return GetService(serviceType);
        }
        #endregion

        #region Implementation of IExternalNavigationService
        bool IExternalNavigationService.CanNavigate {
            get {
                return HtmlPage.IsEnabled;
            }
        }

        void IExternalNavigationService.Navigate(Uri uri, string targetFrame) {
            if (HtmlPage.IsEnabled) {
                HtmlPage.Window.Navigate(uri, targetFrame ?? "_self");
            }
        }
        #endregion

        #region IApplicationService Members
        void IApplicationService.StartService(ApplicationServiceContext context) {
            _startupArguments = context.ApplicationInitParams;

            Application.Current.UnhandledException += OnApplicationUnhandledException;
        }

        void IApplicationService.StopService() {
        }
        #endregion

        #region IApplicationLifetimeAware Members
        void IApplicationLifetimeAware.Exited() {
        }

        void IApplicationLifetimeAware.Exiting() {
            OnClosing();
        }

        void IApplicationLifetimeAware.Started() {
        }

        void IApplicationLifetimeAware.Starting() {
            _started = true;
            OnStarting();
        }
        #endregion
    }
}
