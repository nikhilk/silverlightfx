// Navigate.cs
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
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SilverlightFX.UserInterface.Actions {

    /// <summary>
    /// An action that navigates to a selected URI.
    /// </summary>
    public class Navigate : TriggerAction<FrameworkElement> {

        /// <summary>
        /// Represents the NavigateBrowser property.
        /// </summary>
        public static readonly DependencyProperty NavigateBrowserProperty =
            DependencyProperty.Register("NavigateBrowser", typeof(bool), typeof(Navigate),
                                        new PropertyMetadata(false));

        /// <summary>
        /// Represents the Target property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(Navigate), null);

        /// <summary>
        /// Represents the Uri property.
        /// </summary>
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(Uri), typeof(Navigate), null);

        /// <summary>
        /// Gets or sets the whether the navigation should be internal to the application
        /// or external, i.e. at the level of HTML page containing the application.
        /// </summary>
        public bool NavigateBrowser {
            get {
                return (bool)GetValue(NavigateBrowserProperty);
            }
            set {
                SetValue(NavigateBrowserProperty, value);
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

        /// <summary>
        /// Gets or sets the URL to navigate to.
        /// </summary>
        public Uri Uri {
            get {
                return (Uri)GetValue(UriProperty);
            }
            set {
                SetValue(UriProperty, value);
            }
        }

        /// <internalonly />
        protected override void InvokeAction(EventArgs e) {
            Uri navigateUri = Uri;
            if (navigateUri == null) {
                return;
            }

            string targetFrame = Target;

            if ((NavigateBrowser == false) && (navigateUri.IsAbsoluteUri == false)) {
                INavigationTarget target = null;

                if (String.IsNullOrEmpty(targetFrame) == false) {
                    target = AssociatedObject.FindNameRecursive(targetFrame) as INavigationTarget;
                }
                else {
                    target = (INavigationTarget)AssociatedObject.FindRecursive(typeof(INavigationTarget));
                }

                if (target != null) {
                    target.Navigate(navigateUri);
                }
                else {
                    throw new InvalidOperationException("Did not find a target to navigate.");
                }
            }
            else if (HtmlPage.IsEnabled) {
                HtmlPage.Window.Navigate(navigateUri, targetFrame ?? "_self");
            }
        }
    }
}
