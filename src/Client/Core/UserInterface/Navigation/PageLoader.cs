// PageLoader.cs
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
using System.Reflection;
using System.Threading;
using System.Windows;

namespace SilverlightFX.UserInterface.Navigation {

    /// <summary>
    /// Represents an object that can load pages given a URI.
    /// </summary>
    public class PageLoader {

        internal static readonly PageLoader Default = new PageLoader();

        private PageFrame _frame;
        private Dictionary<Assembly, Dictionary<string, Type>> _pageRegistry;

        /// <summary>
        /// Gets the PageFrame owning this PageLoader.
        /// </summary>
        protected PageFrame Frame {
            get {
                return _frame;
            }
        }

        /// <summary>
        /// Begins loading a page for the specified URI.
        /// </summary>
        /// <param name="uri">The URI to be loaded.</param>
        /// <param name="uriContext">The current Page if one is available as context.</param>
        /// <param name="callback">The callback to be invoked once the page is available.</param>
        /// <param name="userState">Contextual information to be passed to the callback.</param>
        /// <returns>An async result representing the load operation.</returns>
        public IAsyncResult BeginLoadPage(Uri uri, Page uriContext, AsyncCallback callback, object userState) {
            if (uri == null) {
                throw new ArgumentNullException("uri");
            }
            if (callback == null) {
                throw new ArgumentNullException("callback");
            }

            return BeginLoadUri(uri, uriContext, callback, userState);
        }

        /// <summary>
        /// Begins loading the data for the specified URI. The default implementation
        /// interprets the URI as the type name of a page within the application.
        /// </summary>
        /// <param name="uri">The URI to be loaded.</param>
        /// <param name="uriContext">The current Page if one is available as context.</param>
        /// <param name="callback">The callback to be invoked once the data is available.</param>
        /// <param name="userState">Contextual information to be passed to the callback.</param>
        /// <returns>An async result representing the load operation.</returns>
        protected virtual IAsyncResult BeginLoadUri(Uri uri, Page uriContext, AsyncCallback callback, object userState) {
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(uri, uriContext, userState);

            _frame.Dispatcher.BeginInvoke(delegate() {
                asyncResult.Complete();
                callback(asyncResult);
            });

            return asyncResult;
        }

        /// <summary>
        /// Creates a page instance from the specified data. The default implementation
        /// tries to interpret the data as the page instance itself.
        /// </summary>
        /// <param name="data">The data to use to create the frame.</param>
        /// <returns>The result page.</returns>
        protected virtual Page CreatePage(object data) {
            Type pageType = data as Type;
            if ((pageType == null) || (typeof(Page).IsAssignableFrom(pageType) == false)) {
                throw new InvalidOperationException("The specified URL does not resolve to a Page.");
            }

            return (Page)Activator.CreateInstance(pageType);
        }

        /// <summary>
        /// Ends loading a page.
        /// </summary>
        /// <param name="asyncResult">The async result representing the load operation.</param>
        /// <param name="redirectUri">The URI to redirect to if a redirect must be performed.</param>
        /// <returns>The loaded page.</returns>
        public Page EndLoadPage(IAsyncResult asyncResult, out Uri redirectUri) {
            if (asyncResult == null) {
                throw new ArgumentNullException("asyncResult");
            }

            object data = EndLoadUri(asyncResult, out redirectUri);
            if (redirectUri == null) {
                return CreatePage(data);
            }
            return null;
        }

        /// <summary>
        /// Ends loading the data.
        /// </summary>
        /// <param name="asyncResult">The async result representing the load operation.</param>
        /// <param name="redirectUri">The URI to redirect to if a redirect must be performed.</param>
        /// <returns>The loaded data.</returns>
        protected virtual object EndLoadUri(IAsyncResult asyncResult, out Uri redirectUri) {
            SimpleAsyncResult simpleAsyncResult = asyncResult as SimpleAsyncResult;
            if (simpleAsyncResult == null) {
                throw new ArgumentException("Invalid asyncResult");
            }

            redirectUri = null;

            Type pageType = null;

            string typeName = simpleAsyncResult.Uri.ToString();

            int queryStringIndex = typeName.IndexOf('?');
            if (queryStringIndex >= 0) {
                typeName = typeName.Substring(queryStringIndex);
            }

            if (typeName.Length != 0) {
                Type referenceType = (simpleAsyncResult.UriContext ?? _frame.GetRootVisual()).GetType();
                pageType = GetPageType(typeName, referenceType);
            }

            return pageType;
        }

        private Type GetPageType(string typeName, Type referenceType) {
            if ((typeName.IndexOf('.') > 0) || (typeName.IndexOf(',') > 0)) {
                return TypeTypeConverter.ParseTypeName(Application.Current, typeName);
            }

            if (typeName.EndsWith("Page", StringComparison.OrdinalIgnoreCase)) {
                typeName = typeName.Substring(0, typeName.Length - 4);
            }

            Dictionary<string, Type> pageTypes = GetPageTypes(referenceType.Assembly);
            Type pageType = null;

            pageTypes.TryGetValue(typeName, out pageType);

            return pageType;
        }

        private Dictionary<string, Type> GetPageTypes(Assembly assembly) {
            if (_pageRegistry == null) {
                _pageRegistry = new Dictionary<Assembly, Dictionary<string, Type>>();
            }

            Dictionary<string, Type> pageTypes = null;
            if (_pageRegistry.TryGetValue(assembly, out pageTypes) == false) {
                pageTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

                Type[] allTypes = assembly.GetExportedTypes();
                Type pageType = typeof(Page);
                foreach (Type type in allTypes) {
                    if ((type.IsAbstract == false) &&
                        pageType.IsAssignableFrom(type)) {
                        string name = type.Name;
                        if (name.EndsWith("Page", StringComparison.OrdinalIgnoreCase)) {
                            name = name.Substring(0, name.Length - 4);
                        }

                        pageTypes[name] = type;
                    }
                }

                _pageRegistry[assembly] = pageTypes;
            }

            return pageTypes;
        }

        internal void Initialize(PageFrame ownerFrame) {
            _frame = ownerFrame;
        }


        private sealed class SimpleAsyncResult : IAsyncResult {

            private Uri _uri;
            private Page _uriContext;
            private object _asyncState;
            private bool _completed;

            public SimpleAsyncResult(Uri uri, Page uriContext, object asyncState) {
                _uri = uri;
                _uriContext = uriContext;
                _asyncState = asyncState;
            }

            public object AsyncState {
                get {
                    return _asyncState;
                }
            }

            public WaitHandle AsyncWaitHandle {
                get {
                    return null;
                }
            }

            public bool CompletedSynchronously {
                get {
                    return false;
                }
            }

            public bool IsCompleted {
                get {
                    return _completed;
                }
            }

            internal Uri Uri {
                get {
                    return _uri;
                }
            }

            internal Page UriContext {
                get {
                    return _uriContext;
                }
            }

            internal void Complete() {
                _completed = true;
            }
        }
    }
}
