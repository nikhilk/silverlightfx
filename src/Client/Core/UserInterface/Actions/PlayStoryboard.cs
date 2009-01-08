// PlayStoryboard.cs
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
using System.Windows.Media.Animation;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that plays a storyboard.
    /// </summary>
    public class PlayStoryboard : StoryboardAction {

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            Storyboard storyboard = GetStoryboard();
            if (storyboard == null) {
                throw new InvalidOperationException("The specified storyboard '" + StoryboardName + "' for PlayStoryboard could not be found.");
            }

            storyboard.Begin();
        }
    }
}
