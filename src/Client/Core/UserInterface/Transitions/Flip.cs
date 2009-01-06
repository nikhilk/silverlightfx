// Flip.cs
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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Transitions {

    /// <summary>
    /// Represents a flip effect that can be attached to a container
    /// with two child elements. The effect flips one element to another.
    /// </summary>
    public class Flip : Transition {

        private ScaleTransform _scaleTransform;

        /// <internalonly />
        protected override ProceduralAnimation CreateTransitionAnimation(Panel container, EffectDirection direction) {
            if (_scaleTransform == null) {
                _scaleTransform = new ScaleTransform();
                container.RenderTransform = _scaleTransform;

                container.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            TweenInterpolation interpolation = GetEffectiveInterpolation();
            TimeSpan shortDuration = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 3);

            FlipScaleAnimation scaleAnimation =
                new FlipScaleAnimation(Duration, _scaleTransform,
                                       (direction == EffectDirection.Forward ? 180 : -180));
            scaleAnimation.Interpolation = interpolation;

            DoubleAnimation frontAnimation =
                new DoubleAnimation(container.Children[1], UIElement.OpacityProperty, shortDuration,
                                    (direction == EffectDirection.Forward ? 0 : 1));
            frontAnimation.Interpolation = interpolation;
            frontAnimation.StartDelay = shortDuration;

            DoubleAnimation backAnimation =
                new DoubleAnimation(container.Children[0], UIElement.OpacityProperty, shortDuration,
                                    (direction == EffectDirection.Forward ? 1 : 0));
            backAnimation.Interpolation = interpolation;
            backAnimation.StartDelay = shortDuration;

            return new ProceduralAnimationSet(scaleAnimation, frontAnimation, backAnimation);
        }

        /// <internalonly />
        protected override void OnDetach() {
            _scaleTransform = null;

            base.OnDetach();
        }

        private sealed class FlipScaleAnimation : TweenAnimation {

            private double _angle;
            private ScaleTransform _scaleTransform;

            public FlipScaleAnimation(TimeSpan duration, ScaleTransform scaleTransform, double angle)
                : base(duration) {
                _angle = angle;
                _scaleTransform = scaleTransform;
            }

            protected override void PerformTweening(double frame) {
                double flipAngle = frame * _angle;
                double cosine = Math.Cos(flipAngle * Math.PI / 180);
                _scaleTransform.ScaleX = cosine * cosine;
            }
        }
    }
}
