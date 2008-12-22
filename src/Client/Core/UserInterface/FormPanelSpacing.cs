// FormPanelSpacing.cs
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
    /// Represents the spacing style between groups of labels and associated
    /// elements.
    /// </summary>
    public enum FormPanelSpacing {

        /// <summary>
        /// The single unit of spacing is added between groups.
        /// </summary>
        Normal,

        /// <summary>
        /// The default spacing between groups is ignored.
        /// </summary>
        Ignore,

        /// <summary>
        /// An extra unit of spacing is added between groups.
        /// </summary>
        Extra
    }
}
