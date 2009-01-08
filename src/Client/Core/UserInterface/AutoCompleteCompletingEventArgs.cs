// AutoCompleteCompletingEventArgs.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
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
    /// The event data associated with the AutoComplete behavior's Completing event.
    /// </summary>
    public class AutoCompleteCompletingEventArgs : EventArgs {

        private string _prefix;
        private bool _suppressDropDown;
        private Dictionary<string, string> _serviceParameters;
        private IList _completionItems;

        internal AutoCompleteCompletingEventArgs(string prefix) {
            _prefix = prefix;
        }

        internal IList CompletionItems {
            get {
                return _completionItems;
            }
        }

        internal bool IsDropDownSuppressed {
            get {
                return _suppressDropDown;
            }
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

        internal IDictionary<string, string> ServiceParameters {
            get {
                return _serviceParameters;
            }
        }

        /// <summary>
        /// Allows adding a query string parameter to the web request that will
        /// be invoked to get a list of completion items in addition to the
        /// prefix itself.
        /// </summary>
        /// <param name="name">The name of the query string parameter.</param>
        /// <param name="value">The value of the query string parameter.</param>
        public void AddServiceParameter(string name, string value) {
            if (String.IsNullOrEmpty(name)) {
                throw new ArgumentNullException("name");
            }
            if (value == null) {
                throw new ArgumentNullException("value");
            }

            if (_serviceParameters == null) {
                _serviceParameters = new Dictionary<string, string>();
            }
            _serviceParameters[name] = value;
        }

        /// <summary>
        /// Allows providing a set of completion items to be used instead of
        /// invoking a web request to fetch a list. This enables using autocompletion
        /// with computed values.
        /// </summary>
        /// <param name="items">The alternate set of items to use.</param>
        public void SetCompletionItems(IList items) {
            _completionItems = items;
        }

        /// <summary>
        /// Suppresses the AutoComplete dropdown for this particular prefix.
        /// </summary>
        public void SuppressDropDown() {
            _suppressDropDown = true;
        }
    }
}
