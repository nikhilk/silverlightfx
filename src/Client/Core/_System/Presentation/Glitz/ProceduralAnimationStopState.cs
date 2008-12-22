// ProceduralAnimationStopState.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// Used to indicate the final state of an element being animated
    /// when the the animation is stopped mid-way.
    /// </summary>
    public enum ProceduralAnimationStopState {

        /// <summary>
        /// Leaves the animated element in its intended final state.
        /// </summary>
        Complete = 0,

        /// <summary>
        /// Leaves the element in its current state.
        /// </summary>
        Abort = 1,

        /// <summary>
        /// Leaves the element back in its original state.
        /// </summary>
        Revert = 2
    }
}
