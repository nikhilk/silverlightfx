// Slide.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Transitions {

    /// <summary>
    /// Represents an transition that slides top content to reveal underneath content.
    /// </summary>
    public class Slide : Transition {

        private SlideMode _mode;

        /// <summary>
        /// Gets or sets the specific mode of slide behavior to use for the transition.
        /// </summary>
        public SlideMode Mode {
            get {
                return _mode;
            }
            set {
                if ((value < SlideMode.Right) || (value > SlideMode.Up)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _mode = value;
            }
        }

        /// <internalonly />
        protected override ProceduralAnimation CreateTransitionAnimation(Panel container, EffectDirection direction) {
            bool forward = direction == EffectDirection.Forward;
            double width = container.ActualWidth;
            double height = container.ActualHeight;

            TranslateTransform topTransform = null;
            TranslateTransform bottomTransform = null;
            Transform existingTransform = null;

            FrameworkElement topContent = (FrameworkElement)container.Children[1];
            FrameworkElement bottomContent = (FrameworkElement)container.Children[0];

            existingTransform = topContent.RenderTransform;
            if (existingTransform != null) {
                topTransform = existingTransform as TranslateTransform;

                if (topTransform == null) {
                    TransformGroup transformGroup = existingTransform as TransformGroup;

                    if (transformGroup != null) {
                        foreach (Transform transform in transformGroup.Children) {
                            topTransform = transform as TranslateTransform;
                            if (topTransform != null) {
                                break;
                            }
                        }
                        if (topTransform == null) {
                            topTransform = new TranslateTransform();
                            transformGroup.Children.Add(topTransform);
                        }
                    }
                }
            }
            existingTransform = bottomContent.RenderTransform;
            if (existingTransform != null) {
                bottomTransform = existingTransform as TranslateTransform;

                if (bottomTransform == null) {
                    TransformGroup transformGroup = existingTransform as TransformGroup;

                    if (transformGroup != null) {
                        foreach (Transform transform in transformGroup.Children) {
                            bottomTransform = transform as TranslateTransform;
                            if (bottomTransform != null) {
                                break;
                            }
                        }
                        if (bottomTransform == null) {
                            bottomTransform = new TranslateTransform();
                            transformGroup.Children.Add(bottomTransform);
                        }
                    }
                }
            }

            if (topTransform == null) {
                topTransform = new TranslateTransform();
                topContent.RenderTransform = topTransform;
            }
            if (bottomTransform == null) {
                bottomTransform = new TranslateTransform();
                bottomContent.RenderTransform = bottomTransform;
            }

            SlideAnimation slideAnimation = new SlideAnimation(container, Duration, forward);
            DoubleAnimation topAnimation = null;
            DoubleAnimation bottomAnimation = null;
            switch (_mode) {
                case SlideMode.Left:
                    if (forward) {
                        bottomTransform.X = width;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.XProperty, Duration, -width);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.XProperty, Duration, 0);
                    }
                    else {
                        topTransform.X = -width;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.XProperty, Duration, 0);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.XProperty, Duration, width);
                    }
                    break;
                case SlideMode.Right:
                    if (forward) {
                        bottomTransform.X = -width;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.XProperty, Duration, width);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.XProperty, Duration, 0);
                    }
                    else {
                        topTransform.X = width;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.XProperty, Duration, 0);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.XProperty, Duration, -width);
                    }
                    break;
                case SlideMode.Up:
                    if (forward) {
                        bottomTransform.Y = height;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.YProperty, Duration, -height);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.YProperty, Duration, 0);
                    }
                    else {
                        topTransform.Y = -height;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.YProperty, Duration, 0);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.YProperty, Duration, height);
                    }
                    break;
                case SlideMode.Down:
                    if (forward) {
                        bottomTransform.Y = -height;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.YProperty, Duration, height);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.YProperty, Duration, 0);
                    }
                    else {
                        topTransform.Y = height;
                        topAnimation = new DoubleAnimation(topTransform, TranslateTransform.YProperty, Duration, 0);
                        bottomAnimation = new DoubleAnimation(bottomTransform, TranslateTransform.YProperty, Duration, -height);
                    }
                    break;
            }

            TweenInterpolation interpolation = GetEffectiveInterpolation();
            topAnimation.Interpolation = interpolation;
            bottomAnimation.Interpolation = interpolation;

            return new ProceduralAnimationSet(slideAnimation, topAnimation, bottomAnimation);
        }


        private sealed class SlideAnimation : TweenAnimation {

            private Panel _container;
            private bool _forward;

            public SlideAnimation(Panel container, TimeSpan duration, bool forward)
                : base(duration) {
                _container = container;
                _forward = forward;
            }

            protected override void PerformCleanup() {
                _container.Clip = null;
                if (_forward) {
                    _container.Children[1].Visibility = Visibility.Collapsed;
                }
                else {
                    _container.Children[0].Visibility = Visibility.Collapsed;
                }
            }

            protected override void PerformSetup() {
                _container.Clip =
                    new RectangleGeometry() {
                        Rect = new Rect(0, 0, _container.ActualWidth, _container.ActualHeight)
                    };
                _container.Children[0].Visibility = Visibility.Visible;
                _container.Children[1].Visibility = Visibility.Visible;
            }

            protected override void PerformTweening(double frame) {
            }
        }
    }
}
