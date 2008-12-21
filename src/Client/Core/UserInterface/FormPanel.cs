// FormPanel.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// You are free to:
// - use this framework as part of your app
// - make use of the framework in a commercial app
// as long as your app or product is itself not a framework, control pack or
// developer toolkit of any sort under the following conditions:
// Attribution. You must attribute the original work in your
//              product or release.
// Share Alike. If you alter, transform, or build as-is upon this work,
//              you may only distribute the resulting app source under
//              a license identical to this one.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Silverlight.FX.UserInterface {

    /// <summary>
    /// A Panel control with behavior to automatically layout its
    /// children in a form-like manner. The children consist of regular
    /// elements along with Label elements to represent associated form labels.
    /// </summary>
    public class FormPanel : System.Windows.Controls.Grid {

        /// <summary>
        /// Represents the IsLabeled attached property.
        /// </summary>
        public static readonly DependencyProperty IsLabeledProperty =
            DependencyProperty.RegisterAttached("IsLabeled", typeof(bool), typeof(FormPanel),
                                                new PropertyMetadata(true, OnLayoutAttachedPropertyChanged));

        /// <summary>
        /// Represents the IsStretched attached property.
        /// </summary>
        public static readonly DependencyProperty IsStretchedProperty =
            DependencyProperty.RegisterAttached("IsStretched", typeof(bool), typeof(FormPanel),
                                                new PropertyMetadata(false, OnLayoutAttachedPropertyChanged));

        /// <summary>
        /// Represents the LabelAlignment property.
        /// </summary>
        public static readonly DependencyProperty LabelAlignmentProperty =
            DependencyProperty.Register("LabelAlignment", typeof(FormPanelLabelAlignment), typeof(FormPanel),
                                        new PropertyMetadata(FormPanelLabelAlignment.Left, OnLayoutPropertyChanged));

        /// <summary>
        /// Represents the LabelPosition attached property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.RegisterAttached("LabelPosition", typeof(FormPanelLabelPosition), typeof(FormPanel),
                                                new PropertyMetadata(FormPanelLabelPosition.Left, OnLayoutAttachedPropertyChanged));

        /// <summary>
        /// Represents the LabelSpacing property.
        /// </summary>
        public static readonly DependencyProperty LabelSpacingProperty =
            DependencyProperty.Register("LabelSpacing", typeof(int), typeof(FormPanel),
                                        new PropertyMetadata(4, OnLayoutPropertyChanged));

        /// <summary>
        /// Represents the Orientation property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FormPanel),
                                        new PropertyMetadata(Orientation.Vertical, OnLayoutPropertyChanged));

        /// <summary>
        /// Represents the Spacing property.
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(int), typeof(FormPanel),
                                        new PropertyMetadata(6, OnLayoutPropertyChanged));

        /// <summary>
        /// Represents the SpacingMode attached property.
        /// </summary>
        public static readonly DependencyProperty SpacingModeProperty =
            DependencyProperty.RegisterAttached("SpacingMode", typeof(FormPanelSpacing), typeof(FormPanel),
                                                new PropertyMetadata(FormPanelSpacing.Normal, OnLayoutAttachedPropertyChanged));

        private bool _loaded;
        private int _lastLayoutChildCount;

        /// <summary>
        /// Initializes a new instance of a FormPanel panel.
        /// </summary>
        public FormPanel() {
            Loaded += OnLoaded;
            LayoutUpdated += OnLayoutUpdated;
        }

        /// <summary>
        /// Gets or sets the alignment of labels in the FormPanel.
        /// </summary>
        public FormPanelLabelAlignment LabelAlignment {
            get {
                return (FormPanelLabelAlignment)GetValue(LabelAlignmentProperty);
            }
            set {
                SetValue(LabelAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the Label and its associated element.
        /// </summary>
        public int LabelSpacing {
            get {
                return (int)GetValue(LabelSpacingProperty);
            }
            set {
                SetValue(LabelSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the panel that determines whether the
        /// contents are laid out top to bottom or left to right.
        /// </summary>
        public Orientation Orientation {
            get {
                return (Orientation)GetValue(OrientationProperty);
            }
            set {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between consecutive pairs of labels and their
        /// associated elements.
        /// </summary>
        public int Spacing {
            get {
                return (int)GetValue(SpacingProperty);
            }
            set {
                SetValue(SpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets whether a particular element in a FormPanel has an associated Label.
        /// The default is true.
        /// </summary>
        /// <param name="element">The element to lookup.</param>
        /// <returns>true if the element has a Label; false otherwise.</returns>
        public static bool GetIsLabeled(FrameworkElement element) {
            return (bool)element.GetValue(IsLabeledProperty);
        }

        /// <summary>
        /// Gets whether a particular element in a FormPanel should be stretched to
        /// consume all available space.
        /// </summary>
        /// <param name="element">The element to lookup.</param>
        /// <returns>true if the element should be stretched; false otherwise.</returns>
        public static bool GetIsStretched(FrameworkElement element) {
            return (bool)element.GetValue(IsStretchedProperty);
        }

        /// <summary>
        /// Gets the position of a Label with respect to its associated element in
        /// a FormPanel.
        /// </summary>
        /// <param name="element">The Label element to lookup.</param>
        /// <returns>The Left or Top relative position.</returns>
        public static FormPanelLabelPosition GetLabelPosition(Label element) {
            return (FormPanelLabelPosition)element.GetValue(LabelPositionProperty);
        }

        /// <summary>
        /// Gets the spacing mode of the element relative to the previous label/element group.
        /// </summary>
        /// <param name="element">The element to set.</param>
        public static FormPanelSpacing GetSpacingMode(FrameworkElement element) {
            return (FormPanelSpacing)element.GetValue(SpacingModeProperty);
        }

        private static void OnLayoutAttachedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            FormPanel grid = (FormPanel)((FrameworkElement)o).Parent;
            if (grid != null) {
                grid.UpdateLayout(/* orientationChanged */ false);
            }
        }

        private static void OnLayoutPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((FormPanel)o).UpdateLayout(e.Property == OrientationProperty);
        }

        private void OnLayoutUpdated(object sender, EventArgs e) {
            if (_loaded) {
                if (_lastLayoutChildCount != Children.Count) {
                    Dispatcher.BeginInvoke(delegate() {
                        UpdateLayout(/* orientationChanged */ false);
                    });
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            _loaded = true;
            UpdateLayout(/* orientationChanged */ false);
        }

        /// <summary>
        /// Sets whether a particular element in a FormPanel has an associated Label.
        /// </summary>
        /// <param name="element">The element to set.</param>
        /// <param name="labeled">true if the element has an associated Label; false otherwise.</param>
        public static void SetIsLabeled(FrameworkElement element, bool labeled) {
            element.SetValue(IsLabeledProperty, labeled);
        }

        /// <summary>
        /// Sets whether a particular element in a FormPanel is stretched to consume all available space.
        /// </summary>
        /// <param name="element">The element to set.</param>
        /// <param name="stretched">true if the element should be stretched; false otherwise.</param>
        public static void SetIsStretched(FrameworkElement element, bool stretched) {
            element.SetValue(IsStretchedProperty, stretched);
        }

        /// <summary>
        /// Sets the relative position of a Label with respect to its associated element
        /// in a FormPanel.
        /// </summary>
        /// <param name="element">The Label element to set.</param>
        /// <param name="labelPosition">The relative position.</param>
        public static void SetLabelPosition(Label element, FormPanelLabelPosition labelPosition) {
            element.SetValue(LabelPositionProperty, labelPosition);
        }

        /// <summary>
        /// Sets the spacing mode of the element relative to the previous label/element group.
        /// </summary>
        /// <param name="element">The element to set.</param>
        /// <param name="spacingMode">The spacing mode to use.</param>
        public static void SetSpacingMode(FrameworkElement element, FormPanelSpacing spacingMode) {
            element.SetValue(SpacingModeProperty, spacingMode);
        }

        private void UpdateLayout(bool orientationChanged) {
            if (_loaded == false) {
                return;
            }

            _lastLayoutChildCount = Children.Count;

            if (orientationChanged) {
                ColumnDefinitions.Clear();
                RowDefinitions.Clear();
            }

            if (Orientation == Orientation.Horizontal) {
                UpdateLayoutHorizontal();
            }
            else {
                UpdateLayoutVertical();
            }
        }

        private void UpdateLayoutHorizontal() {
        }

        private void UpdateLayoutVertical() {
            ColumnDefinitionCollection columns = ColumnDefinitions;
            if (columns.Count == 0) {
                // For a vertical layout, we have three columns:
                // the Label, LabelSpacing, and the Control

                columns.Add(new ColumnDefinition() {
                    Width = new GridLength(0, GridUnitType.Auto)
                });
                columns.Add(new ColumnDefinition() {
                    Width = new GridLength(LabelSpacing, GridUnitType.Pixel)
                });
                columns.Add(new ColumnDefinition() {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            RowDefinitionCollection rows = RowDefinitions;
            rows.Clear();

            int regularSpacing = Spacing;
            int labelSpacing = LabelSpacing;

            bool hasStretchingElement = false;

            bool spanColumns = false;
            bool createNewRow = true;
            bool addSpacing = false;
            int verticalSpacing = regularSpacing;

            HorizontalAlignment labelAlignment = HorizontalAlignment.Left;
            if (LabelAlignment == FormPanelLabelAlignment.Right) {
                labelAlignment = HorizontalAlignment.Right;
            }

            for (int i = 0; i < Children.Count; i++) {
                FrameworkElement element = (FrameworkElement)Children[i];
                Label labelElement = element as Label;

                // Create a new row if we know we need to create one or we
                // encountered a label (even if we weren't expecting one)
                if (createNewRow || ((labelElement != null) && (i != 0))) {
                    if (addSpacing && (i != 0)) {
                        FormPanelSpacing spacingMode = GetSpacingMode(element);

                        if (spacingMode != FormPanelSpacing.Ignore) {
                            if (spacingMode == FormPanelSpacing.Extra) {
                                verticalSpacing += verticalSpacing;
                            }
                            rows.Add(new RowDefinition() {
                                Height = new GridLength(verticalSpacing, GridUnitType.Pixel)
                            });
                        }
                        addSpacing = false;
                    }
                    rows.Add(new RowDefinition() {
                        Height = new GridLength(0, GridUnitType.Auto)
                    });
                }

                Grid.SetRow(element, rows.Count - 1);

                if (labelElement != null) {
                    if (GetLabelPosition(labelElement) == FormPanelLabelPosition.Top) {
                        labelElement.HorizontalAlignment = HorizontalAlignment.Left;
                        Grid.SetColumnSpan(labelElement, 3);

                        spanColumns = true;
                        createNewRow = true;
                        verticalSpacing = labelSpacing;
                        addSpacing = true;
                    }
                    else {
                        labelElement.HorizontalAlignment = labelAlignment;
                        if (labelElement.VerticalAlignment != VerticalAlignment.Top) {
                            labelElement.VerticalAlignment = VerticalAlignment.Center;
                        }

                        spanColumns = false;

                        createNewRow = false;
                        verticalSpacing = regularSpacing;
                    }
                }
                else {
                    if (spanColumns || (GetIsLabeled(element) == false)) {
                        Grid.SetColumnSpan(element, 3);
                        spanColumns = false;
                    }
                    else {
                        Grid.SetColumn(element, 2);
                        if (Double.IsNaN(element.Width) == false) {
                            element.HorizontalAlignment = HorizontalAlignment.Left;
                        }
                    }

                    if ((hasStretchingElement == false) && GetIsStretched(element)) {
                        rows[rows.Count - 1].Height = new GridLength(1, GridUnitType.Star);
                        hasStretchingElement = true;
                        element.VerticalAlignment = VerticalAlignment.Stretch;
                    }
                    else {
                        element.VerticalAlignment = VerticalAlignment.Center;
                    }

                    createNewRow = true;
                    verticalSpacing = regularSpacing;
                    addSpacing = true;
                }
            }

            if (hasStretchingElement == false) {
                // Add the final filler row
                rows.Add(new RowDefinition() {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }
        }
    }
}
