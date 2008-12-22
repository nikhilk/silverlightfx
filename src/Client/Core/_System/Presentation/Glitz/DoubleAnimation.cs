// DoubleAnimation.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// An animation that interpolates a double-typed property from one value to another.
    /// </summary>
    public sealed class DoubleAnimation : TweenAnimation {

        private DependencyObject _do;
        private DependencyProperty _dp;
        private double _baseValue;
        private double _targetValue;

        /// <summary>
        /// Initializes an instance of an DoubleAnimation.
        /// </summary>
        /// <param name="o">The object to animate.</param>
        /// <param name="dp">The property on the object to animate.</param>
        /// <param name="duration">The time span over which the animation performs the interpolation.</param>
        /// <param name="targetValue">The value to interpolate to.</param>
        public DoubleAnimation(DependencyObject o, DependencyProperty dp, TimeSpan duration, double targetValue)
            : base(duration) {
            if (o == null) {
                throw new ArgumentNullException("o");
            }
            if (dp == null) {
                throw new ArgumentNullException("dp");
            }

            _do = o;
            _dp = dp;
            _targetValue = targetValue;
            _baseValue = (double)_do.GetValue(_dp);
        }

        /// <internalonly />
        protected override void PerformTweening(double frame) {
            double newValue = _baseValue + ((_targetValue - _baseValue) * frame);
            _do.SetValue(_dp, newValue);
        }
    }
}
