// Pulsate.cs
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
    /// Represents an effect that fades in and out the associated element.
    /// </summary>
    public class Pulsate : Effect {

        private double _fadeOpacity;

        /// <summary>
        /// Initializes an instance of a ShakeEffect.
        /// </summary>
        public Pulsate() {
            _fadeOpacity = 0.25;
        }

        /// <summary>
        /// Gets or sets the value to fade to.
        /// </summary>
        public double FadeOpacity {
            get {
                return _fadeOpacity;
            }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _fadeOpacity = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            FrameworkElement target = GetTarget();

            PulsateAnimation animation = new PulsateAnimation(target, Duration, _fadeOpacity);
            animation.Interpolation = GetEffectiveInterpolation();

            return animation;
        }


        private sealed class PulsateAnimation : TweenAnimation {

            private FrameworkElement _element;
            private double _initialOpacity;
            private double _fadeOpacity;

            public PulsateAnimation(FrameworkElement element, TimeSpan duration, double fadeOpacity)
                : base(duration) {
                _element = element;
                _initialOpacity = _element.Opacity;
                _fadeOpacity = fadeOpacity;
            }

            protected override void PerformTweening(double frame) {
                double initialOpacity;
                double finalOpacity;

                if (frame <= .15) {
                    frame = frame / .15;
                    initialOpacity = _initialOpacity;
                    finalOpacity = _fadeOpacity;
                }
                else if (frame <= .3) {
                    frame = (frame - .15) / .15;
                    initialOpacity = _fadeOpacity;
                    finalOpacity = _initialOpacity;
                }
                else if (frame <= .45) {
                    frame = (frame - .3) / .15;
                    initialOpacity = _initialOpacity;
                    finalOpacity = _fadeOpacity;
                }
                else if (frame <= .6) {
                    frame = (frame - .45) / .15;
                    initialOpacity = _fadeOpacity;
                    finalOpacity = _initialOpacity;
                }
                else if (frame <= .75) {
                    frame = (frame - .45) / .15;
                    initialOpacity = _initialOpacity;
                    finalOpacity = _fadeOpacity;
                }
                else {
                    frame = (frame - .75) / .15;
                    initialOpacity = _fadeOpacity;
                    finalOpacity = _initialOpacity;
                }

                frame = Math.Min(frame, 1.0);

                _element.Opacity = initialOpacity + (finalOpacity - initialOpacity) * frame;
            }
        }
    }
}
