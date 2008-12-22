// TriggerAction.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
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

namespace System.Windows.Interactivity {

    /// <summary>
    /// TriggerActions are attachable objects that encapsulate some logic to
    /// perform whenever they are triggered.
    /// </summary>
    [TypeConverter(typeof(TriggerActionTypeConverter))]
    public abstract class TriggerAction : FrameworkElement, IAttachedObject {

        /// <summary>
        /// Represents the Condition property on an Action.
        /// </summary>
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register("Condition", typeof(bool), typeof(TriggerAction), null);

        private TriggerActionCollection _owner;
        private DependencyObject _associatedObject;
        private bool _queue;

        internal TriggerAction() {
            SetValue(ConditionProperty, true);
        }

        internal DependencyObject AssociatedObject {
            get {
                return _associatedObject;
            }
        }

        /// <summary>
        /// Gets or sets whether the action is enabled if it is to be conditionally
        /// enabled.
        /// </summary>
        public bool Condition {
            get {
                return (bool)GetValue(ConditionProperty);
            }
            set {
                SetValue(ConditionProperty, value);
            }
        }

        internal TriggerActionCollection Owner {
            get {
                return _owner;
            }
            set {
                _owner = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the action is executed by placing it into the message
        /// queue, or whether it executes immediately while the associated event is raised.
        /// </summary>
        public bool QueuedExecution {
            get {
                return _queue;
            }
            set {
                _queue = value;
            }
        }

        /// <summary>
        /// Allows the trigger to attach to its associated object.
        /// </summary>
        protected virtual void OnAttach() {
        }

        /// <summary>
        /// Allows the trigger to detach from its associated object.
        /// </summary>
        protected virtual void OnDetach() {
        }

        /// <summary>
        /// Allows an action to perform its functionlity. This is called
        /// by the framework whenever the associated event is raised by
        /// the associated component.
        /// </summary>
        /// <param name="e">The event data associated with the event.</param>
        protected abstract void InvokeAction(EventArgs e);

        internal void InvokeActionInternal(EventArgs e) {
            object dataContext = null;
            FrameworkElement associatedElement = AssociatedObject as FrameworkElement;
            if (associatedElement != null) {
                dataContext = associatedElement.DataContext;
            }

            DataContext = dataContext;
            if (Condition) {
                if (_queue) {
                    AssociatedObject.Dispatcher.BeginInvoke(delegate() {
                        InvokeAction(e);
                    });
                }
                else {
                    InvokeAction(e);
                }
            }
        }

        #region IAttachedObject Members
        DependencyObject IAttachedObject.AssociatedObject {
            get {
                return _associatedObject;
            }
        }

        void IAttachedObject.Attach(DependencyObject associatedObject) {
            _associatedObject = associatedObject;
            OnAttach();
        }

        void IAttachedObject.Detach() {
            OnDetach();
            _associatedObject = null;
        }
        #endregion
    }

    /// <summary>
    /// The base class for all TriggerAction implementations.
    /// </summary>
    /// <typeparam name="T">The type of object that this TriggerAction can be associated with.</typeparam>
    public abstract class TriggerAction<T> : TriggerAction where T : DependencyObject {

        /// <summary>
        /// The object associated with the trigger.
        /// </summary>
        protected new T AssociatedObject {
            get {
                return (T)base.AssociatedObject;
            }
        }
    }
}
