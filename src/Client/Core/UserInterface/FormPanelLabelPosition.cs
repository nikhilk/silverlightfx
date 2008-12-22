// FormPanelLabelPosition.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// You are free to:
// - use this framework as part of your app
// - make use of the framework in a commercial app
// as long as your app or product is itself not a framework, control pack or
// developer toolkit of any sort under the following conditions:
// Attribution. You must attribute the original work in your
//              product or release.
// Share Alike. If you alter, transform, or build as-is upon this work,
//              you may only distribute the resulting app source under
//              a license identical to this one.
//

using System;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents the position of a Label element with respect to its
    /// associated element within a FormPanel.
    /// </summary>
    public enum FormPanelLabelPosition {

        /// <summary>
        /// The Label is placed to the left of its associated control.
        /// </summary>
        Left,

        /// <summary>
        /// The Label is placed on the top of its associated control.
        /// </summary>
        Top
    }
}
