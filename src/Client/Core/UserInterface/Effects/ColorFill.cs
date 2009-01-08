// ColorFill.cs
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
    /// Represents a colored background fill effect.
    /// </summary>
    public class ColorFill : Effect {

        private Color _fillColor;
        private Color _initialColor;
        private SolidColorBrush _brush;

        /// <summary>
        /// Initializes an instance of an ColorFillEffect.
        /// </summary>
        public ColorFill() {
            _fillColor = Color.FromArgb(0, 0, 0, 0);
        }

        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        public Color FillColor {
            get {
                return _fillColor;
            }
            set {
                _fillColor = value;
            }
        }

        /// <internalonly />
        protected internal override ProceduralAnimation CreateEffectAnimation(EffectDirection direction) {
            if (_brush == null) {
                FrameworkElement target = GetTarget();

                Border border = target as Border;
                Panel panel = target as Panel;
                Control control = target as Control;

                if ((border == null) && (panel == null) && (control == null)) {
                    throw new InvalidOperationException("The target of the ColorFillEffect must be a Panel, Border or Control.");
                }

                if (border != null) {
                    _brush = border.Background as SolidColorBrush;
                }
                else if (panel != null) {
                    _brush = panel.Background as SolidColorBrush;
                }
                else if (control != null) {
                    _brush = control.Background as SolidColorBrush;
                }

                if (_brush == null) {
                    _brush = new SolidColorBrush(Color.FromArgb(0, _fillColor.R, _fillColor.G, _fillColor.B));
                    if (border != null) {
                        border.Background = _brush;
                    }
                    else if (panel != null) {
                        panel.Background = _brush;
                    }
                    else if (control != null) {
                        control.Background = _brush;
                    }
                }

                _initialColor = _brush.Color;
            }

            ColorFillAnimation animation;
            if (direction == EffectDirection.Forward) {
                animation = new ColorFillAnimation(Duration, _brush, _initialColor, _fillColor);
            }
            else {
                animation = new ColorFillAnimation(Duration, _brush, _fillColor, _initialColor);
            }

            animation.AutoReverse = AutoReverse;
            animation.Interpolation = GetEffectiveInterpolation();

            return animation;
        }


        private sealed class ColorFillAnimation : TweenAnimation {

            private SolidColorBrush _brush;
            private Color _fillColor;
            private Color _initialColor;

            public ColorFillAnimation(TimeSpan duration, SolidColorBrush brush, Color initialColor, Color fillColor)
                : base(duration) {
                _brush = brush;
                _initialColor = initialColor;
                _fillColor = fillColor;
            }

            protected override void PerformTweening(double frame) {
                byte alpha = (byte)(_initialColor.A + (_fillColor.A - _initialColor.A) * frame);
                byte red = (byte)(_initialColor.R + (_fillColor.R - _initialColor.R) * frame);
                byte green = (byte)(_initialColor.G + (_fillColor.G - _initialColor.G) * frame);
                byte blue = (byte)(_initialColor.B + (_fillColor.B - _initialColor.B) * frame);

                _brush.Color = Color.FromArgb(alpha, red, green, blue);
            }
        }
    }
}
