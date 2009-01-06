// ElasticInterpolation.cs
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
    /// Represents an elastic easing interpolation that introduces spring-like start
    /// and finish animation.
    /// </summary>
    public sealed class ElasticInterpolation : EasingInterpolation {

        private int _oscillations;
        private double _springiness;

        /// <summary>
        /// Initializes an instance of a ElasticInterpolation.
        /// </summary>
        public ElasticInterpolation() {
            _oscillations = 3;
            _springiness = 3.0;
        }

        /// <summary>
        /// Gets or sets the number of spring oscillations.
        /// </summary>
        public int Oscillations {
            get {
                return _oscillations;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _oscillations = value;
            }
        }

        /// <summary>
        /// Gets or sets the relative height of the bounces.
        /// </summary>
        public double Springiness {
            get {
                return _springiness;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _springiness = value;
            }
        }

        /// <internalonly />
        protected override double InterpolateIn(double t) {
            return InterpolateElasticIn(t, _oscillations, _springiness);
        }

        /// <internalonly />
        protected override double InterpolateOut(double t) {
            return InterpolateElasticOut(t, _oscillations, _springiness);
        }
    }
}
