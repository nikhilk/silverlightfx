// LinearInterpolation.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Glitz {

    /// <summary>
    /// Represents a linear or no-op interpolation
    /// </summary>
    public sealed class LinearInterpolation : TweenInterpolation {

        /// <summary>
        /// Initializes an instance of a LinearInterpolation.
        /// </summary>
        public LinearInterpolation()
            : base(/* isLinear */ true) {
        }

        /// <internalonly />
        public override double Interpolate(double t) {
            return t;
        }
    }
}
