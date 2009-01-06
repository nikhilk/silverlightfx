// TimedAnimation.cs
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
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// The base class for animations that involve tweening over a
    /// specific time duration.
    /// </summary>
    public abstract class TweenAnimation : ProceduralAnimation {

        private TimeSpan _duration;
        private TimeSpan _startDelay;
        private TweenInterpolation _interpolation;

        private DateTime _startTimeStamp;
        private bool _isStarting;

        /// <summary>
        /// Initializes an instance of TweenAnimation.
        /// </summary>
        /// <param name="duration">The time span over which the animation runs.</param>
        protected TweenAnimation(TimeSpan duration) {
            _duration = duration;
        }

        /// <summary>
        /// The duration of time (in ticks) that this animation plays over.
        /// </summary>
        public TimeSpan Duration {
            get {
                return _duration;
            }
            set {
                _duration = value;
            }
        }

        /// <summary>
        /// The interpolation used to progress the animation from start to end.
        /// </summary>
        public TweenInterpolation Interpolation {
            get {
                return _interpolation;
            }
            set {
                _interpolation = value;
            }
        }

        /// <summary>
        /// Gets or sets the delay for starting the animation.
        /// </summary>
        public TimeSpan StartDelay {
            get {
                return _startDelay;
            }
            set {
                _startDelay = value;
            }
        }

        /// <internalonly />
        protected internal override void Repeat(bool reverse) {
            base.Repeat(reverse);
            if (reverse == false) {
                _isStarting = true;
            }
        }

        /// <summary>
        /// Allows the animation to implement its core tweening logic to generate
        /// intermediate frames as it progresses from start to end.
        /// </summary>
        /// <param name="frame">A value between 0 and 1 (inclusive) indicating the current frame.</param>
        protected abstract void PerformTweening(double frame);

        /// <internalonly />
        protected override sealed void PlayCore() {
            _startTimeStamp = DateTime.Now;
            _isStarting = true;
            ProgressCore(/* startRepetition */ false, /* startReverse */ false, _startTimeStamp);
        }

        /// <internalonly />
        protected override sealed bool ProgressCore(bool startRepetition, bool startReverse, DateTime timeStamp) {
            double frame = 0;
            bool completed = false;

            if (startRepetition == false) {
                if (_isStarting) {
                    if ((_startDelay.TotalMilliseconds != 0) &&
                        ((_startTimeStamp + _startDelay) > timeStamp)) {
                        return false;
                    }

                    _isStarting = false;
                    _startTimeStamp = timeStamp;
                }

                frame = (timeStamp - _startTimeStamp).TotalMilliseconds / _duration.TotalMilliseconds;
                if (IsReversed == false) {
                    completed = (frame >= 1);
                    frame = Math.Min(1, frame);
                }
                else {
                    frame = 1 - frame;

                    completed = (frame <= 0);
                    frame = Math.Max(0f, frame);
                }

                if ((completed == false) && (_interpolation != null)) {
                    frame = _interpolation.Interpolate(frame);
                }
            }
            else {
                _startTimeStamp = timeStamp;
                if (IsReversed) {
                    frame = 1;
                }
            }

            PerformTweening(frame);
            return completed;
        }

        /// <internalonly />
        protected override sealed void StopCore(bool completed, ProceduralAnimationStopState stopState) {
            if (completed == false) {
                if (stopState == ProceduralAnimationStopState.Complete) {
                    PerformTweening(1f);
                }
                else if (stopState == ProceduralAnimationStopState.Revert) {
                    PerformTweening(0f);
                }
            }
        }
    }
}
