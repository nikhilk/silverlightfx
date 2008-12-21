// Behavior.cs
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
using System.Windows;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Triggers are attachable objects that encapsulate some logic to
    /// determine when they are activated. They execute their associated action
    /// whenever they are activated.
    /// </summary>
    public abstract class Behavior : FrameworkElement, IAttachedObject {

        private BehaviorCollection _owner;
        private DependencyObject _associatedObject;

        internal Behavior() {
        }

        internal DependencyObject AssociatedObject {
            get {
                return _associatedObject;
            }
        }

        internal BehaviorCollection Owner {
            get {
                return _owner;
            }
            set {
                _owner = value;
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
    /// The base class for all behavior.
    /// </summary>
    /// <typeparam name="T">The type of object that this behavior can be associated with.</typeparam>
    public abstract class Behavior<T> : Behavior where T : DependencyObject {

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
