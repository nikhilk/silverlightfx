// NavigateAction.cs
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

    // TODO: Switch default of External and rename to Browser?

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

        private bool _externalNavigation;

        /// <summary>
        /// Initializes an instance of Navigate.
        /// </summary>
        public Navigate() {
            _externalNavigation = true;
        }

        /// <summary>
        /// Gets or sets the whether the navigation should be internal to the application
        /// or external, i.e. at the level of HTML page containing the application.
        /// </summary>
        public bool ExternalNavigation {
            get {
                return _externalNavigation;
            }
            set {
                _externalNavigation = value;
            }
        }

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
            string navigateUrl = NavigateUrl;
            if (String.IsNullOrEmpty(navigateUrl)) {
                return;
            }

            string targetFrame = Target;
            Uri navigateUri = new Uri(navigateUrl, UriKind.RelativeOrAbsolute);

            if (_externalNavigation == false) {
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
