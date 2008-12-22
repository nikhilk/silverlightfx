// AutoCompleteCompletedEventArgs.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// This product's copyrights are licensed under the Creative
// Commons Attribution-ShareAlike (version 2.5).B
// http://creativecommons.org/licenses/by-sa/2.5/
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// The event data associated with the AutoComplete behavior's Completed event.
    /// </summary>
    public class AutoCompleteCompletedEventArgs : EventArgs {

        private string _prefix;
        private object _selectedItem;

        internal AutoCompleteCompletedEventArgs(string prefix, object selectedItem) {
            _prefix = prefix;
            _selectedItem = selectedItem;
        }

        /// <summary>
        /// The current textbox entry being used as a prefix for determining
        /// completion items.
        /// </summary>
        public string Prefix {
            get {
                return _prefix;
            }
        }

        /// <summary>
        /// The item that was selected by the user. This can be set to alter the
        /// selection, or to customize the string that will be used to update the
        /// TextBox associated with the AutoComplete behavior.
        /// </summary>
        public object SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
            }
        }
    }
}
