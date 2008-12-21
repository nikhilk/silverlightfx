// TilePanel.cs
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

// TODO: Add Orientation - current implementation is Horizontal.

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A panel that arranges its child elements horizontally, and then wrapped to the next
    /// row in an animated manner.
    /// </summary>
    public class TilePanel : AnimatedPanel {

        /// <summary>
        /// Represents the TileHeight property of the TilePanel.
        /// </summary>
        public static readonly DependencyProperty TileHeightProperty =
            DependencyProperty.Register("TileHeight", typeof(double), typeof(TilePanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Represents the TileSpacing property of the TilePanel.
        /// </summary>
        public static readonly DependencyProperty TileSpacingProperty =
            DependencyProperty.Register("TileSpacing", typeof(double), typeof(TilePanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Represents the TileWidth property of the TilePanel.
        /// </summary>
        public static readonly DependencyProperty TileWidthProperty =
            DependencyProperty.Register("TileWidth", typeof(double), typeof(TilePanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// The height of each tile in the panel.
        /// </summary>
        public double TileHeight {
            get {
                return (double)GetValue(TileHeightProperty);
            }
            set {
                SetValue(TileHeightProperty, value);
            }
        }

        /// <summary>
        /// The spacing between each tile in the panel.
        /// </summary>
        public double TileSpacing {
            get {
                return (double)GetValue(TileSpacingProperty);
            }
            set {
                SetValue(TileSpacingProperty, value);
            }
        }

        /// <summary>
        /// The width of each tile in the panel.
        /// </summary>
        public double TileWidth {
            get {
                return (double)GetValue(TileWidthProperty);
            }
            set {
                SetValue(TileWidthProperty, value);
            }
        }

        /// <internalonly />
        protected override Size ArrangeOverride(Size finalSize) {
            if (Children.Count != 0) {
                double top = 0;
                double left = 0;
                double width = TileWidth;
                double height = TileHeight;
                double spacing = TileSpacing;
                bool first = true;

                BeginArrange();
                foreach (UIElement element in Children) {
                    if (element.Visibility != Visibility.Collapsed) {
                        if (first == false) {
                            left += spacing;
                        }
                        ArrangeElement(element, new Rect(left, top, width, height));
                        left = left + width;

                        if ((left + spacing + width) > finalSize.Width) {
                            left = 0;
                            top += height + spacing;
                            first = true;
                        }
                        else {
                            first = false;
                        }
                    }
                }
                EndArrange();
            }

            return finalSize;
        }

        /// <internalonly />
        protected override Size MeasureOverride(Size availableSize) {
            if (Children.Count == 0) {
                return new Size(0, 0);
            }

            Size s = new Size(TileWidth, TileHeight);
            foreach (UIElement element in Children) {
                element.Measure(s);
            }

            double top = 0;
            double left = 0;
            double width = TileWidth;
            double height = TileHeight;
            double spacing = TileSpacing;

            bool first = true;
            foreach (UIElement element in Children) {
                if (element.Visibility != Visibility.Collapsed) {
                    if (first == false) {
                        left += spacing;
                    }

                    left += width;

                    if ((left + spacing + width) > availableSize.Width) {
                        left = 0;
                        top += height + spacing;
                        first = true;
                    }
                    else {
                        first = false;
                    }
                }
            }

            if (left == 0) {
                return new Size(availableSize.Width, top);
            }

            return new Size(availableSize.Width, top + height);
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((TilePanel)d).InvalidateMeasure();
        }
    }
}
