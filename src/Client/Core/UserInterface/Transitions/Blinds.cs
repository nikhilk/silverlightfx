// Blinds.cs
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
    /// Represents a blind transition that switches between content by raising or lowering
    /// one contained content element over another.
    /// </summary>
    public class Blinds : Transition {

        private BlindsMode _mode;

        /// <summary>
        /// Gets or sets the nature of the transition.
        /// </summary>
        public BlindsMode Mode {
            get {
                return _mode;
            }
            set {
                if ((value < BlindsMode.Up) || (value > BlindsMode.Down)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _mode = value;
            }
        }

        /// <internalonly />
        protected override ProceduralAnimation CreateTransitionAnimation(Panel container, EffectDirection direction) {
            FrameworkElement topContent = (FrameworkElement)container.Children[1];
            FrameworkElement bottomContent = (FrameworkElement)container.Children[0];

            BlindsAnimation animation = new BlindsAnimation(topContent, bottomContent, Duration, _mode, direction == EffectDirection.Forward);
            animation.Interpolation = GetEffectiveInterpolation();

            return animation;
        }


        private sealed class BlindsAnimation : TweenAnimation {

            private FrameworkElement _topElement;
            private FrameworkElement _bottomElement;

            private double _width;
            private double _initialHeight;
            private double _finalHeight;

            public BlindsAnimation(FrameworkElement topElement, FrameworkElement bottomElement, TimeSpan duration, BlindsMode mode, bool forward)
                : base(duration) {
                _topElement = topElement;
                _bottomElement = bottomElement;

                _width = topElement.ActualWidth;
                if (mode == BlindsMode.Up) {
                    _initialHeight = topElement.ActualHeight;
                    _finalHeight = 0;
                }
                else {
                    _initialHeight = 0;
                    _finalHeight = topElement.ActualHeight;
                }

                if (forward == false) {
                    double temp = _initialHeight;
                    _initialHeight = _finalHeight;
                    _finalHeight = temp;
                }
            }

            protected override void PerformTweening(double frame) {
                double currentHeight = _initialHeight + (_finalHeight - _initialHeight) * frame;

                _topElement.Clip = new RectangleGeometry() {
                    Rect = new Rect(0, 0, _width, currentHeight)
                };
            }
        }
    }
}
