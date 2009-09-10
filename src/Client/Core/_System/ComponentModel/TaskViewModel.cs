// TaskViewModel.cs
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
    /// Provides a base class for implementing models that encapsulate
    /// data and behavior associated with a task that can be canceled or
    /// committed. While this is independent of presentation, this model
    /// is typically associated with a Form.
    /// </summary>
    public abstract class TaskViewModel : ViewModel {

        private bool _committed;
        private bool _completed;
        private EventHandler _completedHandler;
        private Action _completeCallback;

        /// <summary>
        /// Gets whether the associated task was committed or canceled.
        /// </summary>
        public bool IsCommitted {
            get {
                return _committed;
            }
        }

        /// <summary>
        /// Raised when the task associated with this model has been completed.
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
        /// Cancels the task associated with this model.
        /// </summary>
        /// <param name="completeCallback">A callback to invoke when the task is canceled.</param>
        public void Cancel(Action completeCallback) {
            _completeCallback = completeCallback;
            Cancel();
        }

        /// <summary>
        /// Allows the model to perform any logic it needs to execute when canceling the
        /// task. Derived implementations should call the base implementation or the Complete
        /// method (if the task cancels asynchronously).
        /// </summary>
        protected virtual void Cancel() {
            Complete(/* commit */ false);
        }

        /// <summary>
        /// Commits the task associated with this model.
        /// </summary>
        /// <param name="completeCallback">A callback to invoke when the task is committed.</param>
        public void Commit(Action completeCallback) {
            _completeCallback = completeCallback;
            Commit();
        }

        /// <summary>
        /// Allows the model to perform any logic it needs to execute when committing the
        /// task. Derived implementations should call the base implementation or the Complete
        /// method (if the task commits asynchronously).
        /// </summary>
        protected virtual void Commit() {
            Complete(/* commit */ true);
        }

        /// <summary>
        /// Marks the task as completed.
        /// </summary>
        /// <param name="commit">Whether the task was committed or canceled.</param>
        protected void Complete(bool commit) {
            if (_completed) {
                throw new InvalidOperationException("Complete can only be called once.");
            }

            _completed = true;
            _committed = commit;

            if (_completedHandler != null) {
                _completedHandler(this, EventArgs.Empty);
            }
            if (_completeCallback != null) {
                _completeCallback();
            }
        }
    }
}
