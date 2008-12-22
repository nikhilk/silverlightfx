// Move.cs
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
    /// Represents an effect that moves the associated element
    /// </summary>
    public class Move : Effect {

        private TranslateTransform _translateTransform;

        private double _horizontalMovement;
        private double _verticalMovement;

        private double _initialX;
        private double _initialY;

        /// <summary>
        /// Gets or sets the amount to move in the horizontal direction.
        /// </summary>
        public double HorizontalMovement {
            get {
                return _horizontalMovement;
            }
            set {
                _horizontalMovement = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount to move in the vertical direction.
        /// </summary>
        public double VerticalMovement {
            get {
                return _verticalMovement;
            }
            set {
                _verticalMovement = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            if (_translateTransform == null) {
                Transform existingTransform = GetTarget().RenderTransform;
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

                if (_translateTransform == null) {
                    throw new InvalidOperationException("The element does not have a translate transform associated with it.");
                }

                _initialX = _translateTransform.X;
                _initialY = _translateTransform.Y;
            }

            ProceduralAnimationEasingFunction easing = GetEasingFunction();
            DoubleAnimation xAnimation = null;
            DoubleAnimation yAnimation = null;

            if (_horizontalMovement != 0) {
                double targetValue = direction == EffectDirection.Forward ? _horizontalMovement : _initialX;
                xAnimation = new DoubleAnimation(_translateTransform, TranslateTransform.XProperty, Duration, targetValue);
                xAnimation.EasingFunction = easing;
            }

            if (_verticalMovement != 0) {
                double targetValue = direction == EffectDirection.Forward ? _verticalMovement : _initialY;
                yAnimation = new DoubleAnimation(_translateTransform, TranslateTransform.YProperty, Duration, targetValue);
                yAnimation.EasingFunction = easing;
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
