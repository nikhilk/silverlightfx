// PageJournal.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;

namespace SilverlightFX.UserInterface.Navigation {

    internal sealed class PageJournal {

        private List<Uri> _journalEntries;
        private int _current;

        public PageJournal() {
            _journalEntries = new List<Uri>();
            _current = -1;
        }

        public bool CanGoBack {
            get {
                return _current > 0;
            }
        }

        public bool CanGoForward {
            get {
                return _current < _journalEntries.Count - 1;
            }
        }

        public void AddEntry(Uri uri) {
            if (_current != _journalEntries.Count - 1) {
                _journalEntries.RemoveRange(_current, _journalEntries.Count - _current);
            }

            _journalEntries.Add(uri);
            _current = _journalEntries.Count - 1;
        }

        public Uri GoBack() {
            if (_current > 0) {
                _current--;
                return _journalEntries[_current];
            }
            return null;
        }

        public Uri GoForward() {
            if (_current < _journalEntries.Count - 1) {
                _current++;
                return _journalEntries[_current];
            }
            return null;
        }
    }
}
