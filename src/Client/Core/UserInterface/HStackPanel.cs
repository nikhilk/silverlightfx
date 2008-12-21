// HStackPanel.cs
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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A panel that arranges its children horizontally.
    /// </summary>
    public class HStackPanel : AnimatedPanel {

        /// <summary>
        /// Represents the ChildAlignment property of HStackPanel
        /// </summary>
        public static readonly DependencyProperty ChildAlignmentProperty =
            DependencyProperty.Register("ChildAlignment", typeof(VerticalAlignment), typeof(HStackPanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Represents the ChildFlow property of HStackPanel
        /// </summary>
        public static readonly DependencyProperty ChildFlowProperty =
            DependencyProperty.Register("ChildFlow", typeof(HorizontalFlow), typeof(HStackPanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Represents the ChildSpacing property of HStackPanel
        /// </summary>
        public static readonly DependencyProperty ChildSpacingProperty =
            DependencyProperty.Register("ChildSpacing", typeof(double), typeof(HStackPanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Gets or sets the vertical alignment of children within the panel.
        /// </summary>
        public VerticalAlignment ChildAlignment {
            get {
                object o = GetValue(ChildAlignmentProperty);
                return (o != null) ? (VerticalAlignment)o : VerticalAlignment.Stretch;
            }
            set {
                SetValue(ChildAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal flow of children within the panel.
        /// </summary>
        public HorizontalFlow ChildFlow {
            get {
                object o = GetValue(ChildFlowProperty);
                return (o != null) ? (HorizontalFlow)o : HorizontalFlow.Left;
            }
            set {
                SetValue(ChildFlowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between children within the panel.
        /// </summary>
        public double ChildSpacing {
            get {
                object o = GetValue(ChildSpacingProperty);
                return (o != null) ? (double)o : 0;
            }
            set {
                SetValue(ChildSpacingProperty, value);
            }
        }

        /// <internalonly />
        protected override Size ArrangeOverride(Size finalSize) {
            Rect childRect = new Rect();
            double previousChildWidth = 0;

            double childSpacing = ChildSpacing;
            VerticalAlignment alignment = ChildAlignment;

            switch (ChildFlow) {
                case HorizontalFlow.Center:
                    childRect.X = (finalSize.Width - DesiredSize.Width) / 2;
                    break;
                case HorizontalFlow.Right:
                    childRect.X = finalSize.Width - DesiredSize.Width;
                    break;
            }

            bool firstChild = true;

            BeginArrange();
            foreach (UIElement element in Children) {
                if (element.Visibility != Visibility.Collapsed) {
                    childRect.X += previousChildWidth;
                    if (firstChild == false) {
                        childRect.X += childSpacing;
                    }

                    previousChildWidth = childRect.Width = element.DesiredSize.Width;

                    switch (alignment) {
                        case VerticalAlignment.Stretch:
                            childRect.Height = Math.Max(finalSize.Height, element.DesiredSize.Height);
                            break;
                        case VerticalAlignment.Top:
                            childRect.Height = Math.Min(finalSize.Height, element.DesiredSize.Height);
                            break;
                        case VerticalAlignment.Bottom:
                            childRect.Height = Math.Min(finalSize.Height, element.DesiredSize.Height);
                            childRect.Y = finalSize.Height - childRect.Height;
                            break;
                        case VerticalAlignment.Center:
                            childRect.Height = Math.Min(finalSize.Height, element.DesiredSize.Height);
                            childRect.Y = (finalSize.Height - childRect.Height) / 2;
                            break;
                    }

                    ArrangeElement(element, childRect);

                    firstChild = false;
                }
            }
            EndArrange();

            return finalSize;
        }

        /// <internalonly />
        protected override Size MeasureOverride(Size availableSize) {
            Size childAvailableSize = new Size(Double.PositiveInfinity, availableSize.Height);
            Size desiredSize = new Size(0, 0);

            double childSpacing = ChildSpacing;
            bool firstChild = true;

            foreach (UIElement element in Children) {
                element.Measure(childAvailableSize);

                if (element.Visibility != Visibility.Collapsed) {
                    if (firstChild == false) {
                        desiredSize.Width += childSpacing;
                    }

                    desiredSize.Width += element.DesiredSize.Width;
                    desiredSize.Height = Math.Max(desiredSize.Height, element.DesiredSize.Height);

                    firstChild = false;
                }
            }

            return desiredSize;
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((HStackPanel)d).InvalidateMeasure();
        }
    }
}
