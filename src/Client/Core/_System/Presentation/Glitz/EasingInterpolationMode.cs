// EasingInterpolationMode.cs
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
    /// Defines how a TweenInterpolation performs its interpolation on the start and
    /// end of the progression.
    /// </summary>
    public enum EasingInterpolationMode {

        /// <summary>
        /// Indicates that the interpolation should perform easing on both the start
        /// and end of the animation.
        /// </summary>
        EaseInOut,

        /// <summary>
        /// Indicates that the interpolation should perform easing on the start
        /// of the animation.
        /// </summary>
        EaseIn,

        /// <summary>
        /// Indicates that the interpolation should perform easing on the end
        /// of the animation.
        /// </summary>
        EaseOut
    }
}
