// NavigateAction.cs
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
using System.Windows.Browser;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that navigates to a selected URI.
    /// </summary>
    public class Navigate : TriggerAction<FrameworkElement> {

        /// <summary>
        /// Represents the NavigateUrl property.
        /// </summary>
        public static readonly DependencyProperty NavigateUrlProperty =
            DependencyProperty.Register("NavigateUrl", typeof(string), typeof(Navigate), null);

        /// <summary>
        /// Represents the Target property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(Navigate), null);

        /// <summary>
        /// Gets or sets the URL to navigate to.
        /// </summary>
        public string NavigateUrl {
            get {
                return (string)GetValue(NavigateUrlProperty);
            }
            set {
                SetValue(NavigateUrlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target window/frame that should be navigated.
        /// </summary>
        public string Target {
            get {
                return (string)GetValue(TargetProperty);
            }
            set {
                SetValue(TargetProperty, value);
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            if (HtmlPage.IsEnabled) {
                HtmlPage.Window.Navigate(new Uri(NavigateUrl, UriKind.RelativeOrAbsolute), Target ?? "_self");
            }
        }
    }
}
