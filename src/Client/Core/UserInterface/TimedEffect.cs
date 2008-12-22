// TimedEffect.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Plays an effect whenever the associated element is
    /// loaded.
    /// </summary>
    public class TimedEffect : EffectBehavior {

        private bool _forward;
        private DispatcherTimer _dispatcherTimer;

        /// <summary>
        /// Initializes an instance of a TimedEffect.
        /// </summary>
        public TimedEffect() {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += OnTick;
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Gets or sets the tick interval of the timer.
        /// </summary>
        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan Interval {
            get {
                return _dispatcherTimer.Interval;
            }
            set {
                _dispatcherTimer.Interval = value;
            }
        }

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            AssociatedObject.Loaded += OnLoaded;
        }

        /// <internalonly />
        protected override void OnDetach() {
            _dispatcherTimer.Stop();

            AssociatedObject.Loaded -= OnLoaded;

            base.OnDetach();
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            _forward = true;
            PlayEffect(EffectDirection.Forward);

            _dispatcherTimer.Start();
        }

        private void OnTick(object sender, EventArgs e) {
            _forward = !_forward;
            PlayEffect(_forward ? EffectDirection.Forward : EffectDirection.Reverse);
        }
    }
}
