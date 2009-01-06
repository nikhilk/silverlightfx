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
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Glitz;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// Represents an effect that can be associated with an element.
    /// </summary>
    public abstract class Effect : FrameworkElement, IAttachedObject, IProceduralAnimationFactory {

        private FrameworkElement _associatedObject;
        private FrameworkElement _target;
        private string _targetName;
        private TimeSpan _duration;
        private bool _autoReverse;
        private bool _reversible;
        private bool _reversed;
        private bool _useDefaultInterpolation;
        private TweenInterpolation _interpolation;

        private ProceduralAnimation _animation;
        private EffectDirection _direction;

        /// <summary>
        /// Initializes an Effect instance.
        /// </summary>
        protected Effect() {
            _duration = TimeSpan.FromMilliseconds(250);
            _reversible = true;
            _useDefaultInterpolation = true;
        }

        /// <summary>
        /// Gets the object that this Effect is associated with.
        /// </summary>
        protected FrameworkElement AssociatedObject {
            get {
                return _associatedObject;
            }
        }

        /// <summary>
        /// Whether to automatically play the reverse effect upon completing
        /// the forward effect in response to a single event.
        /// </summary>
        public bool AutoReverse {
            get {
                return _autoReverse;
            }
            set {
                _autoReverse = value;
            }
        }

        /// <summary>
        /// Gets the direction that the effect is currently playing while it is
        /// active or has just completed.
        /// </summary>
        public EffectDirection Direction {
            get {
                return _direction;
            }
        }

        /// <summary>
        /// Gets or sets the duration over which the effect is played.
        /// </summary>
        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan Duration {
            get {
                return _duration;
            }
            set {
                _duration = value;
            }
        }

        /// <summary>
        /// Gets or sets how the animation progress is interpolated over time.
        /// The default is to smoothly interpolate from start to finish, which
        /// equates to basic easing in and easing out.
        /// </summary>
        public TweenInterpolation Interpolation {
            get {
                return _interpolation;
            }
            set {
                _interpolation = value;
                _useDefaultInterpolation = false;
            }
        }

        /// <summary>
        /// Gets whether the effect is currently active, i.e. the associated animation
        /// is currently playing.
        /// </summary>
        public bool IsActive {
            get {
                return (_animation != null) && _animation.IsPlaying;
            }
        }

        /// <summary>
        /// Gets or sets whether the effect is logically reversed, i.e. it plays the
        /// reverse effect on being triggered to play in the forward direction and vice versa.
        /// </summary>
        public bool Reversed {
            get {
                return _reversed;
            }
            set {
                _reversed = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the effect is allowed to play in reverse direction.
        /// If this is set to false, the effect only plays in forward direction, and uses
        /// the same animation when triggered to play in reverse direction.
        /// </summary>
        public bool Reversible {
            get {
                return _reversible;
            }
            set {
                _reversible = value;
            }
        }

        /// <summary>
        /// The object on which the effect is applied. By default
        /// this is the object that the effect is associated with.
        /// </summary>
        public FrameworkElement Target {
            get {
                return _target;
            }
            set {
                _target = value;
            }
        }

        /// <summary>
        /// The name of the object on which the effect is applied. By default
        /// this is the object that the effect is associated with
        /// </summary>
        public string TargetName {
            get {
                return _targetName;
            }
            set {
                _targetName = value;
            }
        }

        /// <summary>
        /// Indicates that an effect has been completed. This event is only raised
        /// when the effect completes on its own, rather than implicitly completed
        /// as a result of restarting the effect.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Gets the effective interpolation to use for animations created by the
        /// effect.
        /// </summary>
        /// <returns>The effective interpolation; null if there is no specific interpolation.</returns>
        protected TweenInterpolation GetEffectiveInterpolation() {
            if (_useDefaultInterpolation) {
                return EasingInterpolation.Default;
            }

            if ((_interpolation != null) && (_interpolation.IsLinearInterpolation == false)) {
                return _interpolation;
            }

            return null;
        }

        /// <summary>
        /// Gets the target of the effect. This resolves the TargetName
        /// property value into an actual object if one has been specified.
        /// </summary>
        /// <returns>The target of the effect.</returns>
        protected FrameworkElement GetTarget() {
            if (_target != null) {
                return _target;
            }

            if (String.IsNullOrEmpty(_targetName)) {
                return AssociatedObject;
            }

            FrameworkElement element = AssociatedObject.FindName(_targetName) as FrameworkElement;
            if (element == null) {
                throw new InvalidOperationException("The specified target '" + _targetName + "' could not be found.");
            }

            return element;
        }

        /// <internalonly />
        protected virtual void OnAttach() {
        }

        /// <internalonly />
        protected virtual void OnDetach() {
        }

        /// <summary>
        /// Creates the animation representing the effect.
        /// </summary>
        /// <param name="direction">The direction of the animation.</param>
        /// <returns>The animation to be played to play the effect.</returns>
        protected internal abstract ProceduralAnimation CreateEffectAnimation(EffectDirection direction);

        private void OnAnimationStopped(object sender, EventArgs e) {
            OnCompleted();
        }

        /// <summary>
        /// Indicates that the effect has completed playing.
        /// </summary>
        protected virtual void OnCompleted() {
            if (Completed != null) {
                Completed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Indicates that the effect is about to be played.
        /// </summary>
        protected virtual void OnStarting() {
        }

        /// <summary>
        /// Indicates that the effect should start playing.
        /// </summary>
        /// <param name="direction">Indicates whether to play the direction in forward direction or reverse direction.</param>
        public void PlayEffect(EffectDirection direction) {
            if (_animation != null) {
                _animation.Stopped -= OnAnimationStopped;
                if (_animation.IsPlaying) {
                    _animation.Stop(ProceduralAnimationStopState.Complete);
                }
                _animation = null;
            }

            if ((direction == EffectDirection.Reverse) && (AutoReverse || (Reversible == false))) {
                direction = EffectDirection.Forward;
            }
            if (_reversed) {
                direction = (direction == EffectDirection.Forward) ? EffectDirection.Reverse : EffectDirection.Forward;
            }

            _animation = CreateEffectAnimation(direction);
            if (_animation != null) {
                _direction = direction;
                _animation.Stopped += OnAnimationStopped;

                OnStarting();
                _animation.Play(AssociatedObject);
            }
        }

        /// <summary>
        /// Indicates that the effect should complete itself now and stop.
        /// </summary>
        public void StopEffect() {
            if ((_animation != null) && _animation.IsPlaying) {
                _animation.Stop(ProceduralAnimationStopState.Complete);
            }
        }

        #region IAttachedObject Members
        DependencyObject IAttachedObject.AssociatedObject {
            get {
                return _associatedObject;
            }
        }

        void IAttachedObject.Attach(DependencyObject associatedObject) {
            _associatedObject = (FrameworkElement)associatedObject;
            OnAttach();
        }

        void IAttachedObject.Detach() {
            OnDetach();
            _associatedObject = null;
        }
        #endregion

        #region Implementation of IProceduralAnimationFactory
        ProceduralAnimation IProceduralAnimationFactory.CreateAnimation() {
            return CreateEffectAnimation(EffectDirection.Forward);
        }
        #endregion
    }
}
