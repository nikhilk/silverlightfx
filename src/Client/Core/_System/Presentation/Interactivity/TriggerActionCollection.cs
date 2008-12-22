// TriggerActionCollection.cs
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
using System.Collections.ObjectModel;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Represents a collection of TriggerAction objects.
    /// </summary>
    public sealed class TriggerActionCollection : ObservableCollection<TriggerAction> {

        private DependencyObject _associatedObject;

        internal TriggerActionCollection() {
        }

        internal DependencyObject AssociatedObject {
            get {
                return _associatedObject;
            }
            set {
                _associatedObject = value;
                if (_associatedObject != null) {
                    foreach (TriggerAction action in this) {
                        ((IAttachedObject)action).Attach(_associatedObject);
                    }
                }
                else {
                    foreach (TriggerAction action in this) {
                        ((IAttachedObject)action).Detach();
                    }
                }
            }
        }

        /// <internalonly />
        protected override void InsertItem(int index, TriggerAction item) {
            base.InsertItem(index, item);

            if (item.Owner != null) {
                item.Owner.Remove(item);
            }

            item.Owner = this;
            if (_associatedObject != null) {
                ((IAttachedObject)item).Attach(_associatedObject);
            }
        }

        /// <internalonly />
        protected override void RemoveItem(int index) {
            TriggerAction action = this[index];

            ((IAttachedObject)action).Detach();
            action.Owner = null;

            base.RemoveItem(index);
        }
    }
}
