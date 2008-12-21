// BehaviorCollection.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Represents a collection of behaviors associated with the same DependencyObject.
    /// </summary>
    public sealed class BehaviorCollection : ObservableCollection<Behavior> {

        private DependencyObject _associatedObject;

        internal BehaviorCollection(DependencyObject o) {
            _associatedObject = o;
        }

        /// <internalonly />
        protected override void InsertItem(int index, Behavior item) {
            base.InsertItem(index, item);

            if (item.Owner != null) {
                item.Owner.Remove(item);
            }

            item.Owner = this;
            ((IAttachedObject)item).Attach(_associatedObject);
        }

        /// <internalonly />
        protected override void RemoveItem(int index) {
            Behavior behavior = this[index];

            ((IAttachedObject)behavior).Detach();
            behavior.Owner = null;

            base.RemoveItem(index);
        }
    }
}
