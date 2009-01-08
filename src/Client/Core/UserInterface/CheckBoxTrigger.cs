// CheckBoxTrigger.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A trigger that can be associated with a CheckBox for handling
    /// Checked and Unchecked events.
    /// </summary>
    public sealed class CheckBoxTrigger : EventTriggerBase<CheckBox> {

        private bool _checked;

        /// <summary>
        /// Gets whether the associated action should be triggered when
        /// the Checkbox is checked or unchecked.
        /// </summary>
        public bool Checked {
            get {
                return _checked;
            }
            set {
                _checked = value;
            }
        }

        /// <internalonly />
        protected override string GetEventName() {
            return _checked ? "Checked" : "Unchecked";
        }
    }
}
