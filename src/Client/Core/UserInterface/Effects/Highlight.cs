// Highlight.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents a colored background highlight effect.
    /// </summary>
    public class Highlight : AnimationEffect {

        private Color _highlightColor;

        /// <summary>
        /// Initializes an instance of an HighlightEffect.
        /// </summary>
        public Highlight() {
            _highlightColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00);
        }

        /// <summary>
        /// Gets or sets the color of the highlight.
        /// </summary>
        public Color HighlightColor {
            get {
                return _highlightColor;
            }
            set {
                _highlightColor = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(AnimationEffectDirection direction) {
            SolidColorBrush brush;

            FrameworkElement target = GetTarget();

            Border border = target as Border;
            Panel panel = target as Panel;
            Control control = target as Control;

            if ((border == null) && (panel == null) && (control == null)) {
                throw new InvalidOperationException("The target of the effect must be a Panel, Border or Control.");
            }

            brush = new SolidColorBrush(_highlightColor);
            if (border != null) {
                border.Background = brush;
            }
            else if (panel != null) {
                panel.Background = brush;
            }
            else if (control != null) {
                control.Background = brush;
            }

            HighlightAnimation animation = new HighlightAnimation(Duration, brush, _highlightColor);
            animation.AutoReverse = AutoReverse;
            animation.Interpolation = GetEffectiveInterpolation();

            return animation;
        }


        private sealed class HighlightAnimation : TweenAnimation {

            private Color _highlightColor;
            private SolidColorBrush _brush;

            public HighlightAnimation(TimeSpan duration, SolidColorBrush brush, Color highlightColor)
                : base(duration) {
                _brush = brush;
                _highlightColor = highlightColor;
            }

            protected override void PerformTweening(double frame) {
                byte alpha = (byte)(Math.Sin(frame * Math.PI) * .75 * 255);
                _brush.Color = Color.FromArgb(alpha, _highlightColor.R, _highlightColor.G, _highlightColor.B);
            }
        }
    }
}
