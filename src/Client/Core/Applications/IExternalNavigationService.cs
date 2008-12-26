// IExternalNavigationService.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.Applications {

    /// <summary>
    /// Provides the ability to perform a navigation external to the application.
    /// </summary>
    public interface IExternalNavigationService {

        /// <summary>
        /// Gets whether external navigation can be performed.
        /// </summary>
        bool CanNavigate {
            get;
        }

        /// <summary>
        /// Performs an external navigation to the specified URI in an optionally
        /// specified frame.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        /// <param name="targetFrame">The name of the frame to use.</param>
        void Navigate(Uri uri, string targetFrame);
    }
}
