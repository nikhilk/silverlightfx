// Task.cs
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
    /// Represents asynchronous work and its result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class Task<TResult> : Model {

        private TResult _result;
        private bool _completed;
        private bool _canceled;
        private Exception _error;

        private EventHandler _completedHandler;
        private Action<Task<TResult>, object> _cancelAction;
        private object _taskState;

        private object _userData;

        /// <summary>
        /// Initializes an instance of an async Task. The task is not cancelable.
        /// </summary>
        public Task() {
        }

        /// <summary>
        /// Initializes an instance of an async Task. The task is cancelable using the
        /// specified cancel Action.
        /// </summary>
        /// <param name="cancelAction">The action to perform when the task is canceled.</param>
        /// <param name="taskState">Optional contextual state associated with the task.</param>
        public Task(Action<Task<TResult>, object> cancelAction, object taskState) {
            if (cancelAction == null) {
                throw new ArgumentNullException("cancelAction");
            }
            _cancelAction = cancelAction;
            _taskState = taskState;
        }

        /// <summary>
        /// Gets whether the task can be canceled.
        /// </summary>
        public bool CanCancel {
            get {
                return (_completed == false) && (_cancelAction != null);
            }
        }

        /// <summary>
        /// Gets the error that occurred while completing the task.
        /// </summary>
        public Exception Error {
            get {
                return _error;
            }
        }

        /// <summary>
        /// Gets whether there was an error that occurred while completing the task.
        /// </summary>
        public bool HasError {
            get {
                return _error != null;
            }
        }

        /// <summary>
        /// Gets whether the task was canceled.
        /// </summary>
        public bool IsCanceled {
            get {
                return _canceled;
            }
        }

        /// <summary>
        /// Gets whether the task has been completed.
        /// </summary>
        public bool IsCompleted {
            get {
                return _completed;
            }
        }

        /// <summary>
        /// Gets the result of completing the task.
        /// </summary>
        public TResult Result {
            get {
                return _result;
            }
        }

        /// <summary>
        /// Gets or sets any data that the task caller wants to track along with the task.
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
        /// The event raised when the task is completed, either with a result, with an error
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
        /// Cancels the completion of the task.
        /// </summary>
        public void Cancel() {
            if (CanCancel == false) {
                throw new InvalidOperationException();
            }

            _cancelAction(this, _taskState);
            _canceled = true;
            _completed = true;

            if (_completedHandler != null) {
                _completedHandler(this, EventArgs.Empty);
            }

            RaisePropertyChanged("IsCanceled", "CanCancel", "IsCompleted");
        }

        /// <summary>
        /// Completes the task with the specified result.
        /// </summary>
        /// <param name="result">The result of completing the task.</param>
        public void Complete(TResult result) {
            Complete(result, null);
        }

        /// <summary>
        /// Completes the task with the specified error.
        /// </summary>
        /// <param name="error">The error that occurred while completing the task.</param>
        public void Complete(Exception error) {
            Complete(default(TResult), error);
        }

        private void Complete(TResult result, Exception error) {
            if (_completed) {
                throw new InvalidOperationException();
            }

            _result = result;
            _error = error;
            _completed = true;

            if (_completedHandler != null) {
                _completedHandler(this, EventArgs.Empty);
            }

            if (error == null) {
                RaisePropertyChanged("Result", "IsCompleted", "CanCancel");
            }
            else {
                RaisePropertyChanged("Error", "HasError", "IsCompleted", "CanCancel");
            }
        }
    }
}
