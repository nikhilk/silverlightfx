// PageUriMapper.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.UserInterface.Navigation {

    /// <summary>
    /// Represents an object that can map a logical Page URI into an actual
    /// URI that can be used to load the page.
    /// </summary>
    public abstract class PageUriMapper {

        /// <summary>
        /// Maps the specified logical page URI into an actual URI.
        /// </summary>
        /// <param name="pageUri">The logical page URI that needs to be mapped.</param>
        /// <returns>The URI to use to load the page.</returns>
        public abstract Uri MapPageUri(Uri pageUri);
    }
}
