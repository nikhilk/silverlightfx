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

        private void OnActionCompleted(object sender, EventArgs e) {
            Task<ActionResult> task = (Task<ActionResult>)sender;
            if (task.HasError) {
                _asyncResult.Complete(new ErrorActionResult(task.Error));
            }
            else {
                _asyncResult.Complete(task.Result);
            }
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
                    Task<ActionResult> task = (Task<ActionResult>)actionDescriptor.Invoke(this, action);
                    task.Completed += OnActionCompleted;
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
