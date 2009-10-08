// Async.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.ComponentModel {

    /// <summary>
    /// Represents an asynchronous result and the associated task that
    /// produces the result.
    /// </summary>
    public abstract class Async : Model {

        private string _message;
        private object _result;
        private bool _completed;
        private bool _canceled;
        private Exception _error;
        private bool _errorHandled;

        private EventHandler _completedHandler;
        private bool _supportsCancel;
        private object _userData;

        internal Async(bool supportsCancel) {
            _supportsCancel = supportsCancel;
        }

        /// <summary>
        /// Gets whether the associated task can be canceled.
        /// </summary>
        public bool CanCancel {
            get {
                return (_completed == false) && _supportsCancel;
            }
        }

        /// <summary>
        /// Gets the error that occurred while completing the associated task.
        /// </summary>
        public Exception Error {
            get {
                return _error;
            }
        }

        /// <summary>
        /// Gets whether there was an error that occurred while completing the associated task.
        /// </summary>
        public bool HasError {
            get {
                return _error != null;
            }
        }

        /// <summary>
        /// Gets whether the associated task was canceled.
        /// </summary>
        public bool IsCanceled {
            get {
                return _canceled;
            }
        }

        /// <summary>
        /// Gets whether the associated task has been completed.
        /// </summary>
        public bool IsCompleted {
            get {
                return _completed;
            }
        }

        /// <summary>
        /// Gets whether the error if there was one has been handled.
        /// </summary>
        public bool IsErrorHandled {
            get {
                return _errorHandled;
            }
        }

        /// <summary>
        /// Gets or sets the message associated with this task.
        /// </summary>
        public string Message {
            get {
                return _message;
            }
            set {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        /// <summary>
        /// Gets the result of completing the associated task.
        /// </summary>
        public object Result {
            get {
                return _result;
            }
        }

        /// <summary>
        /// Gets or sets any data that the associated task initiator wants to track along with the task.
        /// </summary>
        public object UserData {
            get {
                return _userData;
            }
            set {
                _userData = value;
            }
        }

        /// <summary>
        /// The event raised when the associated task is completed, either with a result, with an error
        /// or because of cancelation.
        /// </summary>
        public event EventHandler Completed {
            add {
                _completedHandler = (EventHandler)Delegate.Combine(_completedHandler, value);
            }
            remove {
                _completedHandler = (EventHandler)Delegate.Remove(_completedHandler, value);
            }
        }

        /// <summary>
        /// Cancels the completion of the associated task.
        /// </summary>
        public void Cancel() {
            if (CanCancel == false) {
                throw new InvalidOperationException();
            }

            CancelCore();
            _canceled = true;
            _completed = true;

            if (_completedHandler != null) {
                _completedHandler(this, EventArgs.Empty);
            }

            RaisePropertyChanged("IsCanceled", "CanCancel", "IsCompleted");
        }

        internal abstract void CancelCore();

        /// <summary>
        /// Completes the associated task with the specified result.
        /// </summary>
        /// <param name="result">The result of completing the task.</param>
        protected void Complete(object result) {
            Complete(result, null);
        }

        /// <summary>
        /// Completes the associated task with the specified error.
        /// </summary>
        /// <param name="error">The error that occurred while completing the task.</param>
        public void Complete(Exception error) {
            Complete(GetDefaultResult(), error);
        }

        private void Complete(object result, Exception error) {
            if (_completed) {
                throw new InvalidOperationException();
            }

            _result = result;
            _error = error;
            _completed = true;

            if (error == null) {
                RaisePropertyChanged("Result", "IsCompleted", "CanCancel");
            }
            else {
                RaisePropertyChanged("Error", "HasError", "IsCompleted", "CanCancel");
            }

            if (_completedHandler != null) {
                _completedHandler(this, EventArgs.Empty);
            }

            if ((error != null) && (_errorHandled == false)) {
                throw error;
            }
        }

        internal abstract object GetDefaultResult();

        /// <summary>
        /// Marks the error as handled.
        /// </summary>
        public void MarkErrorAsHandled() {
            _errorHandled = true;
        }
    }

    /// <summary>
    /// Represents an asynchronous result and the associated task that
    /// produces the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class Async<TResult> : Async {

        private Action<Async<TResult>, object> _cancelAction;
        private object _taskState;

        /// <summary>
        /// Initializes an instance of an async result and task. The task is not cancelable.
        /// </summary>
        public Async()
            : base(/* supportsCancel */ false) {
        }

        /// <summary>
        /// Initializes an instance of an async result and task. The task is cancelable using the
        /// specified cancel Action.
        /// </summary>
        /// <param name="cancelAction">The action to perform when the task is canceled.</param>
        /// <param name="taskState">Optional contextual state associated with the task.</param>
        public Async(Action<Async<TResult>, object> cancelAction, object taskState)
            : base(/* supportsCancel */ true) {
            if (cancelAction == null) {
                throw new ArgumentNullException("cancelAction");
            }
            _cancelAction = cancelAction;
            _taskState = taskState;
        }

        /// <summary>
        /// Gets the result of completing the associated task.
        /// </summary>
        public new TResult Result {
            get {
                return (TResult)base.Result;
            }
        }

        internal override void CancelCore() {
            _cancelAction(this, _taskState);
        }

        /// <summary>
        /// Completes the associated task with the specified result.
        /// </summary>
        /// <param name="result">The result of completing the task.</param>
        public void Complete(TResult result) {
            base.Complete((object)result);
        }

        internal override object GetDefaultResult() {
            return default(TResult);
        }
    }
}
