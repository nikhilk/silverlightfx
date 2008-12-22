// HoverEffect.cs
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Plays an effect whenever the mouse enters or leaves the
    /// associated element.
    /// </summary>
    public class HoverEffect : EffectBehavior {

        /// <internalonly />
        protected override void OnAttach() {
            base.OnAttach();

            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        /// <internalonly />
        protected override void OnDetach() {
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseLeave -= OnMouseLeave;

            base.OnDetach();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e) {
            PlayEffect(EffectDirection.Forward);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e) {
            PlayEffect(EffectDirection.Reverse);
        }
    }
}
