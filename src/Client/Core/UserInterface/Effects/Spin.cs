// Spin.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
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
    /// Represents a spin effect that rotates the associated element
    /// </summary>
    public class Spin : AnimationEffect {

        private RotateTransform _rotateTransform;
        private double _spinAngle;

        private double _initialAngle;

        /// <summary>
        /// Gets or sets the angle used by the effect.
        /// </summary>
        public double SpinAngle {
            get {
                return _spinAngle;
            }
            set {
                _spinAngle = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(AnimationEffectDirection direction) {
            if (_rotateTransform == null) {
                Transform existingTransform = GetTarget().RenderTransform;
                if (existingTransform != null) {
                    _rotateTransform = existingTransform as RotateTransform;

                    if (_rotateTransform == null) {
                        TransformGroup transformGroup = existingTransform as TransformGroup;

                        if (transformGroup != null) {
                            foreach (Transform transform in transformGroup.Children) {
                                _rotateTransform = transform as RotateTransform;
                                if (_rotateTransform != null) {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (_rotateTransform == null) {
                    throw new InvalidOperationException("The element does not have a rotate transform associated with it.");
                }

                _initialAngle = _rotateTransform.Angle;
            }

            if (_spinAngle != 0) {
                double targetValue = direction == AnimationEffectDirection.Forward ? _initialAngle + _spinAngle : _initialAngle;
                DoubleAnimation animation = new DoubleAnimation(_rotateTransform, RotateTransform.AngleProperty,
                                                                Duration, targetValue);
                animation.Interpolation = GetEffectiveInterpolation();
                animation.AutoReverse = AutoReverse;

                return animation;
            }

            return null;
        }
    }
}
