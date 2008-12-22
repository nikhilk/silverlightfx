// StopStoryboard.cs
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
using System.Windows.Media.Animation;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that stop a storyboard.
    /// </summary>
    public class StopStoryboard : StoryboardAction {

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            Storyboard storyboard = GetStoryboard();
            if (storyboard == null) {
                throw new InvalidOperationException("The specified storyboard '" + StoryboardName + "' for StopStoryboard could not be found.");
            }

            storyboard.Stop();
        }
    }
}
