// Shake.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents an effect that shakes the associated element left and right.
    /// </summary>
    public class Shake : Effect {

        private double _shakeDistance;
        private TranslateTransform _translateTransform;

        /// <summary>
        /// Initializes an instance of a ShakeEffect.
        /// </summary>
        public Shake() {
            _shakeDistance = 10;
        }

        /// <summary>
        /// Gets or sets the distance that the element should be shaken.
        /// </summary>
        public double ShakeDistance {
            get {
                return _shakeDistance;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _shakeDistance = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            FrameworkElement target = GetTarget();

            if (_translateTransform == null) {
                Transform existingTransform = target.RenderTransform;
                if (existingTransform != null) {
                    _translateTransform = existingTransform as TranslateTransform;

                    if (_translateTransform == null) {
                        TransformGroup transformGroup = existingTransform as TransformGroup;

                        if (transformGroup != null) {
                            foreach (Transform transform in transformGroup.Children) {
                                _translateTransform = transform as TranslateTransform;
                                if (_translateTransform != null) {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (_translateTransform == null) {
                throw new InvalidOperationException("The element does not have a translate transform associated with it.");
            }

            ShakeAnimation animation = new ShakeAnimation(_translateTransform, Duration, _shakeDistance);
            animation.Interpolation = GetEffectiveInterpolation();

            return animation;
        }


        private sealed class ShakeAnimation : TweenAnimation {

            private TranslateTransform _transform;
            private double _distance;

            public ShakeAnimation(TranslateTransform transform, TimeSpan duration, double distance)
                : base(duration) {
                _transform = transform;
                _distance = distance;
            }

            protected override void PerformTweening(double frame) {
                double initialX;
                double finalX;

                if (frame <= .1) {
                    frame = frame / .1;
                    initialX = 0;
                    finalX = _distance;
                }
                else if (frame <= .3) {
                    frame = (frame - .1) / .2;
                    initialX = _distance;
                    finalX = -_distance;
                }
                else if (frame <= .5) {
                    frame = (frame - .3) / .2;
                    initialX = -_distance;
                    finalX = _distance;
                }
                else if (frame <= .7) {
                    frame = (frame - .5) / .2;
                    initialX = _distance;
                    finalX = -_distance;
                }
                else if (frame <= .9) {
                    frame = (frame - .7) / .2;
                    initialX = -_distance;
                    finalX = _distance;
                }
                else {
                    frame = (frame - .9) / .1;
                    initialX = _distance;
                    finalX = 0;
                }

                frame = Math.Min(frame, 1.0);

                _transform.X = initialX + (finalX - initialX) * frame;
            }
        }
    }
}
