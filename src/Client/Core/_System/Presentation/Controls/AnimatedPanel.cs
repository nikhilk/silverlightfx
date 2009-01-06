// AnimatedPanel.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Glitz;

namespace System.Windows.Controls {

    /// <summary>
    /// A base class for all panels providing animated layout.
    /// </summary>
    public abstract class AnimatedPanel : Panel {

        private static readonly DependencyProperty BoundsProperty =
            DependencyProperty.RegisterAttached("Bounds", typeof(Rect), typeof(AnimatedPanel), null);

        /// <summary>
        /// Represents the Duration property of an AnimatedPanel.
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(AnimatedPanel), null);

        /// <summary>
        /// Represents the Interpolation property of an AnimatedPanel.
        /// </summary>
        public static readonly DependencyProperty InterpolationProperty =
            DependencyProperty.Register("Interpolation", typeof(TweenInterpolation), typeof(AnimatedPanel), null);

        /// <summary>
        /// Represents the UseAnimatedLayout property of an AnimatedPanel.
        /// </summary>
        public static readonly DependencyProperty UseAnimatedLayoutProperty =
            DependencyProperty.Register("UseAnimatedLayout", typeof(bool), typeof(AnimatedPanel), null);

        private List<ProceduralAnimation> _animations;
        private TweenInterpolation _interpolation;
        private bool _useDefaultInterpolation;
        private TimeSpan _duration;
        private bool _useAnimation;

        private ProceduralAnimation _layoutAnimation;

        /// <summary>
        /// Initializes an instance of an AnimatedPanel.
        /// </summary>
        protected AnimatedPanel() {
            Duration = TimeSpan.FromMilliseconds(500);
            UseAnimatedLayout = false;

            _useDefaultInterpolation = true;
        }

        /// <summary>
        /// Gets or sets the duration over which the layout animation is performed.
        /// </summary>
        public TimeSpan Duration {
            get {
                return (TimeSpan)GetValue(DurationProperty);
            }
            set {
                SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the interpolation used to progress the layout animation.
        /// </summary>
        public TweenInterpolation Interpolation {
            get {
                return (TweenInterpolation)GetValue(InterpolationProperty);
            }
            set {
                SetValue(InterpolationProperty, value);
                _useDefaultInterpolation = false;
            }
        }

        /// <summary>
        /// Gets or sets whether the layout is to be performed in an animated manner.
        /// </summary>
        public bool UseAnimatedLayout {
            get {
                return (bool)GetValue(UseAnimatedLayoutProperty);
            }
            set {
                SetValue(UseAnimatedLayoutProperty, value);
            }
        }

        /// <summary>
        /// Arranges the element by animating it to the specified bounds.
        /// </summary>
        /// <param name="element">The element to animate and arrange.</param>
        /// <param name="finalRect">The bounds that the element should use once it is arranged.</param>
        protected void ArrangeElement(UIElement element, Rect finalRect) {
            ArrangeElement(element, finalRect, true);
        }

        /// <summary>
        /// Arranges the element by optionally animating it to the specified bounds.
        /// </summary>
        /// <param name="element">The element to animate and arrange.</param>
        /// <param name="finalRect">The bounds that the element should use once it is arranged.</param>
        /// <param name="isAnimated">Whether to animate the element or just arrange it immediately.</param>
        protected void ArrangeElement(UIElement element, Rect finalRect, bool isAnimated) {
            if (_animations == null) {
                throw new InvalidOperationException("Calls to ArrangeElement need to be within BeginArrange/EndArrange calls.");
            }

            if (isAnimated && _useAnimation) {
                Rect currentBounds = (Rect)element.GetValue(BoundsProperty);
                if ((currentBounds.Width == 0) && (currentBounds.Height == 0)) {
                    Rect initialRect = GetInitialRect(DesiredSize, finalRect, element);
                    element.Arrange(initialRect);

                    currentBounds = initialRect;
                }

                RectAnimation animation = new RectAnimation(element, currentBounds, finalRect, _duration);
                animation.Interpolation = _interpolation;

                _animations.Add(animation);
            }
            else {
                element.Arrange(finalRect);
                element.SetValue(BoundsProperty, finalRect);
            }
        }

        /// <summary>
        /// Creates a new layout animation when an arrange pass starts.
        /// </summary>
        protected void BeginArrange() {
            if (_layoutAnimation != null) {
                if (_layoutAnimation.IsPlaying) {
                    _layoutAnimation.Stop(ProceduralAnimationStopState.Abort);
                }
                _layoutAnimation = null;
            }

            _useAnimation = UseAnimatedLayout;
            _animations = new List<ProceduralAnimation>();

            if (_useAnimation) {
                if (_useDefaultInterpolation) {
                    _interpolation = EasingInterpolation.Default;
                }
                else {
                    _interpolation = Interpolation;
                    if (_interpolation.IsLinearInterpolation) {
                        _interpolation = null;
                    }
                }
                _duration = Duration;
            }
        }

        /// <summary>
        /// Plays the current layout animation when an arrange pass is completed.
        /// </summary>
        protected void EndArrange() {
            if ((_animations != null) && (_animations.Count != 0)) {
                _layoutAnimation = new ProceduralAnimationSet(_animations.ToArray());
                _layoutAnimation.Play(this);
            }

            _animations = null;
        }

        /// <summary>
        /// Gets the initial bounds of an element.
        /// </summary>
        /// <param name="panelSize">The size of this panel.</param>
        /// <param name="elementRect">The computed bounds of the panel.</param>
        /// <param name="element">The element whose initial bounds are required.</param>
        /// <returns></returns>
        protected virtual Rect GetInitialRect(Size panelSize, Rect elementRect, UIElement element) {
            return new Rect(0, 0, elementRect.Width, elementRect.Height);
        }


        private sealed class RectAnimation : TweenAnimation {

            private UIElement _element;
            private Rect _initialRect;
            private Rect _finalRect;

            public RectAnimation(UIElement element, Rect initialRect, Rect finalRect, TimeSpan duration)
                : base(duration) {
                _element = element;
                _initialRect = initialRect;
                _finalRect = finalRect;
            }

            protected override void PerformTweening(double frame) {
                double left = _initialRect.Left + (_finalRect.Left - _initialRect.Left) * frame;
                double top = _initialRect.Top + (_finalRect.Top - _initialRect.Top) * frame;
                double width = _initialRect.Width + (_finalRect.Width - _initialRect.Width) * frame;
                double height = _initialRect.Height + (_finalRect.Height - _initialRect.Height) * frame;

                Rect bounds = new Rect(left, top, width, height);
                _element.SetValue(BoundsProperty, bounds);
                _element.Arrange(bounds);
            }
        }
    }
}
