// INavigationTarget.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Windows.Controls {

    /// <summary>
    /// Represents a control that provides navigation semantics. Typically implemented
    /// by Frame controls.
    /// </summary>
    public interface INavigationTarget {

        /// <summary>
        /// Gets the URI to which the control has been navigated to.
        /// </summary>
        Uri Uri {
            get;
        }

        /// <summary>
        /// Raised to indicate that the control has completed a navigation.
        /// </summary>
        event EventHandler<NavigatedEventArgs> Navigated;

        /// <summary>
        /// Raised to indicate that the control is performing a navigation.
        /// </summary>
        event EventHandler<NavigatingEventArgs> Navigating;

        /// <summary>
        /// Navigates the control to the specified URI.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        void Navigate(Uri uri);
    }
}
