// Effect.cs
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
    public class CompositeEffect : Effect {

        private EffectCollection _effects;
        private EffectComposition _composition;

        private bool _childrenInitialized;

        /// <summary>
        /// Initializes an instance of a CompositeEffect.
        /// </summary>
        public CompositeEffect() {
            _effects = new EffectCollection();
        }

        /// <summary>
        /// The composition method used to play the child effects.
        /// </summary>
        public EffectComposition Composition {
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
        public EffectCollection Effects {
            get {
                return _effects;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            if (_effects.Count == 0) {
                throw new InvalidOperationException("A CompositeEffect must have more than 1 nested child effect.");
            }

            if (_childrenInitialized == false) {
                foreach (Effect childEffect in _effects) {
                    ((IAttachedObject)childEffect).Attach(AssociatedObject);
                }

                _childrenInitialized = true;
            }

            List<ProceduralAnimation> animations = new List<ProceduralAnimation>(_effects.Count);
            foreach (Effect childEffect in _effects) {
                EffectDirection childDirection = direction;
                if (childEffect.AutoReverse) {
                    childDirection = EffectDirection.Forward;
                }

                ProceduralAnimation childAnimation = childEffect.CreateEffectAnimation(childDirection);
                if (childAnimation != null) {
                    animations.Add(childAnimation);
                }
            }

            if (animations.Count != 0) {
                ProceduralAnimation[] animationItems = animations.ToArray();

                if (_composition == EffectComposition.Parallel) {
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
