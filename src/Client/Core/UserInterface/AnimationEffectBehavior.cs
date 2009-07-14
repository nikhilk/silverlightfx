// AnimationEffectBehavior.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A base class for behaviors that play effects.
    /// </summary>
    [ContentProperty("AnimationEffect")]
    public abstract class AnimationEffectBehavior : Behavior<FrameworkElement> {

        private AnimationEffect _animationEffect;

        /// <summary>
        /// Gets or sets the effect to be played by the behavior.
        /// </summary>
        public AnimationEffect AnimationEffect {
            get {
                return _animationEffect;
            }
            set {
                _animationEffect = value;

                if (AssociatedObject != null) {
                    if (((IAttachedObject)value).AssociatedObject != AssociatedObject) {
                        ((IAttachedObject)value).Detach();
                        ((IAttachedObject)value).Attach(AssociatedObject);
                    }
                }
            }
        }

        /// <internalonly />
        protected override void OnAttach() {
            if (_animationEffect != null) {
                if (((IAttachedObject)_animationEffect).AssociatedObject != null) {
                    ((IAttachedObject)_animationEffect).Detach();
                }
                ((IAttachedObject)_animationEffect).Attach(AssociatedObject);
            }

            base.OnAttach();
        }

        /// <internalonly />
        protected override void OnDetach() {
            if (_animationEffect != null) {
                ((IAttachedObject)_animationEffect).Detach();
            }

            base.OnDetach();
        }

        /// <summary>
        /// Plays the effect in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to play the effect in.</param>
        protected void PlayEffect(AnimationEffectDirection direction) {
            if (_animationEffect != null) {
                _animationEffect.PlayEffect(direction);
            }
        }
    }
}
