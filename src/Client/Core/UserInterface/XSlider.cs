// XSlider.cs
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
using System.Windows.Input;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A extended Slider control with support for
    /// tracking mouse clicks to change the value of the slider.
    /// </summary>
    public class XSlider : Slider {

        private Thumb _horizontalThumb;
        private Thumb _verticalThumb;

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _horizontalThumb = GetTemplateChild("HorizontalThumb") as Thumb;
            _verticalThumb = GetTemplateChild("VerticalThumb") as Thumb;

            FrameworkElement leftTracker = GetTemplateChild("LeftTrack") as FrameworkElement;
            FrameworkElement rightTracker = GetTemplateChild("RightTrack") as FrameworkElement;
            if (leftTracker != null) {
                leftTracker.MouseLeftButtonDown += OnHorizontalTrackerMouseDown;
            }
            if (rightTracker != null) {
                rightTracker.MouseLeftButtonDown += OnHorizontalTrackerMouseDown;
            }

            FrameworkElement topTracker = GetTemplateChild("TopTrack") as FrameworkElement;
            FrameworkElement bottomTracker = GetTemplateChild("BottomTrack") as FrameworkElement;
            if (topTracker != null) {
                topTracker.MouseLeftButtonDown += OnVerticalTrackerMouseDown;
            }
            if (bottomTracker != null) {
                bottomTracker.MouseLeftButtonDown += OnVerticalTrackerMouseDown;
            }
        }

        private void OnHorizontalTrackerMouseDown(object sender, MouseButtonEventArgs e) {
            Point p = e.GetPosition(this);
            Value = (p.X - (_horizontalThumb.ActualWidth / 2)) / (ActualWidth - _horizontalThumb.ActualWidth) * Maximum;
        }

        private void OnVerticalTrackerMouseDown(object sender, MouseButtonEventArgs e) {
            Point p = e.GetPosition(this);
            Value = (p.X - (_verticalThumb.ActualHeight / 2)) / (ActualWidth - _verticalThumb.ActualHeight) * Maximum;
        }
    }
}
