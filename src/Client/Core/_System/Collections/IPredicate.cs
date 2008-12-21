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
    /// Provides the ability to check if an item matches the encapsulated
    /// critieria.
    /// </summary>
    public interface IPredicate {

        /// <summary>
        /// Filters an item using the encapsulated criteria.
        /// </summary>
        /// <param name="item">The item to filter.</param>
        /// <returns>true if the item matches; false otherwise.</returns>
        bool Filter(object item);
    }
}

namespace System.Collections.Generic {

    /// <summary>
    /// Provides the ability to check if an item matches the encapsulated
    /// critieria.
    /// </summary>
    /// <typeparam name="T">The type of object that can be matched.</typeparam>
    public interface IPredicate<T> {

        /// <summary>
        /// Filters an item using the encapsulated criteria.
        /// </summary>
        /// <param name="item">The item to filter.</param>
        /// <returns>true if the item matches; false otherwise.</returns>
        bool Filter(T item);
    }
}
