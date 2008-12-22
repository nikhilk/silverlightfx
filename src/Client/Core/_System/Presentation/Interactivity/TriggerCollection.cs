// TriggerCollection.cs
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
using System.Windows;

namespace System.Windows.Interactivity {

    /// <summary>
    /// Represents a collection of triggers associated with the same DependencyObject.
    /// </summary>
    public sealed class TriggerCollection : ObservableCollection<Trigger> {

        private DependencyObject _associatedObject;

        internal TriggerCollection(DependencyObject o) {
            _associatedObject = o;
        }

        /// <internalonly />
        protected override void InsertItem(int index, Trigger item) {
            base.InsertItem(index, item);

            if (item.Owner != null) {
                item.Owner.Remove(item);
            }

            item.Owner = this;
            ((IAttachedObject)item).Attach(_associatedObject);
        }

        /// <internalonly />
        protected override void RemoveItem(int index) {
            Trigger trigger = this[index];

            ((IAttachedObject)trigger).Detach();
            trigger.Owner = null;

            base.RemoveItem(index);
        }
    }
}
