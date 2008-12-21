// Trigger.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Triggers are attachable objects that encapsulate some logic to
    /// determine when they are activated. They execute their associated action
    /// whenever they are activated.
    /// </summary>
    [ContentProperty("Actions")]
    public abstract class Trigger : FrameworkElement, IAttachedObject {

        private TriggerCollection _owner;

        private DependencyObject _associatedObject;
        private TriggerAction _action;
        private TriggerActionCollection _actions;

        internal Trigger() {
            _actions = new TriggerActionCollection();
        }

        internal DependencyObject AssociatedObject {
            get {
                return _associatedObject;
            }
        }

        /// <summary>
        /// Gets or sets the default Action associated with this Trigger.
        /// </summary>
        public TriggerAction Action {
            get {
                return _action;
            }
            set {
                if (_action != null) {
                    ((IAttachedObject)_action).Detach();
                }
                _action = value;
                if (((IAttachedObject)_action).AssociatedObject != _associatedObject) {
                    ((IAttachedObject)_action).Detach();

                    if (_associatedObject != null) {
                        ((IAttachedObject)_action).Attach(_associatedObject);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Actions to be invoked when the trigger is activated.
        /// </summary>
        public TriggerActionCollection Actions {
            get {
                return _actions;
            }
        }

        internal TriggerCollection Owner {
            get {
                return _owner;
            }
            set {
                _owner = value;
            }
        }

        /// <summary>
        /// Invokes the associated actions.
        /// </summary>
        /// <param name="e">The event data associated with the invocation.</param>
        protected void InvokeActions(EventArgs e) {
            if (_action != null) {
                _action.InvokeActionInternal(e);
            }
            foreach (TriggerAction action in _actions) {
                action.InvokeActionInternal(e);
            }
        }

        /// <summary>
        /// Allows the trigger to attach to its associated object.
        /// </summary>
        protected virtual void OnAttach() {
            if (_action != null) {
                ((IAttachedObject)_action).Attach(_associatedObject);
            }
            _actions.AssociatedObject = _associatedObject;
        }

        /// <summary>
        /// Allows the trigger to detach from its associated object.
        /// </summary>
        protected virtual void OnDetach() {
            if (_action != null) {
                ((IAttachedObject)_action).Detach();
            }
            _actions.AssociatedObject = null;
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
    /// The base class for all triggers.
    /// </summary>
    /// <typeparam name="T">The type of object that this trigger can be associated with.</typeparam>
    public abstract class Trigger<T> : Trigger where T : DependencyObject {

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
