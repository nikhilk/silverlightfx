// ListViewItem.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents a single item within a ListView.
    /// </summary>
    public class ListViewItem : DataItemContentControl {

        /// <summary>
        /// Initializes an instance of a ListViewItem.
        /// </summary>
        public ListViewItem() {
            DefaultStyleKey = typeof(ListViewItem);
        }
    }
}
