// DoubleClickTrigger.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A trigger that can be associated with an element for handling
    /// double click events.
    /// </summary>
    public sealed class DoubleClickTrigger : Trigger<UIElement> {

        private DispatcherTimer _timer;

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += OnTimerTick;

            AssociatedObject.MouseLeftButtonDown += OnElementMouseLeftButtonDown;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.MouseLeftButtonDown -= OnElementMouseLeftButtonDown;

            _timer.Stop();
            _timer = null;

            base.OnDetach();
        }

        private void OnElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (_timer.IsEnabled == false) {
                _timer.Start();
                return;
            }

            _timer.Stop();
            InvokeActions(EventArgs.Empty);
        }

        private void OnTimerTick(object sender, EventArgs e) {
            _timer.Stop();
        }
    }
}
