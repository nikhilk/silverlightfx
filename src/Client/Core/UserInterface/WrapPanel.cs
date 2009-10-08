// WrapPanel.cs
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
using System.ComponentModel;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A panel that positions child elements sequentially from left to right or top to
    /// bottom.  When elements extend beyond the panel edge, elements are
    /// positioned in the next row or column.
    /// </summary>
    public class WrapPanel : AnimatedPanel {

        /// <summary>
        /// Represents the ItemHeight property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(WrapPanel),
                                        new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

        /// <summary>
        /// Represents the ItemWidth property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(WrapPanel),
                                        new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

        /// <summary>
        /// Represents the Orientation property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel),
                                        new PropertyMetadata(Orientation.Horizontal, OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the height of the layout area for each contained child.
        /// </summary>
        public double ItemHeight {
            get {
                return (double)GetValue(ItemHeightProperty);
            }
            set {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the layout area for each contained child.
        /// </summary>
        public double ItemWidth {
            get {
                return (double)GetValue(ItemWidthProperty);
            }
            set {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the direction in which child elements are arranged.
        /// </summary>
        public Orientation Orientation {
            get {
                return (Orientation)GetValue(OrientationProperty);
            }
            set {
                SetValue(OrientationProperty, value);
            }
        }

        /// <internalonly />
        protected override Size ArrangeOverride(Size finalSize) {
            // Variables tracking the size of the current line, and the maximum
            // size available to fill.  Note that the line might represent a row
            // or a column depending on the orientation.
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, finalSize.Width, finalSize.Height);

            // Determine the constraints for individual items
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = Double.IsNaN(itemWidth) == false;
            bool hasFixedHeight = Double.IsNaN(itemHeight) == false;
            double indirectOffset = 0;
            double? directDelta = (o == Orientation.Horizontal) ?
                                    (hasFixedWidth ? (double?)itemWidth : null) :
                                    (hasFixedHeight ? (double?)itemHeight : null);

            BeginArrange();

            // Measure each of the Children.  We will process the elements one
            // line at a time, just like during measure, but we will wait until
            // we've completed an entire line of elements before arranging them.
            // The lineStart and lineEnd variables track the size of the
            // currently arranged line.
            UIElementCollection children = Children;
            int count = children.Count;
            int lineStart = 0;
            for (int lineEnd = 0; lineEnd < count; lineEnd++) {
                UIElement element = children[lineEnd];

                // Get the size of the element
                OrientedSize elementSize =
                    new OrientedSize(o,
                                     hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                                     hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct)) {
                    // Then we just completed a line and we should arrange it
                    ArrangeLine(lineStart, lineEnd, directDelta, indirectOffset, lineSize.Indirect);

                    // Move the current element to a new line
                    indirectOffset += lineSize.Indirect;
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size
                    if (IsGreaterThan(elementSize.Direct, maximumSize.Direct)) {
                        // Arrange the element as a single line
                        ArrangeLine(lineEnd, ++lineEnd, directDelta, indirectOffset, elementSize.Indirect);

                        // Move to a new line
                        indirectOffset += lineSize.Indirect;
                        lineSize = new OrientedSize(o);
                    }

                    // Advance the start index to a new line after arranging
                    lineStart = lineEnd;
                }
                else {
                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Arrange any elements on the last line
            if (lineStart < count) {
                ArrangeLine(lineStart, count, directDelta, indirectOffset, lineSize.Indirect);
            }

            EndArrange();

            return finalSize;
        }

        private void ArrangeLine(int lineStart, int lineEnd, double? directDelta, double indirectOffset, double indirectGrowth) {
            double directOffset = 0.0;

            Orientation o = Orientation;
            bool isHorizontal = o == Orientation.Horizontal;

            UIElementCollection children = Children;
            for (int index = lineStart; index < lineEnd; index++) {
                // Get the size of the element
                UIElement element = children[index];
                OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                // Determine if we should use the element's desired size or the
                // fixed item width or height
                double directGrowth = directDelta != null ? directDelta.Value : elementSize.Direct;

                // Arrange the element
                Rect bounds = isHorizontal ?
                    new Rect(directOffset, indirectOffset, directGrowth, indirectGrowth) :
                    new Rect(indirectOffset, directOffset, indirectGrowth, directGrowth);
                ArrangeElement(element, bounds);

                directOffset += directGrowth;
            }
        }

        /// <internalonly />
        protected override Size MeasureOverride(Size constraint) {
            // Variables tracking the size of the current line, the total size
            // measured so far, and the maximum size available to fill.  Note
            // that the line might represent a row or a column depending on the
            // orientation.
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize totalSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, constraint.Width, constraint.Height);

            // Determine the constraints for individual items
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = Double.IsNaN(itemWidth) == false;
            bool hasFixedHeight = Double.IsNaN(itemHeight) == false;
            Size itemSize = new Size(hasFixedWidth ? itemWidth : constraint.Width,
                                     hasFixedHeight ? itemHeight : constraint.Height);

            // Measure each of the Children
            foreach (UIElement element in Children) {
                // Determine the size of the element
                element.Measure(itemSize);
                OrientedSize elementSize =
                    new OrientedSize(o,
                                     hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                                     hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct)) {
                    // Update the total size with the direct and indirect growth
                    // for the current line
                    totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
                    totalSize.Indirect += lineSize.Indirect;

                    // Move the element to a new line
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size,
                    // place it on a line by itself
                    if (IsGreaterThan(elementSize.Direct, maximumSize.Direct)) {
                        // Update the total size for the line occupied by this
                        // single element
                        totalSize.Direct = Math.Max(elementSize.Direct, totalSize.Direct);
                        totalSize.Indirect += elementSize.Indirect;

                        // Move to a new line
                        lineSize = new OrientedSize(o);
                    }
                }
                else {
                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Update the total size with the elements on the last line
            totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            totalSize.Indirect += lineSize.Indirect;

            // Return the total size required as an un-oriented quantity
            return new Size(totalSize.Width, totalSize.Height);
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            WrapPanel panel = (WrapPanel)d;
            panel.InvalidateMeasure();
        }

        private static bool IsGreaterThan(double left, double right) {
            return (left > right) && !AreClose(left, right);
        }

        private static bool AreClose(double left, double right) {
            if (left == right) {
                return true;
            }

            double a = (Math.Abs(left) + Math.Abs(right) + 10.0) * 2.2204460492503131E-16;
            double b = left - right;
            return (-a < b) && (a > b);
        }

        private struct OrientedSize {

            private Orientation _orientation;
            private double _direct;
            private double _indirect;

            public OrientedSize(Orientation orientation) :
                this(orientation, 0.0, 0.0) {
            }

            public OrientedSize(Orientation orientation, double width, double height) {
                _orientation = orientation;
                _direct = 0.0;
                _indirect = 0.0;

                Width = width;
                Height = height;
            }

            public double Direct {
                get {
                    return _direct;
                }
                set {
                    _direct = value;
                }
            }

            public double Indirect {
                get {
                    return _indirect;
                }
                set {
                    _indirect = value;
                }
            }

            public Orientation Orientation {
                get {
                    return _orientation;
                }
            }

            public double Width {
                get {
                    return (Orientation == Orientation.Horizontal) ? Direct : Indirect;
                }
                set {
                    if (Orientation == Orientation.Horizontal) {
                        Direct = value;
                    }
                    else {
                        Indirect = value;
                    }
                }
            }

            public double Height {
                get {
                    return (Orientation != Orientation.Horizontal) ? Direct : Indirect;
                }
                set {
                    if (Orientation != Orientation.Horizontal) {
                        Direct = value;
                    }
                    else {
                        Indirect = value;
                    }
                }
            }
        }
    }
}
