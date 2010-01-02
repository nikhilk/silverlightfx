// ModelEventTrigger.cs
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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A trigger that can be associated with a specific event and uses
    /// the associated view model as the source for events.
    /// </summary>
    public sealed class ModelEventTrigger : EventTrigger {

        /// <internalonly />
        protected override object GetSource() {
            FrameworkElement fe = AssociatedObject as FrameworkElement;
            if (fe != null) {
                return ViewModelAttribute.GetCurrentViewModel(fe);
            }
            return null;
        }
    }
}
