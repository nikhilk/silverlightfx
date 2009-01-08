// SlideMode.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.UserInterface.Transitions {

    /// <summary>
    /// The slide behavior to use.
    /// </summary>
    public enum SlideMode {

        /// <summary>
        /// Indicates a slide transition that shifts the content from left to right.
        /// </summary>
        Right = 0,

        /// <summary>
        /// Indicates a slide transition that shifts the content from right to left.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Indicates a slide transition that shifts the content from top to bottom.
        /// </summary>
        Down = 2,

        /// <summary>
        /// Indicates a slide transition that shifts the content from bottom to top.
        /// </summary>
        Up = 3
    }
}
