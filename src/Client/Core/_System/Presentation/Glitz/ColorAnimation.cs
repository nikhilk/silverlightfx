// ColorAnimation.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Windows;
using System.Windows.Media;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// An animation that interpolates a Color-typed property from one value to another.
    /// </summary>
    public sealed class ColorAnimation : TweenAnimation {

        private DependencyObject _do;
        private DependencyProperty _dp;
        private Color _baseValue;
        private Color _targetValue;

        /// <summary>
        /// Initializes an instance of an ColorAnimation.
        /// </summary>
        /// <param name="o">The object to animate.</param>
        /// <param name="dp">The property on the object to animate.</param>
        /// <param name="duration">The time span over which the animation performs the interpolation.</param>
        /// <param name="targetValue">The value to interpolate to.</param>
        public ColorAnimation(DependencyObject o, DependencyProperty dp, TimeSpan duration, Color targetValue)
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
            _baseValue = (Color)_do.GetValue(_dp);
        }

        /// <internalonly />
        protected override void PerformTweening(double frame) {
            byte newA = (byte)(_baseValue.A + (_targetValue.A - _baseValue.A) * frame);
            byte newR = (byte)(_baseValue.R + (_targetValue.R - _baseValue.R) * frame);
            byte newG = (byte)(_baseValue.G + (_targetValue.G - _baseValue.G) * frame);
            byte newB = (byte)(_baseValue.B + (_targetValue.B - _baseValue.B) * frame);
            Color newValue = Color.FromArgb(newA, newR, newG, newB);

            _do.SetValue(_dp, newValue);
        }
    }
}
