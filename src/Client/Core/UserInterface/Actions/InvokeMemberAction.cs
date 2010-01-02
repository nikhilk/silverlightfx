// InvokeAction.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
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

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that invokes a member on the associated DataContext or a specified
    /// target object.
    /// </summary>
    public abstract class InvokeMemberAction : TriggerAction<FrameworkElement> {

        private string _targetName;

        /// <summary>
        /// Gets or sets the name of the target to invoke the method on when this action
        /// is triggered. You can use the special $self special value to indicate the
        /// associated object itself. Leaving this unspecified defaults the target to
        /// be the value set as the DataContext of the associated object.
        /// </summary>
        public string TargetName {
            get {
                return _targetName;
            }
            set {
                _targetName = value;
            }
        }

        /// <summary>
        /// Gets the target of the InvokeMemberAction.
        /// </summary>
        /// <returns>The target object; null if no object could be found.</returns>
        protected object GetTarget() {
            object target = null;
            if (String.IsNullOrEmpty(_targetName) || (String.CompareOrdinal(_targetName, "$dataContext") == 0)) {
                target = AssociatedObject.DataContext;
            }
            else if (String.CompareOrdinal(_targetName, "$element") == 0) {
                target = AssociatedObject;
            }
            else if (String.CompareOrdinal(_targetName, "$model") == 0) {
                target = ViewModelAttribute.GetCurrentViewModel(AssociatedObject);
            }
            else {
                target = AssociatedObject.FindNameRecursive(_targetName);
                if (target == null) {
                    target = AssociatedObject.FindResource(_targetName);
                }
            }

            return target;
        }
    }
}
