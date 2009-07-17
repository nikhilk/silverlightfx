// Spin3D.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Glitz;
using System.Windows;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents a spin effect that rotates the associated element along the
    /// X, Y or Z axes.
    /// </summary>
    public class Spin3D : AnimationEffect {

        private bool _spinAroundXAxis;
        private bool _spinAroundYAxis;
        private bool _spinAroundZAxis;

        private PlaneProjection _projection;

        /// <summary>
        /// Gets or sets whether the element should be spun around the x-axis.
        /// </summary>
        public bool SpinAroundXAxis {
            get {
                return _spinAroundXAxis;
            }
            set {
                _spinAroundXAxis = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the element should be spun around the y-axis.
        /// </summary>
        public bool SpinAroundYAxis {
            get {
                return _spinAroundYAxis;
            }
            set {
                _spinAroundYAxis = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the element should be spun around the z-axis.
        /// </summary>
        public bool SpinAroundZAxis {
            get {
                return _spinAroundZAxis;
            }
            set {
                _spinAroundZAxis = value;
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

            List<ProceduralAnimation> animations = new List<ProceduralAnimation>();

            if (_spinAroundXAxis) {
                DoubleAnimation xAnimation =
                    new DoubleAnimation(_projection, PlaneProjection.RotationXProperty, Duration, 360);
                xAnimation.Interpolation = GetEffectiveInterpolation();
                xAnimation.AutoReverse = true;
                animations.Add(xAnimation);
            }
            if (_spinAroundYAxis) {
                DoubleAnimation yAnimation =
                    new DoubleAnimation(_projection, PlaneProjection.RotationYProperty, Duration, 360);
                yAnimation.Interpolation = GetEffectiveInterpolation();
                yAnimation.AutoReverse = true;
                animations.Add(yAnimation);
            }
            if (_spinAroundZAxis) {
                DoubleAnimation zAnimation =
                    new DoubleAnimation(_projection, PlaneProjection.RotationZProperty, Duration, 360);
                zAnimation.Interpolation = GetEffectiveInterpolation();
                zAnimation.AutoReverse = true;
                animations.Add(zAnimation);
            }

            ProceduralAnimationSet animationSet = new ProceduralAnimationSet(animations.ToArray());
            animationSet.AutoReverse = AutoReverse;

            return animationSet;
        }
    }
}
