// EventTriggerBase.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A base class for triggers that can be associated with a specific event.
    /// </summary>
    public abstract class EventTriggerBase<T> : Trigger<T> where T : DependencyObject {

        private string _sourceName;
        private object _source;
        private Delegate _eventHandler;

        /// <summary>
        /// The name of the event to associated the trigger with.
        /// </summary>
        public string SourceName {
            get {
                return _sourceName;
            }
            set {
                _sourceName = value;
            }
        }

        /// <summary>
        /// Returns the name of the event to subscribe to.
        /// </summary>
        /// <returns>The event to handle.</returns>
        protected abstract string GetEventName();

        /// <summary>
        /// Gets the source of the event that this EventTrigger is subscribed to.
        /// </summary>
        /// <returns>The source of the event.</returns>
        protected object GetSource() {
            if (_source == null) {
                if (String.IsNullOrEmpty(_sourceName)) {
                    _source = AssociatedObject;
                }
                else {
                    FrameworkElement fe = AssociatedObject as FrameworkElement;
                    if (fe != null) {
                        if (String.Compare(_sourceName, "$model", StringComparison.Ordinal) == 0) {
                            return View.GetModel(fe);
                        }
                        if (String.Compare(_sourceName, "$dataContext", StringComparison.Ordinal) == 0) {
                            return fe.DataContext;
                        }
                        _source = fe.FindNameRecursive(_sourceName);
                        if (_source == null) {
                            _source = fe.FindResource(_sourceName);
                        }

                        if (_source == null) {
                            throw new InvalidOperationException("Could not resolve the specified SourceName, '" + _sourceName + "'");
                        }
                    }
                }
            }
            return _source;
        }

        /// <summary>
        /// Allows the trigger implementation to handle the event. The
        /// default implementation invokes the associated action.
        /// </summary>
        /// <param name="e">The data associated with the event.</param>
        protected virtual void HandleEvent(EventArgs e) {
            InvokeActions(e);
        }

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            object source = GetSource();
            string eventName = GetEventName();

            Type sourceType = source.GetType();
            EventInfo eventInfo = sourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance |
                                                                 BindingFlags.FlattenHierarchy);

            if (eventInfo == null) {
                throw new InvalidOperationException("The specified event '" + eventName + "' was not a valid event name.");
            }

            Type triggerType = typeof(EventTriggerBase<>).MakeGenericType(typeof(T));
            MethodInfo eventHandlerMethod = triggerType.GetMethod("OnEvent", BindingFlags.NonPublic | BindingFlags.Instance |
                                                                             BindingFlags.FlattenHierarchy);
            _eventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, eventHandlerMethod, /* throwOnBindFailure */ false);
            if (_eventHandler == null) {
                throw new InvalidOperationException("Could not subscribe to the event named '" + eventName + "'.");
            }

            eventInfo.AddEventHandler(source, _eventHandler);
        }

        /// <internalonly />
        protected override void OnDetach() {
            if (_eventHandler != null) {
                object source = GetSource();
                string eventName = GetEventName();

                Type sourceType = source.GetType();
                EventInfo eventInfo = sourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance |
                                                                     BindingFlags.FlattenHierarchy);

                eventInfo.RemoveEventHandler(source, _eventHandler);
                _eventHandler = null;
            }

            base.OnDetach();
        }

        private void OnEvent(object sender, EventArgs e) {
            HandleEvent(e);
        }
    }
}
