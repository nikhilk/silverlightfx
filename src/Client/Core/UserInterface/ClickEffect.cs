// ClickEffect.cs
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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Glitz;
using System.Windows.Threading;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Plays an effect whenever the associated element is clicked.
    /// </summary>
    public class ClickEffect : EffectBehavior {

        private bool _forward;
        private DispatcherTimer _clickTimer;

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            ButtonBase button = AssociatedObject as ButtonBase;
            if (button != null) {
                button.Click += OnButtonClick;
            }
            else {
                AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
                AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e) {
            _forward = !_forward;
            PlayEffect(_forward ? EffectDirection.Forward : EffectDirection.Reverse);
        }

        private void OnClickTimerTick(object sender, EventArgs e) {
            StopClickTimer();
        }

        /// <internalonly />
        protected override void OnDetach() {
            ButtonBase button = AssociatedObject as ButtonBase;
            if (button != null) {
                button.Click -= OnButtonClick;
            }
            else {
                if (_clickTimer != null) {
                    StopClickTimer();
                }

                AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            }

            base.OnDetach();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            StartClickTimer();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (_clickTimer != null) {
                StopClickTimer();

                _forward = !_forward;
                PlayEffect(_forward ? EffectDirection.Forward : EffectDirection.Reverse);
            }
        }

        private void StartClickTimer() {
            _clickTimer = new DispatcherTimer();
            _clickTimer.Tick += OnClickTimerTick;
            _clickTimer.Interval = TimeSpan.FromMilliseconds(250);
            _clickTimer.Start();
        }

        private void StopClickTimer() {
            _clickTimer.Stop();
            _clickTimer = null;
        }
    }
}
