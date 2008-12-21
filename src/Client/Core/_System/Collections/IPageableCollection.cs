// IPageableCollection.cs
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
    /// Represents a collection with ability to provide a paged view
    /// of the contained items.
    /// </summary>
    public interface IPageableCollection : IEnumerable {

        /// <summary>
        /// Gets the number of items contained within the collection.
        /// </summary>
        int Count {
            get;
        }

        /// <summary>
        /// Gets a subset of the items contained within the collection
        /// that fit into the specified page definition.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>A collection of items fitting into the specified page.</returns>
        IEnumerable GetPage(int pageIndex, int pageSize);
    }
}
