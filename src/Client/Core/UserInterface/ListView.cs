// ListView.cs
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A simple data-bound list control.
    /// </summary>
    [TemplatePart(Name = "ItemsPresenter", Type = typeof(Panel))]
    public class ListView : DataboundControl {

        /// <summary>
        /// Represents the ItemAddedEffect property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemAddedEffectProperty =
            DependencyProperty.Register("ItemAddedEffect", typeof(AnimationEffect), typeof(ListView), null);

        /// <summary>
        /// Represents the ItemContainerStyle property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ListView),
                                        new PropertyMetadata(OnItemContainerStylePropertyChanged));

        /// <summary>
        /// Represents the ItemFilter property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemFilterProperty =
            DependencyProperty.Register("ItemFilter", typeof(IPredicate<object>), typeof(ListView),
                                        new PropertyMetadata(OnItemFilterPropertyChanged));

        /// <summary>
        /// Represents the ItemRemovedEffect property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemRemovedEffectProperty =
            DependencyProperty.Register("ItemRemovedEffect", typeof(AnimationEffect), typeof(ListView), null);

        /// <summary>
        /// Represents the ItemSort property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemSortProperty =
            DependencyProperty.Register("ItemSort", typeof(IComparer<object>), typeof(ListView),
                                        new PropertyMetadata(OnItemSortPropertyChanged));

        /// <summary>
        /// Represents the ItemTemplate property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ListView),
                                        new PropertyMetadata(OnItemTemplatePropertyChanged));

        private Panel _itemsPresenter;

        private List<ListViewItem> _items;
        private Dictionary<object, ListViewItem> _itemMap;

        /// <summary>
        /// Initializes an instance of a ListView.
        /// </summary>
        public ListView() {
            DefaultStyleKey = typeof(ListView);

            _items = new List<ListViewItem>();
            _itemMap = new Dictionary<object, ListViewItem>();
        }

        /// <summary>
        /// Whether the control is empty, i.e. is not bound or is bound to an empty collection.
        /// </summary>
        public bool IsEmpty {
            get {
                return (_items.Count == 0);
            }
        }

        /// <summary>
        /// Gets or sets the effect applied to items when they are added to the list.
        /// </summary>
        public AnimationEffect ItemAddedEffect {
            get {
                return (AnimationEffect)GetValue(ItemAddedEffectProperty);
            }
            set {
                SetValue(ItemAddedEffectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style applied to each item in the list.
        /// </summary>
        public Style ItemContainerStyle {
            get {
                return (Style)GetValue(ItemContainerStyleProperty);
            }
            set {
                SetValue(ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter applied to the data source.
        /// </summary>
        public IPredicate<object> ItemFilter {
            get {
                return (IPredicate<object>)GetValue(ItemFilterProperty);
            }
            set {
                SetValue(ItemFilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the effect applied to items when they are removed from the list.
        /// </summary>
        public AnimationEffect ItemRemovedEffect {
            get {
                return (AnimationEffect)GetValue(ItemRemovedEffectProperty);
            }
            set {
                SetValue(ItemRemovedEffectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sort applied to the data source.
        /// </summary>
        public IComparer<object> ItemSort {
            get {
                return (IComparer<object>)GetValue(ItemSortProperty);
            }
            set {
                SetValue(ItemSortProperty, value);
            }
        }

        /// <summary>
        /// Gets the list of items created within the control.
        /// </summary>
        public IEnumerable<ListViewItem> Items {
            get {
                return _items;
            }
        }

        /// <summary>
        /// The item template used to display each item in the list of items
        /// that the ListView is bound to.
        /// </summary>
        public DataTemplate ItemTemplate {
            get {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set {
                SetValue(ItemTemplateProperty, value);
            }
        }

        private void AddItem(object dataItem, int index) {
            if (_itemsPresenter == null) {
                return;
            }

            Style itemContainerStyle = ItemContainerStyle;
            DataTemplate itemTemplate = ItemTemplate;
            if (itemTemplate == null) {
                return;
            }

            ListViewItem item = CreateItem(dataItem, index, itemTemplate, itemContainerStyle);
            if (item != null) {
                _itemsPresenter.Children.Insert(index, item);

                AnimationEffect addedEffect = ItemAddedEffect;
                if (addedEffect != null) {
                    addedEffect.Target = (FrameworkElement)item.Content;

                    ProceduralAnimation addedAnimation = ((IProceduralAnimationFactory)addedEffect).CreateAnimation();
                    addedAnimation.Play(item);
                }
            }
        }

        private ListViewItem CreateItem(object dataItem, int index, DataTemplate itemTemplate, Style itemContainerStyle) {
            FrameworkElement uiItem = itemTemplate.LoadContent() as FrameworkElement;
            if (uiItem != null) {
                ListViewItem item = new ListViewItem();
                if (itemContainerStyle != null) {
                    item.Style = itemContainerStyle;
                }
                item.DataContext = dataItem;
                item.Content = uiItem;

                _items.Insert(index, item);
                _itemMap[dataItem] = item;

                return item;
            }

            return null;
        }

        private void CreateItems() {
            if (_itemsPresenter == null) {
                return;
            }

            DataList dataList = DataList;
            if (dataList == null) {
                return;
            }

            Style itemContainerStyle = ItemContainerStyle;
            DataTemplate itemTemplate = ItemTemplate;
            if (itemTemplate == null) {
                return;
            }

            UIElementCollection items = _itemsPresenter.Children;
            int index = 0;
            foreach (object dataItem in dataList) {
                ListViewItem item = CreateItem(dataItem, index, itemTemplate, itemContainerStyle);
                if (item != null) {
                    items.Add(item);
                    index++;
                }
            }
        }

        /// <summary>
        /// Gets the item associated with the specified data item.
        /// </summary>
        /// <param name="dataItem">The data item to lookup.</param>
        /// <returns>The item if one exists; null otherwise.</returns>
        public ListViewItem GetItem(object dataItem) {
            if (dataItem == null) {
                throw new ArgumentNullException("dataItem");
            }

            ListViewItem item;
            if (_itemMap.TryGetValue(dataItem, out item)) {
                return item;
            }

            return null;
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _itemsPresenter = GetTemplateChild("ItemsPresenter") as Panel;
            ResetItems(/* recreateItems */ true);
        }

        /// <internalonly />
        protected override void OnDataListChanging() {
            DataList dataList = DataList;
            if (dataList != null) {
                ((INotifyCollectionChanged)dataList).CollectionChanged -= OnDataViewCollectionChanged;
            }
        }

        /// <internalonly />
        protected override void OnDataListChanged() {
            DataList dataList = DataList;
            if (dataList != null) {
                ((INotifyCollectionChanged)dataList).CollectionChanged += OnDataViewCollectionChanged;
            }

            ResetItems(/* recreateItems */ true);
        }

        private void OnDataViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    AddItem(e.NewItems[0], e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItem(e.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    UpdateItem(e.OldItems[0], e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetItems(/* recreateItems */ true);
                    break;
            }
        }

        private static void OnItemContainerStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ListView)o).ResetItems(/* recreateItems */ true);
        }

        private void OnItemFilterChanged(IPredicate<object> newFilter) {
            DataList dataList = DataList;
            if (dataList != null) {
                dataList.UpdateFilter(newFilter);
            }
        }

        private static void OnItemFilterPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
            ((ListView)sender).OnItemFilterChanged((IPredicate<object>)e.NewValue);
        }

        private void OnItemRemovedAnimationStopped(object sender, EventArgs e) {
            if (_itemsPresenter == null) {
                return;
            }

            ProceduralAnimation animation = (ProceduralAnimation)sender;
            ListViewItem item = (ListViewItem)animation.AssociatedElement;

            _itemsPresenter.Children.Remove(item);
        }

        private void OnItemSortChanged(IComparer<object> newSort) {
            DataList dataList = DataList;
            if (dataList != null) {
                dataList.UpdateSort(newSort);
            }
        }

        private static void OnItemSortPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
            ((ListView)sender).OnItemSortChanged((IComparer<object>)e.NewValue);
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((ListView)o).ResetItems(/* recreateItems */ true);
        }

        /// <internalonly />
        protected override void OnLoaded(RoutedEventArgs e) {
            base.OnLoaded(e);
            ApplyTemplate();
        }


        private void RemoveItem(object dataItem) {
            if (_itemsPresenter == null) {
                return;
            }

            ListViewItem item = GetItem(dataItem);
            if (item == null) {
                return;
            }

            _items.Remove(item);
            _itemMap.Remove(dataItem);

            AnimationEffect removedEffect = ItemRemovedEffect;
            if (removedEffect == null) {
                _itemsPresenter.Children.Remove(item);
            }
            else {
                removedEffect.Target = (FrameworkElement)item.Content;
                
                ProceduralAnimation removeAnimation = ((IProceduralAnimationFactory)removedEffect).CreateAnimation();
                removeAnimation.Stopped += OnItemRemovedAnimationStopped;

                removeAnimation.Play(item);
            }
        }

        private void ResetItems(bool recreateItems) {
            if (_itemsPresenter != null) {
                _itemsPresenter.Children.Clear();

                _items.Clear();
                _itemMap.Clear();

                DataList dataList = DataList;
                if (dataList != null) {
                    CreateItems();
                }
            }
        }

        private void UpdateItem(object oldDataItem, object newDataItem) {
            if (_itemsPresenter == null) {
                return;
            }

            ListViewItem item = GetItem(oldDataItem);
            if (item == null) {
                return;
            }

            _itemMap.Remove(oldDataItem);
            _itemMap[newDataItem] = item;
            item.DataContext = newDataItem;
        }
    }
}
