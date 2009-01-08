// BlindsMode.cs
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
    /// The mode that the blinds transition operates in.
    /// </summary>
    public enum BlindsMode {

        /// <summary>
        /// Indicates a blind transition that shows content underneath by lifting up the top content.
        /// </summary>
        Up = 0,

        /// <summary>
        /// Indicates a blind effect that hides content underneath by dropping down the top content.
        /// </summary>
        Down = 1
    }
}
