// ProceduralAnimationSet.cs
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

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// An animation that composes a set of multiple animations playing
    /// simulataneously.
    /// </summary>
    public sealed class ProceduralAnimationSet : ProceduralAnimation {

        private ProceduralAnimation[] _animations;

        /// <summary>
        /// Creates an AnimationSet with the specified set of individual
        /// animations.
        /// </summary>
        /// <param name="animations">The individual animations.</param>
        public ProceduralAnimationSet(params ProceduralAnimation[] animations) {
            if ((animations == null) || (animations.Length == 0)) {
                throw new ArgumentNullException("animations");
            }

            _animations = (ProceduralAnimation[])animations.Clone();
        }

        /// <internalonly />
        protected internal override void Repeat(bool reverse) {
            base.Repeat(reverse);
            foreach (ProceduralAnimation animation in _animations) {
                animation.Repeat(reverse);
            }
        }

        /// <internalonly />
        protected override void PlayCore() {
            foreach (ProceduralAnimation animation in _animations) {
                animation.OnPlay(IsReversed);
            }
        }

        /// <internalonly />
        protected override bool ProgressCore(bool startRepetition, bool startReverse, DateTime timeStamp) {
            int activeCount = 0;
            int completedCount = 0;

            foreach (ProceduralAnimation animation in _animations) {
                if (animation.IsPlaying == false) {
                    continue;
                }

                activeCount++;

                bool completed = animation.OnProgress(timeStamp);
                if (completed) {
                    completedCount++;
                    animation.OnStop(/* completed */ true, ProceduralAnimationStopState.Complete);
                }
            }

            return (completedCount == activeCount);
        }

        /// <internalonly />
        protected override void StopCore(bool completed, ProceduralAnimationStopState stopState) {
            if (completed == false) {
                foreach (ProceduralAnimation animation in _animations) {
                    if (animation.IsPlaying) {
                        animation.OnStop(/* completed */ false, stopState);
                    }
                }
            }
        }
    }
}
