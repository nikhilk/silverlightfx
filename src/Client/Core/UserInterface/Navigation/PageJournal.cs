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
using System.Windows;
using System.Windows.Browser;
using System.Windows.Interop;

namespace SilverlightFX.UserInterface.Navigation {

    internal sealed class PageJournal {

        private List<Uri> _journalEntries;
        private int _current;

        private bool _addingJournalEntry;
        private bool _integrated;
        private Action<Uri> _navigateAction;

        public PageJournal() {
            _journalEntries = new List<Uri>();
            _current = -1;
        }

        public bool CanGoBack {
            get {
                if (_integrated) {
                    return true;
                }
                return _current > 0;
            }
        }

        public bool CanGoForward {
            get {
                if (_integrated) {
                    return true;
                }
                return _current < _journalEntries.Count - 1;
            }
        }

        public bool CanIntegrateWithBrowser {
            get {
                return HtmlPage.IsEnabled;
            }
        }

        public bool IsIntegratedWithBrowser {
            get {
                return _integrated;
            }
        }

        public void AddEntry(Uri uri) {
            if (_integrated) {
                try {
                    _addingJournalEntry = true;
                    Application.Current.Host.NavigationState = uri.ToString();
                }
                finally {
                    _addingJournalEntry = false;
                }
            }
            else {
                if (_current != _journalEntries.Count - 1) {
                    _journalEntries.RemoveRange(_current, _journalEntries.Count - _current);
                }

                _journalEntries.Add(uri);
                _current = _journalEntries.Count - 1;
            }
        }

        public Uri GoBack() {
            if (_integrated) {
                ((ScriptObject)HtmlPage.Window.GetProperty("history")).Invoke("back");
                return null;
            }

            if (_current > 0) {
                _current--;
                return _journalEntries[_current];
            }
            return null;
        }

        public Uri GoForward() {
            if (_integrated) {
                ((ScriptObject)HtmlPage.Window.GetProperty("history")).Invoke("forward");
                return null;
            }

            if (_current < _journalEntries.Count - 1) {
                _current++;
                return _journalEntries[_current];
            }
            return null;
        }

        public void IntegrateWithBrowser(Action<Uri> navigateAction) {
            _navigateAction = navigateAction;

            Application.Current.Host.NavigationStateChanged += OnHostNavigationStateChanged;
            _integrated = true;
        }

        private void OnHostNavigationStateChanged(object sender, NavigationStateChangedEventArgs e) {
            if (_addingJournalEntry) {
                return;
            }

            _navigateAction(new Uri(e.NewNavigationState, UriKind.Relative));
        }
    }
}
