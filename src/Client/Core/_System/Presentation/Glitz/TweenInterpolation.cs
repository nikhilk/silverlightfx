// TweenInterpolation.cs
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
    /// Provides the functionality for interpolating an animation from start to finish.
    /// </summary>
    public abstract class TweenInterpolation {

        private bool _isLinear;

        /// <summary>
        /// Initializes an instance of TweenInterpolation.
        /// </summary>
        protected TweenInterpolation()
            : this(/* isLinear */ false) {
        }

        /// <summary>
        /// Initializes an instance of TweenInterpolation.
        /// </summary>
        /// <param name="isLinear">Whether this instance represents the linear interpolation.</param>
        protected TweenInterpolation(bool isLinear) {
            _isLinear = isLinear;
        }

        /// <summary>
        /// Gets whether this interpolation represents the no-op linear interpolation.
        /// </summary>
        public bool IsLinearInterpolation {
            get {
                return _isLinear;
            }
        }

        /// <summary>
        /// Implements a interpolation function that progresses an animation.
        /// This represents a function f(t) where t = [0...1] such that
        /// it is continuous and f(0) == 0 and f(1) == 1.
        /// Interpolation can be used to modify the default linear progression.
        /// </summary>
        /// <param name="t">The current progress value based on linear interpolation.</param>
        /// <returns>The modified value to use as the new progress value.</returns>
        public abstract double Interpolate(double t);
    }
}
