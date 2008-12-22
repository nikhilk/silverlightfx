// Fade.cs
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
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents a fade effect that fades in the associated element
    /// </summary>
    public class Fade : Effect {

        private double _fadeOpacity;
        private double _initialOpacity;

        /// <summary>
        /// Initializes an instance of a FadeEffect.
        /// </summary>
        public Fade() {
            _fadeOpacity = 1;
            _initialOpacity = -1;
        }

        /// <summary>
        /// Gets or sets the final value of the opacity that this effect should animate to.
        /// </summary>
        public double FadeOpacity {
            get {
                return _fadeOpacity;
            }
            set {
                if ((value < 0) || (value > 1)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _fadeOpacity = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            FrameworkElement target = GetTarget();

            if (_initialOpacity == -1) {
                _initialOpacity = target.Opacity;
            }

            DoubleAnimation animation;
            if (direction == EffectDirection.Forward) {
                animation = new DoubleAnimation(target, UIElement.OpacityProperty, Duration, _fadeOpacity);
            }
            else {
                animation = new DoubleAnimation(target, UIElement.OpacityProperty, Duration, _initialOpacity);
            }

            animation.AutoReverse = AutoReverse;
            animation.EasingFunction = GetEasingFunction();

            return animation;
        }
    }
}
