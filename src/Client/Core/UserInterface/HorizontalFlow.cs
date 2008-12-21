// HorizontalFlow.cs
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
    /// Indicates the flow children into a horizontal layout panel.
    /// </summary>
    public enum HorizontalFlow {

        /// <summary>
        /// Children flow from left to right.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Children flow from center out toward left and right.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Children flow from right to left.
        /// </summary>
        Right = 2
    }
}
