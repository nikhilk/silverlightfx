// DetailViewItem.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// Represents a single item within a DetailView.
    /// </summary>
    public class DetailViewItem : DataItemContentControl {

        /// <summary>
        /// Initializes an instance of a DetailViewItem.
        /// </summary>
        public DetailViewItem() {
            DefaultStyleKey = typeof(DetailViewItem);
        }
    }
}
