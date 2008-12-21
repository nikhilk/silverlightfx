// VerticalFlow.cs
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
    /// Indicates the flow children into a vertical layout panel.
    /// </summary>
    public enum VerticalFlow {

        /// <summary>
        /// Children flow from top to bottom.
        /// </summary>
        Top = 0,

        /// <summary>
        /// Children flow from center out toward top and bottom.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Children flow from bottom to top.
        /// </summary>
        Bottom = 2
    }
}
