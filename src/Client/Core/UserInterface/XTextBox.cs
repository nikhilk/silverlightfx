// XTextBox.cs
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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// An extended TextBox control with adornment capabilities.
    /// </summary>
    public class XTextBox : TextBox {

        private Panel _adornerLayer;
        private Panel _glyphPresenter;
        private ButtonBase _inplaceButton;

        /// <summary>
        /// Gets the button contained within a TextBox if there is one.
        /// </summary>
        public ButtonBase InPlaceButton {
            get {
                ApplyTemplate();
                return _inplaceButton;
            }
        }

        /// <summary>
        /// Whether this textbox can render adornments.
        /// </summary>
        public bool SupportsAdornments {
            get {
                ApplyTemplate();
                return _adornerLayer != null;
            }
        }

        /// <summary>
        /// Whether this textbox can render glyphs.
        /// </summary>
        public bool SupportsGlyphs {
            get {
                ApplyTemplate();
                return _adornerLayer != null;
            }
        }

        /// <summary>
        /// Adds the specified adornment to the textbox. An adornment is
        /// overlaid over the text in the textbox.
        /// </summary>
        /// <param name="adornment">The adornment to overlay.</param>
        public void AddAdornment(UIElement adornment) {
            if (_adornerLayer != null) {
                _adornerLayer.Children.Add(adornment);
                _adornerLayer.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Adds the specified glyph or icon to the textbox. A glyph is
        /// overlaid to the right of the text in the textbox.
        /// </summary>
        /// <param name="glyph">The glyph to add.</param>
        public void AddGlyph(UIElement glyph) {
            if (_glyphPresenter != null) {
                _glyphPresenter.Children.Add(glyph);
                _glyphPresenter.Visibility = Visibility.Visible;
            }
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _adornerLayer = GetTemplateChild("AdornerLayer") as Panel;
            _glyphPresenter = GetTemplateChild("GlyphPresenter") as Panel;
            _inplaceButton = GetTemplateChild("ButtonElement") as ButtonBase;
        }

        /// <summary>
        /// Removes the specified adornment from the textbox.
        /// </summary>
        /// <param name="adornment">The adornment to remove.</param>
        public void RemoveAdornment(UIElement adornment) {
            if (_adornerLayer != null) {
                _adornerLayer.Children.Remove(adornment);
                if (_adornerLayer.Children.Count == 0) {
                    _adornerLayer.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Removes the specified glyph from the textbox.
        /// </summary>
        /// <param name="glyph">The glyph to remove.</param>
        public void RemoveGlyph(UIElement glyph) {
            if (_glyphPresenter != null) {
                _glyphPresenter.Children.Remove(glyph);
                if (_glyphPresenter.Children.Count == 0) {
                    _glyphPresenter.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
