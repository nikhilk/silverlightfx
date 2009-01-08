// GoToState.cs
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

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that transitions from one visual state to another.
    /// </summary>
    public sealed class GoToState : TriggerAction<FrameworkElement> {

        /// <summary>
        /// Represents the State property.
        /// </summary>
        public static readonly DependencyProperty StateNameProperty =
            DependencyProperty.Register("StateName", typeof(string), typeof(GoToState), null);

        /// <summary>
        /// Represents the TargetName property.
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof(string), typeof(GoToState), null);

        /// <summary>
        /// Represents the UseTransition property.
        /// </summary>
        public static readonly DependencyProperty UseTransitionProperty =
            DependencyProperty.Register("UseTransition", typeof(bool), typeof(GoToState), null);

        /// <summary>
        /// Initializes an instance of a GoToState action.
        /// </summary>
        public GoToState() {
            UseTransition = true;
        }

        /// <summary>
        /// Gets or sets the name of the state to navigate to.
        /// </summary>
        public string StateName {
            get {
                return (string)GetValue(StateNameProperty);
            }
            set {
                SetValue(StateNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target control that should be navigated. By default this is
        /// the control that the action is associated with.
        /// </summary>
        public string TargetName {
            get {
                return (string)GetValue(TargetNameProperty);
            }
            set {
                SetValue(TargetNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the state navigation should be accompanied with a transition.
        /// </summary>
        public bool UseTransition {
            get {
                return (bool)GetValue(UseTransitionProperty);
            }
            set {
                SetValue(UseTransitionProperty, value);
            }
        }

        private Control GetTarget() {
            string targetName = TargetName;
            if (String.IsNullOrEmpty(targetName)) {
                return AssociatedObject as Control;
            }
            else {
                return AssociatedObject.FindName(targetName) as Control;
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            string stateName = StateName;
            if (String.IsNullOrEmpty(stateName)) {
                throw new InvalidOperationException("The StateName must be set on a GoToState action.");
            }

            Control target = GetTarget();
            if (target == null) {
                throw new InvalidOperationException("The target of the GoToState action was not found or was not a Control.");
            }

            bool hasState = VisualStateManager.GoToState(target, stateName, UseTransition);
            if (hasState == false) {
                throw new InvalidOperationException("The state named '" + stateName + "' does not exist.");
            }
        }
    }
}
