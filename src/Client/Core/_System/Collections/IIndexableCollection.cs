// IIndexableCollection.cs
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
    /// Represents a collection with ability to access items by index.
    /// </summary>
    public interface IIndexableCollection : IEnumerable {

        /// <summary>
        /// The number of items contained in the collection.
        /// </summary>
        int Count {
            get;
        }

        /// <summary>
        /// Gets the item at the specified index in the collection.
        /// </summary>
        /// <param name="index">The index to lookup.</param>
        /// <returns>The item at the specified index.</returns>
        object this[int index] {
            get;
        }
    }
}
