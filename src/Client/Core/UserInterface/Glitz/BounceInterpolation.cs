// BounceInterpolation.cs
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
    /// Represents a bounce easing interpolation that starts the animation bouncing
    /// or ends the animation bouncing.
    /// </summary>
    public sealed class BounceInterpolation : EasingInterpolation {

        private int _bounces;
        private double _bounciness;

        /// <summary>
        /// Initializes an instance of a BounceInterpolation.
        /// </summary>
        public BounceInterpolation() {
            _bounces = 3;
            _bounciness = 3.0;
        }

        /// <summary>
        /// Gets or sets the number of bounces.
        /// </summary>
        public int Bounces {
            get {
                return _bounces;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _bounces = value;
            }
        }

        /// <summary>
        /// Gets or sets the relative height of the bounces.
        /// </summary>
        public double Bounciness {
            get {
                return _bounciness;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _bounciness = value;
            }
        }

        /// <internalonly />
        protected override double InterpolateIn(double t) {
            return InterpolateBounceIn(t, _bounces, _bounciness);
        }

        /// <internalonly />
        protected override double InterpolateOut(double t) {
            return InterpolateBounceOut(t, _bounces, _bounciness);
        }
    }
}
