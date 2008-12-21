// VStackPanel.cs
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
    /// A panel that arranges its children vertically.
    /// </summary>
    public class VStackPanel : AnimatedPanel {

        /// <summary>
        /// Represents the ChildAlignment property of HStackPanel
        /// </summary>
        public static readonly DependencyProperty ChildAlignmentProperty =
            DependencyProperty.Register("ChildAlignment", typeof(HorizontalAlignment), typeof(VStackPanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Represents the ChildFlow property of HStackPanel
        /// </summary>
        public static readonly DependencyProperty ChildFlowProperty =
            DependencyProperty.Register("ChildFlow", typeof(VerticalFlow), typeof(VStackPanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Represents the ChildSpacing property of HStackPanel
        /// </summary>
        public static readonly DependencyProperty ChildSpacingProperty =
            DependencyProperty.Register("ChildSpacing", typeof(double), typeof(VStackPanel),
                                        new PropertyMetadata(OnLayoutChanged));

        /// <summary>
        /// Gets or sets the vertical alignment of children within the panel.
        /// </summary>
        public HorizontalAlignment ChildAlignment {
            get {
                object o = GetValue(ChildAlignmentProperty);
                return (o != null) ? (HorizontalAlignment)o : HorizontalAlignment.Stretch;
            }
            set {
                SetValue(ChildAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical flow of children within the panel.
        /// </summary>
        public VerticalFlow ChildFlow {
            get {
                object o = GetValue(ChildFlowProperty);
                return (o != null) ? (VerticalFlow)o : VerticalFlow.Top;
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
            double previousChildHeight = 0;

            double childSpacing = ChildSpacing;
            HorizontalAlignment alignment = ChildAlignment;

            switch (ChildFlow) {
                case VerticalFlow.Center:
                    childRect.Y = (finalSize.Height - DesiredSize.Height) / 2;
                    break;
                case VerticalFlow.Bottom:
                    childRect.Y = finalSize.Height - DesiredSize.Height;
                    break;
            }

            bool firstChild = true;

            BeginArrange();
            foreach (UIElement element in Children) {
                if (element.Visibility != Visibility.Collapsed) {
                    childRect.Y += previousChildHeight;
                    if (firstChild == false) {
                        childRect.Y += childSpacing;
                    }

                    previousChildHeight = childRect.Height = element.DesiredSize.Height;

                    switch (alignment) {
                        case HorizontalAlignment.Stretch:
                            childRect.Width = Math.Max(finalSize.Width, element.DesiredSize.Width);
                            break;
                        case HorizontalAlignment.Left:
                            childRect.Width = Math.Min(finalSize.Width, element.DesiredSize.Width);
                            break;
                        case HorizontalAlignment.Right:
                            childRect.Width = Math.Min(finalSize.Width, element.DesiredSize.Width);
                            childRect.X = finalSize.Height - childRect.Height;
                            break;
                        case HorizontalAlignment.Center:
                            childRect.Width = Math.Min(finalSize.Width, element.DesiredSize.Width);
                            childRect.X = (finalSize.Width - childRect.Width) / 2;
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
            Size childAvailableSize = new Size(availableSize.Width, Double.PositiveInfinity);
            Size desiredSize = new Size(0, 0);

            double childSpacing = ChildSpacing;
            bool firstChild = true;

            foreach (UIElement element in Children) {
                element.Measure(childAvailableSize);

                if (element.Visibility != Visibility.Collapsed) {
                    if (firstChild == false) {
                        desiredSize.Height += childSpacing;
                    }

                    desiredSize.Height += element.DesiredSize.Height;
                    desiredSize.Width = Math.Max(desiredSize.Width, element.DesiredSize.Width);

                    firstChild = false;
                }
            }

            return desiredSize;
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((VStackPanel)d).InvalidateMeasure();
        }
    }
}
