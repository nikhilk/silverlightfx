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
        /// Represents the Easing property of an AnimatedPanel.
        /// </summary>
        public static readonly DependencyProperty EasingProperty =
            DependencyProperty.Register("Easing", typeof(LayoutEasing), typeof(AnimatedPanel), null);

        /// <summary>
        /// Represents the UseAnimatedLayout property of an AnimatedPanel.
        /// </summary>
        public static readonly DependencyProperty UseAnimatedLayoutProperty =
            DependencyProperty.Register("UseAnimatedLayout", typeof(bool), typeof(AnimatedPanel), null);

        private List<ProceduralAnimation> _animations;
        private ProceduralAnimationEasingFunction _easingFunction;
        private TimeSpan _duration;
        private bool _useAnimation;

        private ProceduralAnimation _layoutAnimation;

        /// <summary>
        /// Initializes an instance of an AnimatedPanel.
        /// </summary>
        protected AnimatedPanel() {
            Easing = LayoutEasing.None;
            Duration = TimeSpan.FromMilliseconds(500);
            UseAnimatedLayout = false;
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
        /// Gets or sets the easing effect applied to the layout animation.
        /// </summary>
        public LayoutEasing Easing {
            get {
                return (LayoutEasing)GetValue(EasingProperty);
            }
            set {
                SetValue(EasingProperty, value);
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
                _easingFunction = GetEasingFunction();
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

        private ProceduralAnimationEasingFunction GetEasingFunction() {
            switch (Easing) {
                case LayoutEasing.QuadraticIn:
                    return EasingFunctions.EaseQuadraticIn;
                case LayoutEasing.QuadraticOut:
                    return EasingFunctions.EaseQuadraticOut;
                case LayoutEasing.QuadraticInOut:
                    return EasingFunctions.EaseQuadraticInOut;
                case LayoutEasing.BounceIn:
                    return EasingFunctions.EaseBounceIn;
                case LayoutEasing.BounceOut:
                    return EasingFunctions.EaseBounceOut;
                case LayoutEasing.BounceInOut:
                    return EasingFunctions.EaseBounceInOut;
                case LayoutEasing.BackIn:
                    return EasingFunctions.EaseBackIn;
                case LayoutEasing.BackOut:
                    return EasingFunctions.EaseBackOut;
                case LayoutEasing.BackInOut:
                    return EasingFunctions.EaseBackInOut;
                case LayoutEasing.ElasticIn:
                    return EasingFunctions.EaseElasticIn;
                case LayoutEasing.ElasticOut:
                    return EasingFunctions.EaseElasticOut;
                case LayoutEasing.ElasticInOut:
                    return EasingFunctions.EaseElasticInOut;
            }

            return null;
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
                animation.EasingFunction = _easingFunction;

                _animations.Add(animation);
            }
            else {
                element.Arrange(finalRect);
                element.SetValue(BoundsProperty, finalRect);
            }
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
