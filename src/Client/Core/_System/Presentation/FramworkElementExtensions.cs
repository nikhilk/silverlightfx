// FrameworkElementExtensions.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows {

    internal static class FrameworkElementExtensions {

        public static object FindNameRecursive(this FrameworkElement element, string name) {
            FrameworkElement rootVisual = element.GetRootVisual();
            if (rootVisual == null) {
                return null;
            }

            return rootVisual.FindName(name);
#if FALSE
            while (element != null) {
                object value = element.FindName(name);
                if (value != null) {
                    return value;
                }

                element = element.GetParent();
            }

            return null;
#endif
        }

        public static object FindResource(this FrameworkElement element, string key) {
            while (element != null) {
                object value = element.Resources[key];
                if (value != null) {
                    return value;
                }

                element = element.GetParentVisual();
            }

            return null;
        }

        public static FrameworkElement GetParentVisual(this FrameworkElement element) {
            return VisualTreeHelper.GetParent(element) as FrameworkElement;
        }

        public static FrameworkElement GetRootVisual(this FrameworkElement element) {
            FrameworkElement parent = null;
            while (element != null) {
                parent = element;
                if (parent is UserControl) {
                    // HACK: A UserControl parented to another UserControl has a non-null
                    //       parent; however we want to consider the UserControl as the
                    //       root visual for its contents...
                    break;
                }

                element = element.Parent as FrameworkElement;
            }

            return parent;
        }
    }
}
