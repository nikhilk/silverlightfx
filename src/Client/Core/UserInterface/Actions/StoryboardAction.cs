// StoryboardAction.cs
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
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that interacts with a storyboard.
    /// </summary>
    public abstract class StoryboardAction : TriggerAction<FrameworkElement> {

        private string _storyboardName;

        /// <summary>
        /// Gets or sets the name of the storyboard to play. This is used as the
        /// key to lookup a storyboard in the associated object's resources.
        /// </summary>
        public string StoryboardName {
            get {
                return _storyboardName;
            }
            set {
                _storyboardName = value;
            }
        }

        /// <summary>
        /// Gets the storyboard instance associated with this action.
        /// </summary>
        /// <returns>The selected storyboard instance</returns>
        protected Storyboard GetStoryboard() {
            if (String.IsNullOrEmpty(_storyboardName) == false) {
                return AssociatedObject.FindResource(_storyboardName) as Storyboard;
            }
            return null;
        }
    }
}
