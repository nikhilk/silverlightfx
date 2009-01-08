// DetailView.cs
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
using System.Windows.Input;
using System.Windows.Media.Glitz;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A simple data-bound list control that shows one item at a time.
    /// </summary>
    [TemplatePart(Name = "ItemPresenter", Type = typeof(Grid))]
    [TemplateVisualState(GroupName = "DataStates", Name = "Empty")]
    [TemplateVisualState(GroupName = "DataStates", Name = "NonEmpty")]
    public class DetailView : DataboundControl {

        /// <summary>
        /// Represents the ItemContainerStyle property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(DetailView),
                                        new PropertyMetadata(OnItemContainerStylePropertyChanged));

        /// <summary>
        /// Represents the ItemTemplate property on a ListView control.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(DetailView),
                                        new PropertyMetadata(OnItemTemplatePropertyChanged));

        /// <summary>
        /// Represents the ItemTransition property on a DetailView control.
        /// </summary>
        public static readonly DependencyProperty ItemTransitionProperty =
            DependencyProperty.Register("ItemTransition", typeof(Transition), typeof(DetailView), null);

        private Panel _itemPresenter;
        private DetailViewItem _item;

        private DelegateCommand _nextCommand;
        private DelegateCommand _previousCommand;

        /// <summary>
        /// Initializes an instance of a DetailView.
        /// </summary>
        public DetailView() {
            DefaultStyleKey = typeof(DetailView);

            _nextCommand = new DelegateCommand(OnNextCommand, /* canExecute */ false);
            _previousCommand = new DelegateCommand(OnPreviousCommand, /* canExecute */ false);

            Resources.Add("NextCommand", _nextCommand);
            Resources.Add("PreviousCommand", _previousCommand);
        }

        /// <summary>
        /// Whether the control is empty, i.e. is not bound or is bound to an empty collection.
        /// </summary>
        public bool IsEmpty {
            get {
                return (_item == null);
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
        /// Gets the current item created within the control.
        /// </summary>
        public DetailViewItem Item {
            get {
                return _item;
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

        /// <summary>
        /// The transition used to switch from one item to another.
        /// </summary>
        public Transition ItemTransition {
            get {
                return (Transition)GetValue(ItemTransitionProperty);
            }
            set {
                SetValue(ItemTransitionProperty, value);
            }
        }

        /// <internalonly />
        protected override DataList CreateDataList(IEnumerable data) {
            return new DataList(data, /* enableCurrency */ true);
        }

        /// <internalonly />
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _itemPresenter = GetTemplateChild("ItemPresenter") as Grid;
            Update(/* useTransition */ false);
        }

        /// <internalonly />
        protected override void OnDataListChanging() {
            DataList dataList = DataList;
            if (dataList != null) {
                ((INotifyPropertyChanged)dataList).PropertyChanged -= OnDataListPropertyChanged;
            }
        }

        /// <internalonly />
        protected override void OnDataListChanged() {
            DataList dataList = DataList;
            if (dataList != null) {
                ((INotifyPropertyChanged)dataList).PropertyChanged += OnDataListPropertyChanged;
            }

            Update(/* useTransition */ true);

            _nextCommand.UpdateStatus(dataList.CanMoveNext);
            _previousCommand.UpdateStatus(dataList.CanMovePrevious);
        }

        private void OnDataListPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "CurrentItem") {
                Update(/* useTransition */ true);
            }
            else if (e.PropertyName == "CanMoveNext") {
                _nextCommand.UpdateStatus(DataList.CanMoveNext);
            }
            else if (e.PropertyName == "CanMovePrevious") {
                _previousCommand.UpdateStatus(DataList.CanMovePrevious);
            }
        }

        private static void OnItemContainerStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((DetailView)o).Update(/* useTransition */ false);
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((DetailView)o).Update(/* useTransition */ false);
        }

        private void OnItemTransitionCompleted(object sender, EventArgs e) {
            if (_itemPresenter != null) {
                _itemPresenter.Children.RemoveAt(1);
            }
        }

        /// <internalonly />
        protected override void OnLoaded(RoutedEventArgs e) {
            base.OnLoaded(e);
            ApplyTemplate();

            if (_item == null) {
                VisualStateManager.GoToState(this, "Empty", /* useTransitions */ false);
            }
        }

        private void OnNextCommand(object parameter) {
            DataList dataList = DataList;
            if (dataList.CanMoveNext) {
                dataList.MoveNext();
            }
        }

        private void OnPreviousCommand(object parameter) {
            DataList dataList = DataList;
            if (dataList.CanMovePrevious) {
                dataList.MovePrevious();
            }
        }

        private void Update(bool useTransition) {
            if (_itemPresenter == null) {
                return;
            }

            DataTemplate itemTemplate = ItemTemplate;
            if (itemTemplate == null) {
                return;
            }

            object dataItem = null;
            DataList dataList = DataList;
            if (dataList != null) {
                dataItem = dataList.CurrentItem;
            }

            DetailViewItem item = new DetailViewItem();
            Style itemContainerStyle = ItemContainerStyle;
            if (itemContainerStyle != null) {
                item.Style = itemContainerStyle;
            }

            item.DataContext = dataItem;

            if (dataItem != null) {
                FrameworkElement uiItem = itemTemplate.LoadContent() as FrameworkElement;
                if (uiItem != null) {
                    item.Content = uiItem;
                }
            }

            Transition itemTransition = ItemTransition;
            if ((itemTransition == null) || (_item == null)) {
                useTransition = false;
            }

            if (useTransition == false) {
                _itemPresenter.Children.Clear();
            }
            _itemPresenter.Children.Insert(0, item);

            if (useTransition) {
                if (((IAttachedObject)itemTransition).AssociatedObject != _itemPresenter) {
                    ((IAttachedObject)itemTransition).Detach();
                    ((IAttachedObject)itemTransition).Attach(_itemPresenter);

                    itemTransition.Completed += OnItemTransitionCompleted;
                }

                itemTransition.PlayEffect(EffectDirection.Forward);
            }

            if (dataItem != null) {
                if (_item == null) {
                    VisualStateManager.GoToState(this, "NonEmpty", /* useTransitions */ true);
                }
                _item = item;
            }
            else {
                if (_item != null) {
                    VisualStateManager.GoToState(this, "Empty", /* useTransitions */ true);
                }
                _item = null;
            }
        }
    }
}
