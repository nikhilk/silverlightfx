// DataboundControl.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.ComponentModel;

namespace System.Windows.Data {

    /// <summary>
    /// Represents an object or component that is able to provide
    /// data source functionality, i.e. a collection of items.
    /// </summary>
    public interface IDataSource : INotifyPropertyChanged {

        /// <summary>
        /// Gets the collection of items contained within underlying collection.
        /// </summary>
        object Data {
            get;
        }
    }
}
