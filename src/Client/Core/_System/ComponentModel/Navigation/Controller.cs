// Controller.cs
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
using System.Reflection;
using System.Threading;

namespace System.ComponentModel.Navigation {

    /// <summary>
    /// Encapsulates application logic exposed as a set of actions. The controller itself
    /// should be stateless. Each action is invoked using a different controller instance.
    /// </summary>
    public abstract class Controller : IController {

        private ActionInvocation _action;
        private ActionAsyncResult _asyncResult;

        /// <summary>
        /// Provides access to additional information about the action being executed.
        /// </summary>
        protected ActionInvocation CurrentAction {
            get {
                return _action;
            }
        }

        /// <summary>
        /// Creates an ErrorActionResult that specifies the error that occurred in
        /// completing the action.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        /// <returns>The ErrorActionResult containing error information.</returns>
        protected ErrorActionResult Error(Exception error) {
            return new ErrorActionResult(error);
        }

        private void OnActionCompleted(object sender, EventArgs e) {
            Async<ActionResult> asyncActionResult = (Async<ActionResult>)sender;
            if (asyncActionResult.HasError) {
                asyncActionResult.MarkErrorAsHandled();
                _asyncResult.Complete(new ErrorActionResult(asyncActionResult.Error));
            }
            else {
                _asyncResult.Complete(asyncActionResult.Result);
            }
        }

        /// <summary>
        /// Creates a RedirectActionResult that results in a redirect to the specified action.
        /// </summary>
        /// <param name="actionName">The name of the action on this controller to redirect to.</param>
        /// <returns>The RedirectActionResult containing information about the redirect to perform.</returns>
        protected RedirectActionResult Redirect(string actionName) {
            return Redirect(GetType(), actionName, null);
        }

        /// <summary>
        /// Creates a RedirectActionResult that results in a redirect to the specified action along
        /// with any specified parameters.
        /// </summary>
        /// <param name="actionName">The name of the action on this controller to redirect to.</param>
        /// <param name="parameters">The parameters to pass into the redirect.</param>
        /// <returns>The RedirectActionResult containing information about the redirect to perform.</returns>
        protected RedirectActionResult Redirect(string actionName, params string[] parameters) {
            return Redirect(GetType(), actionName, parameters);
        }

        /// <summary>
        /// Creates a RedirectActionResult that results in a redirect to the specified controller and
        /// action along with any specified parameters.
        /// </summary>
        /// <param name="controllerType">The controller to redirect to.</param>
        /// <param name="actionName">The name of the action on the specified controller to redirect to.</param>
        /// <param name="parameters">The parameters to pass into the redirect.</param>
        /// <returns>The RedirectActionResult containing information about the redirect to perform.</returns>
        protected RedirectActionResult Redirect(Type controllerType, string actionName, params string[] parameters) {
            return new RedirectActionResult(controllerType, actionName, parameters);
        }

        /// <summary>
        /// Creates a ViewActionResult that results in the specified view being created.
        /// </summary>
        /// <param name="viewName">The name of the view to create.</param>
        /// <returns>The ViewActionResult containing information about the view to create.</returns>
        protected ViewActionResult View(string viewName) {
            return new ViewActionResult(viewName);
        }

        #region Implementation of IController
        IAsyncResult IController.BeginExecute(ActionInvocation action, AsyncCallback callback, object asyncState) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            if (callback == null) {
                throw new ArgumentNullException("callback");
            }
            if (_asyncResult != null) {
                throw new InvalidOperationException();
            }

            _action = action;
            ActionDescriptor actionDescriptor = ControllerDescriptor.GetAction(this, action.ActionName);
            if (actionDescriptor == null) {
                throw new InvalidOperationException();
            }

            if (actionDescriptor.IsAsync) {
                _asyncResult = new ActionAsyncResult(this, callback, asyncState);

                try {
                    Async<ActionResult> asyncActionResult = (Async<ActionResult>)actionDescriptor.Invoke(this, action);
                    if (String.IsNullOrEmpty(asyncActionResult.Message)) {
                        asyncActionResult.Message = "Navigating";
                    }
                    asyncActionResult.Completed += OnActionCompleted;
                }
                catch (Exception e) {
                    _asyncResult = new ActionAsyncResult(new ErrorActionResult(e), asyncState);
                    callback(_asyncResult);
                }
            }
            else {
                ActionResult result;
                try {
                    result = (ActionResult)actionDescriptor.Invoke(this, action);
                }
                catch (Exception e) {
                    result = new ErrorActionResult(e);
                }

                _asyncResult = new ActionAsyncResult(result, asyncState);
                callback(_asyncResult);
            }

            return _asyncResult;
        }

        ActionResult IController.EndExecute(IAsyncResult asyncResult) {
            if (_asyncResult != asyncResult) {
                throw new ArgumentException();
            }

            return _asyncResult.Result;
        }
        #endregion


        private sealed class ActionAsyncResult : IAsyncResult {

            private Controller _controller;
            private ActionResult _result;
            private AsyncCallback _callback;
            private object _asyncState;

            public ActionAsyncResult(Controller controller, AsyncCallback callback, object asyncState) {
                _controller = controller;
                _callback = callback;
                _asyncState = asyncState;
            }

            public ActionAsyncResult(ActionResult result, object asyncState) {
                _result = result;
                _asyncState = asyncState;
            }

            public ActionResult Result {
                get {
                    return _result;
                }
            }

            public void Complete(ActionResult result) {
                _result = result;
                _callback(this);
            }

            #region Implementation of IAsyncResult
            object IAsyncResult.AsyncState {
                get {
                    return _asyncState;
                }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle {
                get {
                    throw new NotSupportedException();
                }
            }

            bool IAsyncResult.CompletedSynchronously {
                get {
                    return (_controller == null);
                }
            }

            bool IAsyncResult.IsCompleted {
                get {
                    return (_result != null);
                }
            }
            #endregion
        }
    }
}
