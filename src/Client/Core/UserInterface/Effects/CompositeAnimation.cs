// CompositeAnimation.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Glitz;
using System.Windows.Threading;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents an effect that contains a set of nested effects.
    /// </summary>
    [ContentProperty("Effects")]
    public class CompositeAnimation : AnimationEffect {

        private AnimationEffectCollection _effects;
        private AnimationComposition _composition;

        private bool _childrenInitialized;

        /// <summary>
        /// Initializes an instance of a CompositeAnimation.
        /// </summary>
        public CompositeAnimation() {
            _effects = new AnimationEffectCollection();
        }

        /// <summary>
        /// The composition method used to play the child effects.
        /// </summary>
        public AnimationComposition Composition {
            get {
                return _composition;
            }
            set {
                _composition = value;
            }
        }

        /// <summary>
        /// The list of effects to be composed.
        /// </summary>
        public AnimationEffectCollection Effects {
            get {
                return _effects;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(AnimationEffectDirection direction) {
            if (_effects.Count == 0) {
                throw new InvalidOperationException("A CompositeEffect must have more than 1 nested child effect.");
            }

            if (_childrenInitialized == false) {
                foreach (AnimationEffect childEffect in _effects) {
                    ((IAttachedObject)childEffect).Attach(AssociatedObject);
                }

                _childrenInitialized = true;
            }

            List<ProceduralAnimation> animations = new List<ProceduralAnimation>(_effects.Count);
            foreach (AnimationEffect childEffect in _effects) {
                AnimationEffectDirection childDirection = direction;
                if (childEffect.AutoReverse) {
                    childDirection = AnimationEffectDirection.Forward;
                }

                ProceduralAnimation childAnimation = childEffect.CreateEffectAnimation(childDirection);
                if (childAnimation != null) {
                    animations.Add(childAnimation);
                }
            }

            if (animations.Count != 0) {
                ProceduralAnimation[] animationItems = animations.ToArray();

                if (_composition == AnimationComposition.Parallel) {
                    ProceduralAnimationSet animation = new ProceduralAnimationSet(animationItems);
                    animation.AutoReverse = AutoReverse;

                    return animation;
                }
                else {
                    ProceduralAnimationSequence animation = new ProceduralAnimationSequence(animationItems);
                    animation.AutoReverse = AutoReverse;

                    return animation;
                }
            }

            return null;
        }
    }
}
