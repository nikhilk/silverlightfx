// DataList.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

// TODO: Need to test this code - it really hasn't been exercised beyond the super
//       basic stuff!

// TODO: How do we handle the scenario where some item not currently in the snapshot
//       is edited, and then belongs to the snapshot based on the current filter...
//       The only way to do so would be to subscribe to propchange on all entities (or
//       have an INotifyCollectionChangeEx...

namespace System.Windows.Data {

    /// <summary>
    /// An object that abstracts a collection of items and various capabilities
    /// of different collection interfaces.
    /// </summary>
    public sealed class DataList : IIndexableCollection, IEditableCollection, IPageableCollection,
                                   INotifyPropertyChanged, INotifyCollectionChanged {

        private IEnumerable _sourceData;
        private Type _itemType;
        private bool _itemTypeSupportsNew;

        private IComparer<object> _comparer;
        private IPredicate<object> _predicate;
        private bool _enableCurrency;

        private List<object> _snapShot;
        private int _currentIndex;
        private int _version;

        private bool _ignoreChanges;

        private PropertyChangedEventHandler _propChangedHandler;
        private NotifyCollectionChangedEventHandler _collectionChangedHandler;

        /// <summary>
        /// Creates an instance of a DataList from the specified underlying data.
        /// </summary>
        /// <param name="data">The underlying data to abstract.</param>
        public DataList(IEnumerable data)
            : this(data, /* enableCurrency */ true) {
        }

        /// <summary>
        /// Creates an instance of a DataList from the specified underlying data,
        /// and supplies the item type that goes into the list.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="itemType"></param>
        public DataList(IEnumerable data, Type itemType)
            : this(data, /* enableCurrency */ true, itemType) {
        }

        /// <summary>
        /// Creates an instance of a DataList from the specified underlying data.
        /// </summary>
        /// <param name="data">The underlying data to abstract.</param>
        /// <param name="enableCurrency">Whether to enable currency management.</param>
        public DataList(IEnumerable data, bool enableCurrency)
            : this(data, enableCurrency, null) {
        }

        private DataList(IEnumerable data, bool enableCurrency, Type itemType) {
            if (data == null) {
                throw new ArgumentNullException("data");
            }

            _sourceData = data;
            _currentIndex = -1;
            _enableCurrency = enableCurrency;

            if (itemType == null) {
                if (data.GetType().IsGenericType) {
                    itemType = data.GetType().GetGenericArguments()[0];
                }
            }
            if (itemType == null) {
                foreach (object o in data) {
                    itemType = o.GetType();
                    break;
                }
            }
            _itemType = itemType;
            if (itemType != null) {
                _itemTypeSupportsNew = itemType.IsVisible && itemType.IsClass &&
                                       (itemType.GetConstructor(new Type[0]) != null);
            }

            INotifyCollectionChanged sourceNotifier = data as INotifyCollectionChanged;
            if (sourceNotifier != null) {
                sourceNotifier.CollectionChanged += OnSourceCollectionChanged;
            }
        }

        /// <summary>
        /// Gets whether the current index can be moved forward.
        /// </summary>
        public bool CanMoveNext {
            get {
                if (_enableCurrency == false) {
                    return false;
                }

                EnsureSnapShot();
                return (_currentIndex < _snapShot.Count - 1);
            }
        }

        /// <summary>
        /// Gets whether the current index can be moved back.
        /// </summary>
        public bool CanMovePrevious {
            get {
                if (_enableCurrency == false) {
                    return false;
                }

                EnsureSnapShot();
                return (_currentIndex > 0);
            }
        }

        /// <summary>
        /// Gets the count of items currently in the list.
        /// </summary>
        public int Count {
            get {
                EnsureSnapShot();
                return _snapShot.Count;
            }
        }

        private int CurrentIndex {
            get {
                return _currentIndex;
            }
            set {
                if (value != -1) {
                    if (value >= _snapShot.Count) {
                        value = _snapShot.Count - 1;
                    }
                }
                else {
                    if (_snapShot.Count != 0) {
                        value = 0;
                    }
                }
                _currentIndex = value;
            }
        }

        /// <summary>
        /// Gets the item at the current index.
        /// </summary>
        public object CurrentItem {
            get {
                EnsureSnapShot();

                if (_currentIndex >= 0) {
                    return _snapShot[_currentIndex];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets whether current index is tracked and can be changed.
        /// </summary>
        public bool IsCurrencyEnabled {
            get {
                return _enableCurrency;
            }
        }

        /// <summary>
        /// Gets the item at the specified index within the underlying data.
        /// </summary>
        /// <param name="index">The index to lookup.</param>
        /// <returns>The item at the specified index.</returns>
        public object this[int index] {
            get {
                EnsureSnapShot();
                return _snapShot[index];
            }
        }

        private void AddItem(object item, bool notifyPropertyChange) {
            int index = _snapShot.Count;
            if (_comparer != null) {
                index = _snapShot.BinarySearch(item, _comparer);
                index = ~index;
            }

            bool currentIndexChanged = false;
            if (IsCurrencyEnabled && (index <= _currentIndex)) {
                currentIndexChanged = true;
            }

            _snapShot.Insert(index, item);

            if (currentIndexChanged) {
                CurrentIndex++;
            }

            if (notifyPropertyChange && (_propChangedHandler != null)) {
                RaisePropertyChanged("Count");

                if (currentIndexChanged) {
                    RaisePropertyChanged("CanMovePrevious");
                    RaisePropertyChanged("CanMoveNext");
                }
            }

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        private void EnsureSnapShot() {
            if (_snapShot != null) {
                return;
            }

            ICollection sourceCollection = _sourceData as ICollection;
            int capacity = (sourceCollection != null) ? sourceCollection.Count : 16;

            _snapShot = new List<object>(capacity);
            if ((sourceCollection == null) || (sourceCollection.Count != 0)) {
                if (_predicate != null) {
                    foreach (object o in _sourceData) {
                        if (_predicate.Filter(o)) {
                            _snapShot.Add(o);
                        }
                    }
                }
                else {
                    foreach (object o in _sourceData) {
                        _snapShot.Add(o);
                    }
                }

                if (_comparer != null) {
                    _snapShot.Sort(_comparer);
                }
            }

            if (_currentIndex < 0) {
                _currentIndex = 0;
            }
            if (_currentIndex >= _snapShot.Count) {
                _currentIndex = _snapShot.Count - 1;
            }
        }

        /// <summary>
        /// Moves the current index forward.
        /// </summary>
        public void MoveNext() {
            if (CanMoveNext == false) {
                throw new InvalidOperationException("Cannot call MoveNext at this point.");
            }

            CurrentIndex++;

            RaisePropertyChanged("CanMoveNext");
            RaisePropertyChanged("CanMovePrevious");
            RaisePropertyChanged("CurrentItem");
        }

        /// <summary>
        /// Moves the current index back.
        /// </summary>
        public void MovePrevious() {
            if (CanMovePrevious == false) {
                throw new InvalidOperationException("Cannot call MovePrevious at this point.");
            }

            CurrentIndex--;

            RaisePropertyChanged("CanMoveNext");
            RaisePropertyChanged("CanMovePrevious");
            RaisePropertyChanged("CurrentItem");
        }

        /// <summary>
        /// Sets the current item to the specified item.
        /// </summary>
        /// <param name="item">The item that should become the current item.</param>
        public void MoveToItem(object item) {
            VerifyValidItem(item);

            if (IsCurrencyEnabled == false) {
                throw new InvalidOperationException("This DataList does not support currency.");
            }

            int index = _snapShot.IndexOf(item);
            CurrentIndex = index;

            RaisePropertyChanged("CanMovePrevious");
            RaisePropertyChanged("CanMoveNext");
            RaisePropertyChanged("CurrentItem");
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (_ignoreChanges) {
                return;
            }

            UpdateVersion();
            if ((_snapShot == null) &&
                (_propChangedHandler == null) && (_collectionChangedHandler == null)) {
                // The snapshot hasn't been created, and there are no change notification
                // handlers, so there is no more work to be done.
                return;
            }

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    if ((_predicate == null) || _predicate.Filter(e.NewItems[0])) {
                        if (_snapShot != null) {
                            AddItem(e.NewItems[0], /* notifyPropertyChange */ true);
                        }
                        else {
                            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, e.NewItems[0], 0);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if ((_predicate == null) || _predicate.Filter(e.OldItems[0])) {
                        if (_snapShot != null) {
                            RemoveItem(e.OldItems[0], /* notifyPropertyChange */ true);
                        }
                        else {
                            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, e.OldItems[0], 0);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnSourceCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems[0], e.OldStartingIndex));
                    OnSourceCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems[0], e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _snapShot = null;
                    _currentIndex = -1;
                    RaiseCollectionReset();
                    break;
            }

            if (e.Action != NotifyCollectionChangedAction.Replace) {
                RaisePropertyChanged("Count");
            }
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index) {
            if (_collectionChangedHandler != null) {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, item, index);
                _collectionChangedHandler(this, e);
            }
        }

        private void RaiseCollectionReset() {
            if (_collectionChangedHandler != null) {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                _collectionChangedHandler(this, e);
            }
        }

        private void RaisePropertyChanged(string propertyName) {
            if (_propChangedHandler != null) {
                _propChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RemoveItem(object item, bool notifyPropertyChange) {
            int index = _snapShot.IndexOf(item);
            RemoveItem(item, index, notifyPropertyChange);
        }

        private void RemoveItem(object item, int index, bool notifyPropertyChange) {
            bool currentIndexChanged = false;
            if (IsCurrencyEnabled && (index <= _currentIndex)) {
                currentIndexChanged = true;
            }

            _snapShot.RemoveAt(index);

            if (currentIndexChanged) {
                CurrentIndex--;
            }

            if (notifyPropertyChange && (_propChangedHandler != null)) {
                RaisePropertyChanged("Count");

                if (currentIndexChanged) {
                    RaisePropertyChanged("CanMovePrevious");
                    RaisePropertyChanged("CanMoveNext");
                    RaisePropertyChanged("CurrentItem");
                }
            }
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        /// <summary>
        /// Sets the predicate to use to filter the items contained in this DataList.
        /// </summary>
        /// <param name="predicate">The predicate to use; null if no filter is to be applied.</param>
        public void UpdateFilter(IPredicate<object> predicate) {
            if ((_predicate == null) && (predicate == null)) {
                return;
            }

            UpdateVersion();

            if (_snapShot == null) {
                RaisePropertyChanged("Count");
                RaiseCollectionReset();
            }
            else {
                object currentItem = null;
                if (IsCurrencyEnabled) {
                    currentItem = CurrentItem;
                }

                // Potentially some items in the view are no longer in the snapshot and some new items
                // now become part of the snapshot. First we'll remove any that no longer should be in
                // the snapshot, and then we'll add whatever is.

                if (predicate != null) {
                    for (int i = _snapShot.Count - 1; i >= 0; i--) {
                        object item = _snapShot[i];
                        if (predicate.Filter(item) == false) {
                            RemoveItem(item, i, /* raisePropertyChange */ false);
                        }
                    }
                }

                if (_predicate != null) {
                    foreach (object item in _sourceData) {
                        if ((_predicate.Filter(item) == false) &&
                            ((predicate == null) || predicate.Filter(item))) {
                            AddItem(item, /* raisePropertyChange */ false);
                        }
                    }
                }

                if (currentItem != null) {
                    bool currentItemChanged = false;
                    int index = _snapShot.IndexOf(currentItem);
                    if (index == -1) {
                        index = 0;
                    }
                    CurrentIndex = index;

                    RaisePropertyChanged("CanMovePrevious");
                    RaisePropertyChanged("CanMoveNext");
                    if (currentItemChanged) {
                        RaisePropertyChanged("CurrentItem");
                    }
                }

            }

            _predicate = predicate;

            RaisePropertyChanged("Count");
        }

        /// <summary>
        /// Sets the comparer to use to sort the items contained in this DataList.
        /// </summary>
        /// <param name="comparer">The comparer to use; null if no sort is to be applied.</param>
        public void UpdateSort(IComparer<object> comparer) {
            if ((_comparer == null) && (comparer == null)) {
                return;
            }

            object currentItem = null;
            if (IsCurrencyEnabled && (_snapShot != null)) {
                currentItem = CurrentItem;
            }

            _comparer = comparer;
            _snapShot = null;

            if (currentItem != null) {
                EnsureSnapShot();

                int index = _snapShot.IndexOf(currentItem);
                CurrentIndex = index;

                RaisePropertyChanged("CanMovePrevious");
                RaisePropertyChanged("CanMoveNext");
            }

            UpdateVersion();
            RaiseCollectionReset();
        }

        private void UpdateVersion() {
            _version++;
        }

        private void VerifyValidItem(object item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            EnsureSnapShot();
            if (_snapShot.Contains(item) == false) {
                throw new ArgumentOutOfRangeException("The specified item is not part of this DataList.");
            }
        }


        #region Implementation of IEnumerable
        IEnumerator IEnumerable.GetEnumerator() {
            EnsureSnapShot();
            return _snapShot.GetEnumerator();
        }
        #endregion

        #region Implementation of IIndexableCollection
        int IIndexableCollection.Count {
            get {
                return Count;
            }
        }

        object IIndexableCollection.this[int index] {
            get {
                return this[index];
            }
        }
        #endregion

        #region Implementation of IEditableCollection
        bool IEditableCollection.CanAdd {
            get {
                return (_itemType != null) && (_sourceData is IList);
            }
        }

        bool IEditableCollection.CanCancelEditItem {
            get {
                return ((_itemType != null) && typeof(IEditableObject).IsAssignableFrom(_itemType));
            }
        }

        bool IEditableCollection.CanCreateNew {
            get {
                return _itemTypeSupportsNew;
            }
        }

        bool IEditableCollection.CanEditItems {
            get {
                return ((_itemType != null) && typeof(IEditableObject).IsAssignableFrom(_itemType));
            }
        }

        bool IEditableCollection.CanRemove {
            get {
                return (_sourceData is IList);
            }
        }

        void IEditableCollection.AddItem(object item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (_itemType.IsAssignableFrom(item.GetType())) {
                throw new ArgumentOutOfRangeException("Invalid item type being added.");
            }
            if (((IEditableCollection)this).CanAdd == false) {
                throw new InvalidOperationException("This collection does not support adds.");
            }

            _ignoreChanges = true;
            try {
                ((IList)_sourceData).Add(item);
                UpdateVersion();

                if ((_predicate == null) || _predicate.Filter(item)) {
                    EnsureSnapShot();
                    AddItem(item, /* notifyPropertyChanges */ true);
                }
            }
            finally {
                _ignoreChanges = false;
            }
        }

        void IEditableCollection.BeginEditItem(object item) {
            VerifyValidItem(item);

            if (((IEditableCollection)this).CanEditItems == false) {
                throw new InvalidOperationException("This list does not support item editing.");
            }

            ((IEditableObject)item).BeginEdit();
        }

        void IEditableCollection.CancelEditItem(object item) {
            VerifyValidItem(item);

            if (((IEditableCollection)this).CanCancelEditItem == false) {
                throw new InvalidOperationException("This list does not support canceling item editing.");
            }

            ((IEditableObject)item).CancelEdit();
        }

        object IEditableCollection.CreateNew() {
            if (((IEditableCollection)this).CanCreateNew == false) {
                throw new InvalidOperationException("This list does not support creating new items.");
            }

            return Activator.CreateInstance(_itemType);
        }

        void IEditableCollection.EndEditItem(object item) {
            VerifyValidItem(item);

            if (((IEditableCollection)this).CanEditItems == false) {
                throw new InvalidOperationException("This list does not support item editing.");
            }

            ((IEditableObject)item).EndEdit();

            if ((_predicate != null) && (_predicate.Filter(item) == false)) {
                UpdateVersion();
                RemoveItem(item, /* notifyPropertyChange */ true);
            }
            else if (_comparer != null) {
                int oldIndex = _snapShot.IndexOf(item);
                bool isCurrent = IsCurrencyEnabled && (CurrentIndex == oldIndex);

                _snapShot.RemoveAt(oldIndex);

                int newIndex = _snapShot.BinarySearch(item, _comparer);
                newIndex = ~newIndex;

                _snapShot.Insert(newIndex, item);

                if (oldIndex != newIndex) {
                    UpdateVersion();

                    // TODO: Whats a better way to do this without simulating via a remove followed
                    //       by an Add

                    RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, oldIndex);
                    RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, newIndex);

                    if (isCurrent) {
                        CurrentIndex = newIndex;

                        RaisePropertyChanged("CanMovePrevious");
                        RaisePropertyChanged("CanMoveNext");
                    }
                }
            }
        }

        void IEditableCollection.RemoveItem(object item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (((IEditableCollection)this).CanRemove == false) {
                throw new InvalidOperationException("This collection does not support removes.");
            }

            _ignoreChanges = true;
            try {
                if ((_predicate == null) || _predicate.Filter(item)) {
                    UpdateVersion();

                    EnsureSnapShot();
                    RemoveItem(item, /* notifyPropertyChanges */ true);
                }

                ((IList)_sourceData).Remove(item);
            }
            finally {
                _ignoreChanges = false;
            }
        }
        #endregion

        #region Implementation of IPageableCollection
        int IPageableCollection.Count {
            get {
                return Count;
            }
        }

        IEnumerable IPageableCollection.GetPage(int pageIndex, int pageSize) {
            if (pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex");
            }
            if (pageSize < 1) {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            EnsureSnapShot();
            return new DataRange(this, pageIndex, pageSize);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
            add {
                _propChangedHandler =
                    (PropertyChangedEventHandler)Delegate.Combine(_propChangedHandler, value);
            }
            remove {
                _propChangedHandler =
                    (PropertyChangedEventHandler)Delegate.Remove(_propChangedHandler, value);
            }
        }
        #endregion

        #region Implementation of INotifyCollectionChanged
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
            add {
                _collectionChangedHandler =
                    (NotifyCollectionChangedEventHandler)Delegate.Combine(_collectionChangedHandler, value);
            }
            remove {
                _collectionChangedHandler =
                    (NotifyCollectionChangedEventHandler)Delegate.Remove(_collectionChangedHandler, value);
            }
        }
        #endregion


        private sealed class DataRange : IEnumerable, IEnumerator {

            private DataList _owner;

            private int _startIndex;
            private int _endIndex;
            private int _version;

            private int _index;

            public DataRange(DataList owner, int pageIndex, int pageSize) {
                _owner = owner;

                _startIndex = pageIndex * pageSize;
                _endIndex = _startIndex + pageSize;
                _version = owner._version;

                _index = -1;
            }

            #region Implementation of IEnumerator
            IEnumerator IEnumerable.GetEnumerator() {
                return this;
            }
            #endregion

            #region Implementation of IEnumerable
            object IEnumerator.Current {
                get {
                    if (_index == -1) {
                        throw new InvalidOperationException("Enumerator was not initialized or has no current item.");
                    }
                    if (_version != _owner._version) {
                        throw new InvalidOperationException("This enumerator is no longer valid.");
                    }

                    return _owner._snapShot[_index];
                }
            }

            bool IEnumerator.MoveNext() {
                if (_version != _owner._version) {
                    throw new InvalidOperationException("This enumerator is no longer valid.");
                }

                if (_index == -1) {
                    _index = _startIndex;
                }
                else {
                    _index++;
                    if (_index > _endIndex) {
                        _index = -1;
                    }
                }

                if (_index >= _owner._snapShot.Count) {
                    _index = -1;
                }
                return (_index != -1);
            }

            void IEnumerator.Reset() {
                if (_version != _owner._version) {
                    throw new InvalidOperationException("This enumerator is no longer valid.");
                }

                _index = -1;
            }
            #endregion
        }
    }
}
