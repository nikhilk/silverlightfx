// FocusEffect.cs
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
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Plays an effect whenever the associated element gets or loses
    /// focus.
    /// </summary>
    public class FocusEffect : EffectBehavior {

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;

            base.OnDetach();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e) {
            PlayEffect(EffectDirection.Forward);
        }

        private void OnLostFocus(object sender, RoutedEventArgs e) {
            PlayEffect(EffectDirection.Reverse);
        }
    }
}
