// LayoutControl.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A control that presents a list of multiple individual content elements arranged
    /// in a layout defined by a set of ContentPresenter elements. This layout itself is
    /// defined in the same way a UserControl's content is defined.
    /// Content elements are matched to associated ContentPresenter elements
    /// via the attached ContentName property.
    /// </summary>
    [ContentProperty("ContentList")]
    public class LayoutControl : UserControl {

        private static readonly DependencyProperty ContainerProperty =
            DependencyProperty.RegisterAttached("Container", typeof(LayoutControl), typeof(LayoutControl), null);

        /// <summary>
        /// Represents the ContentName attached property.
        /// </summary>
        public static readonly DependencyProperty ContentNameProperty =
            DependencyProperty.RegisterAttached("ContentName", typeof(string), typeof(LayoutControl),
                                                new PropertyMetadata(OnContentNamePropertyChanged));

        private ObservableCollection<UIElement> _contentList;
        private bool _loaded;

        /// <summary>
        /// Initializes an instance of a LayoutControl.
        /// </summary>
        public LayoutControl() {
            _contentList = new ObservableCollection<UIElement>();
            _contentList.CollectionChanged += OnContentListCollectionChanged;

            Loaded += OnLoaded;
        }

        /// <summary>
        /// The list of content elements within the control.
        /// </summary>
        public ObservableCollection<UIElement> ContentList {
            get {
                return _contentList;
            }
        }

        internal static LayoutControl GetContainer(DependencyObject o) {
            return (LayoutControl)o.GetValue(ContainerProperty);
        }

        /// <summary>
        /// Gets the value of the ContentName attached property.
        /// </summary>
        /// <param name="o">The object with the attached property.</param>
        /// <returns>The name if it has been set; null otherwise.</returns>
        public static string GetContentName(DependencyObject o) {
            return (string)o.GetValue(ContentNameProperty);
        }

        private ContentPresenter GetPresenter(string name) {
            if (String.IsNullOrEmpty(name) == false) {
                return FindName(name) as ContentPresenter;
            }
            return null;
        }

        private void OnContentListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (_loaded == false) {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add) {
                UIElement content = (UIElement)e.NewItems[0];

                LayoutControl currentOwner = LayoutControl.GetContainer(content);
                if (currentOwner != null) {
                    currentOwner.ContentList.Remove(content);
                }

                LayoutControl.SetContainer(content, this);

                string contentName = LayoutControl.GetContentName(content);

                ContentPresenter contentPresenter = GetPresenter(contentName);
                if (contentPresenter != null) {
                    Grid grid = new Grid();
                    contentPresenter.Content = grid;

                    grid.Children.Add(content);
                    grid.UpdateLayout();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                UIElement content = (UIElement)e.OldItems[0];

                string contentName = LayoutControl.GetContentName(content);

                ContentPresenter contentPresenter = GetPresenter(contentName);
                if (contentPresenter != null) {
                    contentPresenter.Content = null;
                }
            }
        }

        private void OnContentNameChanged(DependencyObject content, string oldName, string newName) {
            ContentPresenter oldPresenter = GetPresenter(oldName);
            if (oldPresenter != null) {
                oldPresenter.Content = null;
            }

            ContentPresenter newPresenter = GetPresenter(newName);
            if (newPresenter != null) {
                Grid grid = new Grid();
                grid.Children.Add((UIElement)content);

                newPresenter.Content = grid;
                grid.UpdateLayout();
            }
        }

        private static void OnContentNamePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            LayoutControl currentOwner = GetContainer(o);
            if (currentOwner != null) {
                currentOwner.OnContentNameChanged(o, (string)e.OldValue, (string)e.NewValue);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (_contentList.Count != 0) {
                Dispatcher.BeginInvoke(delegate() {
                    bool layoutContentAdded = false;

                    foreach (UIElement content in _contentList) {
                        if (layoutContentAdded == false) {
                            Content = content;
                            layoutContentAdded = true;
                            continue;
                        }

                        string contentName = LayoutControl.GetContentName(content);
                        ContentPresenter contentPresenter = GetPresenter(contentName);

                        if (contentPresenter != null) {
                            Grid grid = new Grid();
                            grid.Children.Add(content);

                            contentPresenter.Content = grid;
                            grid.UpdateLayout();
                        }
                    }
                });
            }

            _loaded = true;
        }

        internal static void SetContainer(DependencyObject o, LayoutControl value) {
            o.SetValue(ContainerProperty, value);
        }

        /// <summary>
        /// Sets the value of the ContentName attached property.
        /// </summary>
        /// <param name="o">The object to attach a name to.</param>
        /// <param name="value">The name to set.</param>
        public static void SetContentName(DependencyObject o, string value) {
            o.SetValue(ContentNameProperty, value);
        }
    }
}
