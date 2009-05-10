// RedirectActionResult.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.ComponentModel.Navigation {

    /// <summary>
    /// Represents a redirect as the result of invoking a controller action.
    /// </summary>
    public sealed class RedirectActionResult : ActionResult {

        private Uri _redirectUri;

        /// <summary>
        /// Initializes an instance of a RedirectActionResult with the specified URI.
        /// </summary>
        /// <param name="redirectUri">The URI to redirect to.</param>
        public RedirectActionResult(Uri redirectUri) {
            if (redirectUri == null) {
                throw new ArgumentNullException("redirectUri");
            }
            if (redirectUri.IsAbsoluteUri) {
                throw new ArgumentException("URI must be relative.");
            }

            _redirectUri = redirectUri;
        }

        /// <summary>
        /// Gets the URI to redirect to.
        /// </summary>
        public Uri RedirectUri {
            get {
                return _redirectUri;
            }
        }
    }
}
