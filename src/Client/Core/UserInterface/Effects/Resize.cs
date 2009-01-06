// Resize.cs
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
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents a fade effect that fades in the associated element
    /// </summary>
    public class Resize : Effect {

        private ScaleTransform _scaleTransform;

        private double _scaleXRatio;
        private double _scaleYRatio;

        private double _initialX;
        private double _initialY;

        /// <summary>
        /// Initializes an instance of a ScaleEffect.
        /// </summary>
        public Resize() {
            _scaleXRatio = 1;
            _scaleYRatio = 1;
        }

        /// <summary>
        /// Gets or sets the X scaling factor used by the effect.
        /// </summary>
        public double ScaleXRatio {
            get {
                return _scaleXRatio;
            }
            set {
                if (value == 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _scaleXRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y scaling factor used by the effect.
        /// </summary>
        public double ScaleYRatio {
            get {
                return _scaleYRatio;
            }
            set {
                if (value == 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _scaleYRatio = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            if (_scaleTransform == null) {
                Transform existingTransform = GetTarget().RenderTransform;
                if (existingTransform != null) {
                    _scaleTransform = existingTransform as ScaleTransform;

                    if (_scaleTransform == null) {
                        TransformGroup transformGroup = existingTransform as TransformGroup;

                        if (transformGroup != null) {
                            foreach (Transform transform in transformGroup.Children) {
                                _scaleTransform = transform as ScaleTransform;
                                if (_scaleTransform != null) {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (_scaleTransform == null) {
                    throw new InvalidOperationException("The element does not have a scale transform associated with it.");
                }

                _initialX = _scaleTransform.ScaleX;
                _initialY = _scaleTransform.ScaleY;
            }

            TweenInterpolation interpolation = GetEffectiveInterpolation();
            DoubleAnimation xAnimation = null;
            DoubleAnimation yAnimation = null;

            if (_scaleXRatio != 1) {
                double targetValue = direction == EffectDirection.Forward ? _initialX * _scaleXRatio : _initialX;

                xAnimation = new DoubleAnimation(_scaleTransform, ScaleTransform.ScaleXProperty, Duration,
                                                 targetValue);
                xAnimation.Interpolation = interpolation;
            }

            if (_scaleYRatio != 1) {
                double targetValue = direction == EffectDirection.Forward ? _initialY * _scaleYRatio : _initialY;

                yAnimation = new DoubleAnimation(_scaleTransform, ScaleTransform.ScaleYProperty, Duration,
                                                 targetValue);
                yAnimation.Interpolation = interpolation;
            }

            if ((xAnimation != null) && (yAnimation != null)) {
                ProceduralAnimationSet animation = new ProceduralAnimationSet(xAnimation, yAnimation);
                animation.AutoReverse = AutoReverse;

                return animation;
            }
            else if (xAnimation != null) {
                xAnimation.AutoReverse = AutoReverse;
                return xAnimation;
            }
            else if (yAnimation != null) {
                yAnimation.AutoReverse = AutoReverse;
                return yAnimation;
            }
            return null;
        }
    }
}
