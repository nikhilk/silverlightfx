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

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A ToolTipService implementation that allows the contents of the tooltip to share
    /// the DataContext of the element that the tooltip is associated with, and thereby
    /// contain binding expressions.
    /// </summary>
    public static class XToolTipService {

        /// <summary>
        /// Represents the ToolTip attached property.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.RegisterAttached("ToolTip", typeof(object),
                                                typeof(XToolTipService),
                                                new PropertyMetadata(OnToolTipChanged));

        /// <summary>
        /// Gets the Tooltip associated with the specified element.
        /// </summary>
        /// <param name="element">The element to lookup.</param>
        /// <returns>The current tooltip if one exists; null otherwise.</returns>
        public static object GetToolTip(FrameworkElement element) {
            return element.GetValue(XToolTipService.ToolTipProperty);
        }

        /// <summary>
        /// Sets the Tooltip associated with the specified element.
        /// </summary>
        /// <param name="element">The element whose tooltip is to be set.</param>
        /// <param name="value">The tooltip to use.</param>
        public static void SetToolTip(FrameworkElement element, object value) {
            element.SetValue(XToolTipService.ToolTipProperty, value);
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e) {
            FrameworkElement element = (FrameworkElement)sender;

            element.Loaded -= OnElementLoaded;

            FrameworkElement toolTipElement = element.GetValue(XToolTipService.ToolTipProperty) as FrameworkElement;
            if (toolTipElement != null) {
                toolTipElement.DataContext = element.DataContext;
            }
            ToolTipService.SetToolTip(element, toolTipElement);
        }

        private static void OnToolTipChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded += OnElementLoaded;
        }
    }
}
