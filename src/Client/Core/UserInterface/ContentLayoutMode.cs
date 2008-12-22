// ContentLayoutMode.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Indicates the mode of a panel within the template associated
    /// with a ContentLayout control.
    /// </summary>
    public enum ContentLayoutMode {

        /// <summary>
        /// Indicates that the content element matched to a panel
        /// should replace or override existing content in the panel.
        /// This is the default behavior.
        /// </summary>
        Replace = 0,

        /// <summary>
        /// Indicates that the content element matched to a panel
        /// should be merged along with existing content in the panel.
        /// </summary>
        Merge = 1
    }
}
