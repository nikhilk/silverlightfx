// IPredicate.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Collections {

    /// <summary>
    /// Represents a collection with editing semantics over the list
    /// of items, and the individual items.
    /// </summary>
    public interface IEditableCollection : IEnumerable {

        /// <summary>
        /// Whether the collection supports additions.
        /// </summary>
        bool CanAdd {
            get;
        }

        /// <summary>
        /// Whether the collection supports canceling editing on items.
        /// </summary>
        bool CanCancelEditItem {
            get;
        }

        /// <summary>
        /// Whether the collection supports creating new items.
        /// </summary>
        bool CanCreateNew {
            get;
        }

        /// <summary>
        /// Whether the collection supports editing items.
        /// </summary>
        bool CanEditItems {
            get;
        }

        /// <summary>
        /// Whether the collection supports removals.
        /// </summary>
        bool CanRemove {
            get;
        }

        /// <summary>
        /// Adds the specified item into the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddItem(object item);

        /// <summary>
        /// Begins editing the specified item.
        /// </summary>
        /// <param name="item">The item to edit.</param>
        void BeginEditItem(object item);

        /// <summary>
        /// Cancels editing the specified item.
        /// </summary>
        /// <param name="item">The item to cancel editing on.</param>
        void CancelEditItem(object item);

        /// <summary>
        /// Creates a new item that can be added to the collection.
        /// </summary>
        /// <returns>A new item not yet in the collection.</returns>
        object CreateNew();

        /// <summary>
        /// Ends editing the specified item.
        /// </summary>
        /// <param name="item">The item to end editing on.</param>
        void EndEditItem(object item);

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        void RemoveItem(object item);
    }
}
