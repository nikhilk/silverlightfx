// Float.cs
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
using System.Windows.Media.Effects;
using System.Windows.Media.Glitz;
using System.Windows;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents an effect that floats an element by projecting it forward and
    /// adding a shadow.
    /// </summary>
    public class Float : AnimationEffect {

        private double _distance;
        private double _shadowLength;

        private DropShadowEffect _dropShadow;
        private PlaneProjection _projection;

        /// <summary>
        /// Gets or sets the distance by which the element is floated.
        /// </summary>
        public double Distance {
            get {
                return _distance;
            }
            set {
                _distance = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the shadow to create when the element
        /// is floated.
        /// </summary>
        public double ShadowLength {
            get {
                return _shadowLength;
            }
            set {
                _shadowLength = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(AnimationEffectDirection direction) {
            FrameworkElement target = GetTarget();

            if (_projection == null) {
                _projection = target.Projection as PlaneProjection;
                if (_projection == null) {
                    _projection = new PlaneProjection();
                    _projection.CenterOfRotationX = 0.5;
                    _projection.CenterOfRotationY = 0.5;
                    _projection.CenterOfRotationZ = 0.5;

                    target.Projection = _projection;
                }
            }

            DoubleAnimation zOffsetAnimation =
                new DoubleAnimation(_projection, PlaneProjection.GlobalOffsetZProperty, Duration,
                                    (direction == AnimationEffectDirection.Forward ? Distance : 0));
            zOffsetAnimation.Interpolation = GetEffectiveInterpolation();

            if (_shadowLength == 0) {
                zOffsetAnimation.AutoReverse = AutoReverse;

                return zOffsetAnimation;
            }

            if (_dropShadow == null) {
                _dropShadow = target.Effect as DropShadowEffect;
                if (_dropShadow == null) {
                    _dropShadow = new DropShadowEffect();
                    _dropShadow.BlurRadius = 0;
                    _dropShadow.ShadowDepth = 0;
                    _dropShadow.Color = Colors.Black;
                    _dropShadow.Opacity = 0.5;

                    target.Effect = _dropShadow;
                }
            }

            DoubleAnimation shadowAnimation =
                new DoubleAnimation(_dropShadow, DropShadowEffect.BlurRadiusProperty, Duration,
                                    (direction == AnimationEffectDirection.Forward ? _shadowLength : 0));
            shadowAnimation.Interpolation = GetEffectiveInterpolation();

            ProceduralAnimationSet animationSet = new ProceduralAnimationSet(zOffsetAnimation, shadowAnimation);
            animationSet.AutoReverse = AutoReverse;

            return animationSet;
        }
    }
}
