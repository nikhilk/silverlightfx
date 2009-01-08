// LoadEffect.cs
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
using System.Windows.Interactivity;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Plays an effect whenever the associated element is
    /// loaded.
    /// </summary>
    public class LoadEffect : EffectBehavior {

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            AssociatedObject.Loaded += OnLoaded;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.Loaded -= OnLoaded;

            base.OnDetach();
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            PlayEffect(EffectDirection.Forward);
        }
    }
}
