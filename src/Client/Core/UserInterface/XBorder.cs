// XBorder.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A control providing border treatments to its contents.
    /// </summary>
    [TemplatePart(Name = "NineGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "Border", Type = typeof(Border))]
    [TemplatePart(Name = "TopLeftShadow", Type = typeof(RadialGradientBrush))]
    [TemplatePart(Name = "TopRightShadow", Type = typeof(RadialGradientBrush))]
    [TemplatePart(Name = "BottomLeftShadow", Type = typeof(RadialGradientBrush))]
    [TemplatePart(Name = "BottomRightShadow", Type = typeof(RadialGradientBrush))]
    [TemplatePart(Name = "LeftShadow", Type = typeof(LinearGradientBrush))]
    [TemplatePart(Name = "TopShadow", Type = typeof(LinearGradientBrush))]
    [TemplatePart(Name = "RightShadow", Type = typeof(LinearGradientBrush))]
    [TemplatePart(Name = "BottomShadow", Type = typeof(LinearGradientBrush))]
    [TemplatePart(Name = "BackgroundShadow", Type = typeof(SolidColorBrush))]
    [ContentProperty("Child")]
    public class XBorder : Control {

        /// <summary>
        /// Represents the Child property of a Border.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(Border),
                                        new PropertyMetadata(OnBorderChildPropertyChanged));

        /// <summary>
        /// Represents the ClipChild property of a Border.
        /// </summary>
        public static readonly DependencyProperty ClipChildProperty =
            DependencyProperty.Register("ClipChild", typeof(bool), typeof(Border), null);

        /// <summary>
        /// Represents the CornerRadius property of a Border.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(Border),
                                        new PropertyMetadata(OnCornerRadiusPropertyChanged));

        /// <summary>
        /// Represents the ShadowBrush property of a Border.
        /// </summary>
        public static readonly DependencyProperty ShadowBrushProperty =
            DependencyProperty.Register("ShadowBrush", typeof(LinearGradientBrush), typeof(Border),
                                        new PropertyMetadata(OnShadowBrushPropertyChanged));

        /// <summary>
        /// Represents the ShadowSpread property of a Border.
        /// </summary>
        public static readonly DependencyProperty ShadowSpreadProperty =
            DependencyProperty.Register("ShadowSpread", typeof(double), typeof(Border),
                                        new PropertyMetadata(OnShadowSpreadPropertyChanged));

        private Grid _nineGrid;
        private Border _border;
        private List<GradientBrush> _shadowGradients;
        private SolidColorBrush _shadowFill;

        /// <summary>
        /// Initializes an instance of a Border.
        /// </summary>
        public XBorder() {
            DefaultStyleKey = typeof(XBorder);
            ShadowSpread = 0;
            CornerRadius = 0;
            ClipChild = true;

            LayoutUpdated += OnLayoutUpdated;
        }

        /// <summary>
        /// Gets or sets the child property representing the element shown within
        /// a Border.
        /// </summary>
        public FrameworkElement Child {
            get {
                return (FrameworkElement)GetValue(ChildProperty);
            }
            set {
                SetValue(ChildProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the child is clipped when the border has a non-zero
        /// corner radius. The default is true, but can be set to false if the
        /// perf hit does not justify this.
        /// </summary>
        public bool ClipChild {
            get {
                return (bool)GetValue(ClipChildProperty);
            }
            set {
                SetValue(ClipChildProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius of the border.
        /// </summary>
        public double CornerRadius {
            get {
                return (double)GetValue(CornerRadiusProperty);
            }
            set {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush used to create a shadow around the child.
        /// </summary>
        public LinearGradientBrush ShadowBrush {
            get {
                return (LinearGradientBrush)GetValue(ShadowBrushProperty);
            }
            set {
                SetValue(ShadowBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spread or size of the shadow around the child.
        /// </summary>
        public double ShadowSpread {
            get {
                return (double)GetValue(ShadowSpreadProperty);
            }
            set {
                SetValue(ShadowSpreadProperty, value);
            }
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _nineGrid = GetTemplateChild("NineGrid") as Grid;
            _border = GetTemplateChild("Border") as Border;

            _shadowGradients = new List<GradientBrush>(8);
            _shadowGradients.Add((GradientBrush)GetTemplateChild("TopLeftShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("TopRightShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("BottomLeftShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("BottomRightShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("LeftShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("TopShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("RightShadow"));
            _shadowGradients.Add((GradientBrush)GetTemplateChild("BottomShadow"));
            _shadowFill = GetTemplateChild("BackgroundShadow") as SolidColorBrush;

            UpdateBorderChild(Child);
            UpdateBorderCornerRadius(CornerRadius);
            UpdateShadowBrush(ShadowBrush);
            UpdateShadowSpread(ShadowSpread);
        }

        private static void OnBorderChildPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((XBorder)o).UpdateBorderChild((UIElement)e.NewValue);
        }

        private static void OnCornerRadiusPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((XBorder)o).UpdateBorderCornerRadius((double)e.NewValue);
        }

        private void OnLayoutUpdated(object sender, EventArgs e) {
            UpdateChildClip();
        }

        private static void OnShadowBrushPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((XBorder)o).UpdateShadowBrush((LinearGradientBrush)e.NewValue);
        }

        private static void OnShadowSpreadPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((XBorder)o).UpdateShadowSpread((double)e.NewValue);
        }

        private void UpdateBorderChild(UIElement childElement) {
            if (_border != null) {
                _border.Child = childElement;
            }
        }

        private void UpdateBorderCornerRadius(double radius) {
            if (_border != null) {
                _border.CornerRadius = new CornerRadius(radius);
                UpdateChildClip();
            }
        }

        private void UpdateChildClip() {
            FrameworkElement child = Child;
            if (child != null) {
                double radius = Math.Max(0, CornerRadius - (BorderThickness.Left * 0.5));

                if ((radius == 0) || (ClipChild == false)) {
                    child.Clip = null;
                }
                else {
                    child.Clip = new RectangleGeometry() {
                        Rect = new Rect(0, 0, child.ActualWidth, child.ActualHeight),
                        RadiusX = radius,
                        RadiusY = radius
                    };
                }
            }
        }

        private void UpdateShadowBrush(LinearGradientBrush brush) {
            if (_shadowGradients == null) {
                return;
            }

            Color firstStopColor = Colors.Transparent;

            for (int i = 0; i < _shadowGradients.Count; i++) {
                GradientStopCollection targetGradients = _shadowGradients[i].GradientStops;
                GradientStopCollection sourceGradients = brush.GradientStops;

                targetGradients.Clear();
                for (int j = 0; j < sourceGradients.Count; j++) {
                    if ((i == 0) && (j == 0)) {
                        firstStopColor = sourceGradients[j].Color;
                    }

                    GradientStop stop = new GradientStop() {
                        Color = sourceGradients[j].Color,
                        Offset = sourceGradients[j].Offset
                    };
                    targetGradients.Add(stop);
                }
            }

            if (_shadowFill != null) {
                _shadowFill.Color = firstStopColor;
            }
        }

        private void UpdateShadowSpread(double spread) {
            if (_nineGrid == null) {
                return;
            }

            GridLength width = new GridLength(spread, GridUnitType.Pixel);
            _nineGrid.ColumnDefinitions[0].Width = width;
            _nineGrid.ColumnDefinitions[2].Width = width;

            GridLength height = new GridLength(spread, GridUnitType.Pixel);
            _nineGrid.RowDefinitions[0].Height = height;
            _nineGrid.RowDefinitions[2].Height = height;
        }
    }
}
