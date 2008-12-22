// ProceduralAnimation.cs
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

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// The base class for procedural animations.
    /// </summary>
    public abstract class ProceduralAnimation : IDisposable {

        private int _repeatCount;
        private TimeSpan _repeatDelay;
        private bool _autoReverse;
        private TimeSpan _reverseDelay;

        private bool _completed;
        private bool _isPlaying;
        private bool _reversed;

        private bool _isReversing;
        private bool _isRepeating;
        private int _repetitions;
        private DateTime _repeatTimeStamp;

        private ProceduralAnimationController _controller;

        /// <summary>
        /// Raised before the animation is repeated. This event can
        /// be used to cancel further repeatitions.
        /// </summary>
        public event CancelEventHandler Repeating;

        /// <summary>
        /// Raised before the animation is started and performs
        /// any setup work.
        /// </summary>
        public event EventHandler Starting;

        /// <summary>
        /// Raised after the animation is stopped or completed,
        /// and any cleanup work has been performed.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Initializes an instance of an Animation class.
        /// </summary>
        protected ProceduralAnimation() {
            _repeatCount = 1;
        }

        /// <summary>
        /// Gets the element associated with the animation while the animation
        /// is playing.
        /// </summary>
        public FrameworkElement AssociatedElement {
            get {
                if (_controller != null) {
                    return _controller.Element;
                }

                return null;
            }
        }

        /// <summary>
        /// Whether an animation automatically plays in the reverse direction
        /// upon repeating.
        /// </summary>
        public bool AutoReverse {
            get {
                return _autoReverse;
            }
            set {
                _autoReverse = value;
            }
        }

        internal ProceduralAnimationController Controller {
            get {
                return _controller;
            }
        }

        /// <summary>
        /// True if the animation has completely finished playing without
        /// being stopped mid-way.
        /// </summary>
        public bool Completed {
            get {
                return _completed;
            }
        }

        /// <summary>
        /// Whether the animation is currently playing or not.
        /// </summary>
        public bool IsPlaying {
            get {
                return _isPlaying;
            }
        }

        /// <summary>
        /// Whether the animation is currently playing in the reverse direction.
        /// </summary>
        protected bool IsReversed {
            get {
                return _reversed;
            }
        }

        /// <summary>
        /// Whether the animation repeats or plays a single time. 0 implies
        /// implies endless repetition, and other positive values indicate a fixed
        /// number of repetitions. The default value is 1.
        /// </summary>
        public int RepeatCount {
            get {
                return _repeatCount;
            }
            set {
                _repeatCount = value;
            }
        }

        /// <summary>
        /// The number of milliseconds to delay the animation between successive
        /// repetitions.
        /// </summary>
        public TimeSpan RepeatDelay {
            get {
                return _repeatDelay;
            }
            set {
                _repeatDelay = value;
            }
        }

        /// <summary>
        /// Returns the number of repeatitions of the animation completed so far.
        /// </summary>
        public int Repetitions {
            get {
                return _repetitions;
            }
        }

        /// <summary>
        /// The number of milliseconds to delay the animation between reversal.
        /// </summary>
        public TimeSpan ReverseDelay {
            get {
                return _reverseDelay;
            }
            set {
                _reverseDelay = value;
            }
        }

        /// <summary>
        /// Disposes the animation instance.
        /// </summary>
        public virtual void Dispose() {
            if (_isPlaying) {
                Stop(ProceduralAnimationStopState.Abort);
            }
        }

        internal void OnPlay(bool reversed) {
            if (Starting != null) {
                Starting(this, EventArgs.Empty);
            }
            PerformSetup();

            _isPlaying = true;
            _repetitions = 1;
            _reversed = reversed;
            PlayCore();
        }

        internal void OnStop(bool completed, ProceduralAnimationStopState stopState) {
            StopCore(completed, stopState);

            _completed = completed;
            _isPlaying = false;

            PerformCleanup();
            if (Stopped != null) {
                Stopped(this, EventArgs.Empty);
            }
        }

        internal bool OnProgress(DateTime timeStamp) {
            if (_isRepeating) {
                if (_isReversing) {
                    if ((_reverseDelay.TotalMilliseconds != 0) &&
                        ((_repeatTimeStamp + _reverseDelay) > timeStamp)) {
                        return false;
                    }
                }
                else {
                    if ((_repeatDelay.TotalMilliseconds != 0) &&
                        ((_repeatTimeStamp + _repeatDelay) > timeStamp)) {
                        return false;
                    }
                }
            }

            bool completed = ProgressCore(_isRepeating, _isReversing, timeStamp);
            _isRepeating = false;
            _isReversing = false;

            if (completed && _autoReverse) {
                if (_reversed == false) {
                    Repeat(/* reverse */ true);
                    _repeatTimeStamp = timeStamp;

                    completed = false;
                    return completed;
                }
                else {
                    _reversed = false;
                }
            }
            if (completed &&
                ((_repeatCount == 0) || (_repeatCount > _repetitions))) {
                completed = false;

                _repetitions++;
                if (Repeating != null) {
                    CancelEventArgs ce = new CancelEventArgs();
                    Repeating(this, ce);

                    completed = ce.Canceled;
                }

                if (completed == false) {
                    Repeat(/* reverse */ false);
                    _repeatTimeStamp = timeStamp;
                }
            }

            return completed;
        }

        /// <summary>
        /// Allows the animation to perform any cleanup work once the
        /// animation is complete.
        /// </summary>
        protected virtual void PerformCleanup() {
        }

        /// <summary>
        /// Allows the animation to perform any setup work before the
        /// animation is started.
        /// </summary>
        protected virtual void PerformSetup() {
        }

        /// <summary>
        /// Schedules the animation to be played.
        /// </summary>
        /// <param name="associatedElement">The element to use to control the animation.</param>
        public void Play(FrameworkElement associatedElement) {
            _completed = false;
            _controller = ProceduralAnimationController.Play(this, associatedElement);
        }

        /// <summary>
        /// Plays the animation when it has been scheduled and started.
        /// </summary>
        protected abstract void PlayCore();

        /// <summary>
        /// Progresses the animation to the new current time.
        /// </summary>
        /// <param name="startRepetition">Whether the animation is starting a repetition.</param>
        /// <param name="startReverse">Whether the animation is starting a reverse run.</param>
        /// <param name="timeStamp">The time stamp to progress the animation through.</param>
        /// <returns>Whether the animation has completed.</returns>
        protected abstract bool ProgressCore(bool startRepetition, bool startReverse, DateTime timeStamp);

        /// <summary>
        /// Indicates the animation is being repeated.
        /// </summary>
        /// <param name="reverse">Whether the next repetition will happen in reverse direction.</param>
        protected internal virtual void Repeat(bool reverse) {
            _isRepeating = true;
            _isReversing = reverse;
            _reversed = reverse;
            _isPlaying = true;
        }

        /// <summary>
        /// Stops playing the animation mid-way. The specified stopState determines
        /// the state in which the element being animated is left in. 
        /// </summary>
        /// <param name="stopState">The state of the element upon stopping the animation.</param>
        public void Stop(ProceduralAnimationStopState stopState) {
            ProceduralAnimationController.Stop(this, stopState);
            _controller = null;
        }

        /// <summary>
        /// Stops the animation when it is no longer scheduled to continue playing.
        /// </summary>
        /// <param name="completed">Whether the animation has completed naturally.</param>
        /// <param name="stopState">The state in which the animation should end if it was interrupted.</param>
        protected abstract void StopCore(bool completed, ProceduralAnimationStopState stopState);
    }
}
