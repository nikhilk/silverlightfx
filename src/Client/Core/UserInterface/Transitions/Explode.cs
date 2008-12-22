// Explode.cs
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
    /// Represents an explode-like effect that can be attached to a container
    /// with two child elements. The effect zooms out and fade out one element
    /// to reveal the other.
    /// </summary>
    public class Explode : Transition {

        private double _scaleRatio;

        private double _initialScaleX;
        private double _initialScaleY;

        /// <summary>
        /// Initializes an instance of ExplodeTransition.
        /// </summary>
        public Explode() {
            _scaleRatio = 2;
        }

        /// <summary>
        /// Gets or sets the ratio by which elements are scaled during the
        /// explode transition.
        /// </summary>
        public double ScaleRatio {
            get {
                return _scaleRatio;
            }
            set {
                if (value <= 1) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _scaleRatio = value;
            }
        }

        /// <internalonly />
        protected override ProceduralAnimation CreateTransitionAnimation(Panel container, EffectDirection direction) {
            FrameworkElement topElement = (FrameworkElement)container.Children[1];
            FrameworkElement bottomElement = (FrameworkElement)container.Children[0];
            ScaleTransform scaleTransform = null;

            Transform existingTransform = topElement.RenderTransform;
            if (existingTransform != null) {
                scaleTransform = existingTransform as ScaleTransform;

                if (scaleTransform == null) {
                    TransformGroup transformGroup = existingTransform as TransformGroup;

                    if (transformGroup != null) {
                        foreach (Transform transform in transformGroup.Children) {
                            scaleTransform = transform as ScaleTransform;
                            if (transform != null) {
                                break;
                            }
                        }
                        if (scaleTransform == null) {
                            scaleTransform = new ScaleTransform();
                            transformGroup.Children.Add(scaleTransform);
                        }
                    }
                }
            }

            if (scaleTransform == null) {
                scaleTransform = new ScaleTransform();
                topElement.RenderTransform = scaleTransform;
            }

            if (_initialScaleX == 0) {
                _initialScaleX = scaleTransform.ScaleX;
                _initialScaleY = scaleTransform.ScaleY;
            }

            scaleTransform.CenterX = container.ActualWidth / 2;
            scaleTransform.CenterY = container.ActualHeight / 2;

            ProceduralAnimationEasingFunction easing = GetEasingFunction();
            DoubleAnimation scaleXAnimation;
            DoubleAnimation scaleYAnimation;
            DoubleAnimation fadeAnimation;

            if (direction == EffectDirection.Forward) {
                fadeAnimation = new DoubleAnimation(topElement, UIElement.OpacityProperty, Duration, 0);
                scaleXAnimation = new DoubleAnimation(scaleTransform, ScaleTransform.ScaleXProperty, Duration, _initialScaleX * _scaleRatio);
                scaleYAnimation = new DoubleAnimation(scaleTransform, ScaleTransform.ScaleYProperty, Duration, _initialScaleY * _scaleRatio);
            }
            else {
                fadeAnimation = new DoubleAnimation(topElement, UIElement.OpacityProperty, Duration, 1);
                scaleXAnimation = new DoubleAnimation(scaleTransform, ScaleTransform.ScaleXProperty, Duration, _initialScaleX);
                scaleYAnimation = new DoubleAnimation(scaleTransform, ScaleTransform.ScaleYProperty, Duration, _initialScaleY);
            }

            scaleXAnimation.EasingFunction = easing;
            scaleYAnimation.EasingFunction = easing;
            fadeAnimation.EasingFunction = easing;

            return new ProceduralAnimationSet(scaleXAnimation, scaleYAnimation, fadeAnimation);
        }
    }
}
