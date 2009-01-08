// CrossFade.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
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
    /// Represents a fade effect that can be attached to a container
    /// with two child elements. The effect fades one element to another.
    /// </summary>
    public class CrossFade : Transition {

        /// <internalonly />
        protected override ProceduralAnimation CreateTransitionAnimation(Panel container, EffectDirection direction) {
            TweenInterpolation interpolation = GetEffectiveInterpolation();

            DoubleAnimation frontAnimation =
                new DoubleAnimation(container.Children[1], UIElement.OpacityProperty, Duration,
                                    (direction == EffectDirection.Forward ? 0 : 1));
            frontAnimation.Interpolation = interpolation;

            DoubleAnimation backAnimation =
                new DoubleAnimation(container.Children[0], UIElement.OpacityProperty, Duration,
                                    (direction == EffectDirection.Forward ? 1 : 0));
            backAnimation.Interpolation = interpolation;

            return new ProceduralAnimationSet(frontAnimation, backAnimation);
        }
    }
}
