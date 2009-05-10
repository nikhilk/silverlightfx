// MvcPageLoader.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Navigation;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace SilverlightFX.UserInterface.Navigation {

    // TODO: We need a better way to find all controllers and corresponding views
    //       in the application
    // TODO: We need a model for on-demand loading of controllers and views

    /// <summary>
    /// A PageLoader that loads pages using the MVC pattern. The PageLoader finds
    /// a Controller matching the URI, invokes an Action on it, and then converts the
    /// ActionResult to a Page using a set of views.
    /// </summary>
    public class MvcPageLoader : PageLoader {

        private Type _controllerType;

        private Controller _currentController;
        private string _currentControllerName;
        private string _currentActionName;

        /// <summary>
        /// Gets or sets the type of the singleton Controller in the application to
        /// use for all URIs. This helps simplify URIs for simple applications that do
        /// not have multiple controllers.
        /// </summary>
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type ControllerType {
            get {
                return _controllerType;
            }
            set {
                if ((value != null) && (typeof(Controller).IsAssignableFrom(value) == false)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _controllerType = value;
            }
        }

        /// <internalonly />
        protected override IAsyncResult BeginLoadUri(Uri uri, Page uriContext, AsyncCallback callback, object asyncState) {
            if (_currentController != null) {
                throw new InvalidOperationException();
            }

            UriData uriData = new UriData(uri);

            IList<string> path = uriData.GetPath();
            int minimumCount = _controllerType != null ? 1 : 2;

            if ((path != null) && (path.Count >= minimumCount)) {
                Controller controller = null;

                Type controllerType = _controllerType;
                if (controllerType == null) {
                    Type appType = Application.Current.GetType();

                    _currentControllerName = path[0];
                    path.RemoveAt(0);

                    string controllerTypeName =
                        appType.Namespace + ".Controllers." + _currentControllerName + "Controller";

                    controllerType = appType.Assembly.GetType(controllerTypeName, /* throwOnError */ false);
                    if (typeof(Controller).IsAssignableFrom(controllerType) == false) {
                        controllerType = null;
                    }
                }
                if (controllerType != null) {
                    controller = CreateController(controllerType);
                }

                if (controller != null) {
                    _currentController = controller;

                    _currentActionName = path[0];
                    path.RemoveAt(0);

                    ActionInvocation action = new ActionInvocation(_currentActionName);
                    foreach (string parameterItem in path) {
                        action.Parameters.Add(parameterItem);
                    }

                    IDictionary<string, string> queryString = uriData.GetQueryString();
                    if (queryString != null) {
                        foreach (KeyValuePair<string, string> parameterItem in queryString) {
                            action.NamedParameters[parameterItem.Key] = parameterItem.Value;
                        }
                    }

                    return ((IController)controller).BeginExecute(action, callback, asyncState);
                }
            }

            return base.BeginLoadUri(uri, uriContext, callback, asyncState);
        }

        private Controller CreateController(Type controllerType) {
            IComponentContainer container = null;

            IServiceProvider sp = Application.Current as IServiceProvider;
            if (sp != null) {
                container = (IComponentContainer)sp.GetService(typeof(IComponentContainer));
            }

            if (container != null) {
                return (Controller)container.GetObject(controllerType);
            }

            return (Controller)Activator.CreateInstance(controllerType);
        }

        /// <internalonly />
        protected override Page CreatePage(object data) {
            ActionResult actionResult = data as ActionResult;

            if (actionResult != null) {
                Page page = null;

                string viewName = null;
                object viewModel = null;

                ViewActionResult viewResult = actionResult as ViewActionResult;
                if (viewResult != null) {
                    viewName = viewResult.ViewName;
                    viewModel = viewResult.ViewModel;
                }

                ErrorActionResult errorResult = actionResult as ErrorActionResult;
                if (errorResult != null) {
                    viewName = "Error";
                }

                if (String.IsNullOrEmpty(viewName) == false) {
                    Type appType = Application.Current.GetType();
                    string viewTypeName =
                        appType.Namespace +
                        ".Views." + _currentControllerName + "." + viewResult.ViewName + "Page";

                    Type viewType = appType.Assembly.GetType(viewTypeName, /* throwOnError */ false);

                    if ((viewType == null) || (typeof(Page).IsAssignableFrom(viewType) == false)) {
                        viewTypeName = appType.Namespace + ".Views.Shared." + viewResult.ViewName + "Page";
                        viewType = appType.Assembly.GetType(viewTypeName, /* throwOnError */ false);
                    }

                    if ((viewType != null) && typeof(Page).IsAssignableFrom(viewType)) {
                        page = (Page)Activator.CreateInstance(viewType);
                        if (viewResult.ViewModel != null) {
                            page.Model = viewResult.ViewModel;
                        }

                        if ((page is ErrorPage) && (errorResult != null)) {
                            ((ErrorPage)page).Error = errorResult.Error;
                        }
                    }
                }

                if ((page == null) && (errorResult != null)) {
                    throw errorResult.Error;
                }

                return page;
            }

            return base.CreatePage(data);
        }

        /// <internalonly />
        protected override object EndLoadUri(IAsyncResult asyncResult, out Uri redirectUri) {
            redirectUri = null;

            if (_currentController != null) {
                ActionResult actionResult = null;
                try {
                    actionResult = ((IController)_currentController).EndExecute(asyncResult);

                    RedirectActionResult redirectResult = actionResult as RedirectActionResult;
                    if (redirectResult != null) {
                        redirectUri = redirectResult.RedirectUri;
                    }
                }
                finally {
                    _currentController = null;
                }
                return actionResult;
            }

            return base.EndLoadPage(asyncResult, out redirectUri);
        }
    }
}
