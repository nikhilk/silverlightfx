// Transition.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// Represents a transition effect that can be attached to a container
    /// with two child elements. The transition shows one element and hides the other.
    /// </summary>
    public abstract class Transition : Effect {

        /// <summary>
        /// Creates the animation associated with the transition for the specified direction.
        /// Forward implies going from the first child to the second child in the container.
        /// Reverse implies second to the first.
        /// </summary>
        /// <param name="container">The container that contains the controls to transition.</param>
        /// <param name="direction">The direction of the transition.</param>
        /// <returns>The animation representing the transition.</returns>
        protected abstract ProceduralAnimation CreateTransitionAnimation(Panel container, EffectDirection direction);

        /// <internalonly />
        protected internal sealed override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            Panel container = GetTarget() as Panel;
            if (container == null) {
                throw new InvalidOperationException("A FlipEffect's target must be a Panel.");
            }

            if (container.Children.Count != 2) {
                throw new InvalidOperationException("A FlipEffect's target Panel must have 2 children.");
            }

            return CreateTransitionAnimation(container, direction);
        }

        /// <internalonly />
        protected override void OnCompleted() {
            base.OnCompleted();

            Panel container = GetTarget() as Panel;
            if (container.Children.Count == 2) {
                UIElement topContent = container.Children[1];
                UIElement bottomContent = container.Children[0];

                if (Direction == EffectDirection.Forward) {
                    topContent.Visibility = Visibility.Collapsed;
                    topContent.IsHitTestVisible = false;
                    bottomContent.IsHitTestVisible = true;
                }
                else {
                    topContent.IsHitTestVisible = true;
                    bottomContent.IsHitTestVisible = false;
                    bottomContent.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <internalonly />
        protected override void OnStarting() {
            base.OnStarting();

            Panel container = GetTarget() as Panel;
            UIElement topContent = container.Children[1];
            UIElement bottomContent = container.Children[0];

            topContent.Visibility = Visibility.Visible;
            bottomContent.Visibility = Visibility.Visible;
        }
    }
}
