// EventTrigger.cs
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
using System.Windows;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A trigger that can be associated with a specific event.
    /// </summary>
    public sealed class EventTrigger : EventTriggerBase<DependencyObject> {

        private string _eventName;

        /// <summary>
        /// The name of the event to associated the trigger with.
        /// </summary>
        public string EventName {
            get {
                return _eventName;
            }
            set {
                _eventName = value;
            }
        }

        /// <internalonly />
        protected override string GetEventName() {
            if (String.IsNullOrEmpty(_eventName)) {
                throw new InvalidOperationException("The EventName property must be set on an EventTrigger.");
            }

            return _eventName;
        }
    }
}
