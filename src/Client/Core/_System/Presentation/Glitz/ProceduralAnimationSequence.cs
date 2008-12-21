// ProceduralAnimationSequence.cs
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
    /// An animation that composes a set of multiple animations into a
    /// sequence with one animation following the other.
    /// </summary>
    public sealed class ProceduralAnimationSequence : ProceduralAnimation {

        private ProceduralAnimation[] _animations;
        private TimeSpan _successionDelay;

        private int _current;
        private bool _nextAnimation;
        private DateTime _successionTimeStamp;

        /// <summary>
        /// Creates an AnimationSequence with the specified set of individual
        /// animations.
        /// </summary>
        /// <param name="animations"></param>
        public ProceduralAnimationSequence(params ProceduralAnimation[] animations) {
            if ((animations == null) || (animations.Length == 0)) {
                throw new ArgumentNullException("animations");
            }

            _animations = (ProceduralAnimation[])animations.Clone();
            _current = -1;
        }

        /// <summary>
        /// The delay in milliseconds between animations within the
        /// animation sequence.
        /// </summary>
        public TimeSpan SuccessionDelay {
            get {
                return _successionDelay;
            }
            set {
                _successionDelay = value;
            }
        }

        /// <internalonly />
        protected override void PlayCore() {
            _nextAnimation = false;
            if (IsReversed == false) {
                _current = 0;
            }
            else {
                _current = _animations.Length - 1;
            }
            _animations[_current].OnPlay(IsReversed);
        }

        /// <internalonly />
        protected internal override void Repeat(bool reverse) {
            base.Repeat(reverse);

            _nextAnimation = false;
            if (reverse == false) {
                _current = 0;
            }
            else {
                _current = _animations.Length - 1;
            }
            _animations[_current].OnPlay(reverse);
        }

        /// <internalonly />
        protected override bool ProgressCore(bool startRepetition, bool startReverse, DateTime timeStamp) {
            ProceduralAnimation animation = _animations[_current];

            if (_nextAnimation) {
                if ((_successionDelay.TotalMilliseconds != 0) &&
                    ((_successionTimeStamp + _successionDelay) > timeStamp)) {
                    return false;
                }

                _nextAnimation = false;
                animation.OnPlay(IsReversed);
            }

            bool completed = animation.OnProgress(timeStamp);
            if (completed) {
                animation.OnStop(/* completed */ true, ProceduralAnimationStopState.Complete);
                if (IsReversed == false) {
                    _current++;
                }
                else {
                    _current--;
                }
                _nextAnimation = true;
                _successionTimeStamp = timeStamp;
            }

            return completed && ((_current == _animations.Length) || (_current == -1));
        }

        /// <internalonly />
        protected override void StopCore(bool completed, ProceduralAnimationStopState stopState) {
            if (completed == false) {
                ProceduralAnimation animation = _animations[_current];
                animation.OnStop(/* completed */ false, stopState);
            }
        }
    }
}
