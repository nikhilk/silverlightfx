// AutoComplete.cs
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
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Runtime.Serialization.Json;
using System.Windows.Threading;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// A behavior that can be associated with the TextBox control to add auto-completion
    /// functionality.
    /// </summary>
    public class AutoComplete : Behavior<TextBox> {

        private Uri _serviceUri;
        private Type _serviceResultType;
        private bool _serviceCachingEnabled;
        private int _minimumPrefixLength;
        private DataTemplate _dropDownTemplate;

        private EventHandler<AutoCompleteCompletingEventArgs> _completingHandler;
        private EventHandler<AutoCompleteCompletedEventArgs> _completedHandler;

        private CompletionCache _completionCache;

        private string _prefix;
        private DispatcherTimer _timer;
        private WebClient _request;
        private Popup _popup;
        private ListBox _dropDown;

        private bool _ignoreTextChange;
        private bool _ignoreSelectionChange;
        private bool _processTextChange;
        private string _previousText;
        private Uri _resolvedServiceUri;

        /// <summary>
        /// Initializes an instance of an AutoComplete behavior.
        /// </summary>
        public AutoComplete() {
            _minimumPrefixLength = 3;
            _serviceResultType = typeof(string);
        }

        /// <summary>
        /// Gets or sets the template used to customize the dropdown. The template
        /// must contain a ListBox or other derived control.
        /// If this is not set, a default ListBox control is created.
        /// </summary>
        public DataTemplate DropDownTemplate {
            get {
                return _dropDownTemplate;
            }
            set {
                _dropDownTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets whether results of invoking the associated service should be
        /// cached or not. The AutoComplete by default caches the last 10 sets of items.
        /// </summary>
        public bool EnableServiceResultCaching {
            get {
                return _serviceCachingEnabled;
            }
            set {
                _serviceCachingEnabled = value;
                if (value == false) {
                    ClearCache();
                }
            }
        }

        /// <summary>
        /// Gets or sets the length of the minimum text entry before the dropdown is
        /// displayed. This defaults to 3.
        /// </summary>
        public int MinimumPrefixLength {
            get {
                return _minimumPrefixLength;
            }
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _minimumPrefixLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of items returned by the service. The expectation is that
        /// the service returns a JSON-formatted array of items of the specified type. The
        /// default type is String.
        /// </summary>
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type ServiceResultType {
            get {
                return _serviceResultType;
            }
            set {
                _serviceResultType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Uri of the service to invoke to fetch the list of completion
        /// items to display in the dropdown.
        /// </summary>
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri ServiceUri {
            get {
                return _serviceUri;
            }
            set {
                _serviceUri = value;
            }
        }

        /// <summary>
        /// Raised to indicate that user has selected an item from the dropdown. This event
        /// can be used to automatically fill other fields on the form based on selection, or
        /// to customize the text that will be used to update the TextBox.
        /// </summary>
        public event EventHandler<AutoCompleteCompletedEventArgs> Completed {
            add {
                _completedHandler = (EventHandler<AutoCompleteCompletedEventArgs>)Delegate.Combine(_completedHandler, value);
            }
            remove {
                _completedHandler = (EventHandler<AutoCompleteCompletedEventArgs>)Delegate.Remove(_completedHandler, value);
            }
        }

        /// <summary>
        /// Raised to indicate that the dropdown is being displayed. This event can be used
        /// to suppress the dropdown, or provide a computed set of items to display instead
        /// of invoking the specified service.
        /// </summary>
        public event EventHandler<AutoCompleteCompletingEventArgs> Completing {
            add {
                _completingHandler = (EventHandler<AutoCompleteCompletingEventArgs>)Delegate.Combine(_completingHandler, value);
            }
            remove {
                _completingHandler = (EventHandler<AutoCompleteCompletingEventArgs>)Delegate.Remove(_completingHandler, value);
            }
        }

        /// <summary>
        /// Indicates whether the dropdown is currently being displayed.
        /// </summary>
        public bool IsCompleting {
            get {
                return (_popup != null) && _popup.IsOpen;
            }
        }

        /// <summary>
        /// Clears the cache of service results.
        /// </summary>
        public void ClearCache() {
            _completionCache = null;
        }

        /// <summary>
        /// Closes the completion dropdown without making a selection if it is open.
        /// </summary>
        public void CancelCompletion() {
            CloseDropDown();
        }

        private void CloseDropDown() {
            if (_request != null) {
                _request.CancelAsync();
                _request = null;
            }

            if (_dropDown != null) {
                _dropDown.SelectionChanged -= OnDropDownSelectionChanged;
                _dropDown = null;
            }

            if (_popup != null) {
                _popup.IsOpen = false;
                ((Screen)Application.Current.RootVisual).DisposePopup(_popup);

                _popup = null;
            }
        }

        /// <internalonly />
        protected override void OnAttach() {
            AssociatedObject.KeyDown += OnTextBoxKeyDown;
            AssociatedObject.TextChanged += OnTextBoxTextChanged;
            AssociatedObject.LostFocus += OnTextBoxLostFocus;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += OnTimerTick;

            _previousText = AssociatedObject.Text;
        }

        /// <internalonly />
        protected override void OnDetach() {
            CloseDropDown();

            AssociatedObject.LostFocus -= OnTextBoxLostFocus;
            AssociatedObject.TextChanged -= OnTextBoxTextChanged;
            AssociatedObject.KeyDown -= OnTextBoxKeyDown;

            _timer.Stop();
        }

        internal void OnDropDownSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (_ignoreSelectionChange) {
                _ignoreSelectionChange = false;
                return;
            }

            UpdateText();
            CloseDropDown();
        }

        private void OnRequestCompleted(object sender, OpenReadCompletedEventArgs e) {
            WebClient request = (WebClient)sender;
            if (_request != request) {
                return;
            }

            _request = null;
            if (e.Cancelled == false) {
                IList items = null;

                try {
                    Type listType = typeof(List<>).MakeGenericType(_serviceResultType);
                    DataContractJsonSerializer dcjs = new DataContractJsonSerializer(listType);

                    items = (IList)dcjs.ReadObject(e.Result);
                }
                catch {
                }

                if ((items != null) && (items.Count != 0)) {
                    if (_serviceCachingEnabled) {
                        if (_completionCache == null) {
                            _completionCache = new CompletionCache();
                        }
                        _completionCache.AddItems(_prefix, items);
                    }

                    ShowDropDown(items);
                }
            }
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e) {
            if ((_popup != null) && _popup.IsOpen) {
                if ((e.Key == Key.Up) || (e.Key == Key.Down)) {
                    if (e.Key == Key.Up) {
                        if (_dropDown.SelectedIndex > 0) {
                            _ignoreSelectionChange = true;
                            _dropDown.SelectedIndex--;

                            _dropDown.ScrollIntoView(_dropDown.Items[_dropDown.SelectedIndex]);
                        }
                    }
                    else {
                        if (_dropDown.SelectedIndex < _dropDown.Items.Count - 1) {
                            _ignoreSelectionChange = true;
                            _dropDown.SelectedIndex++;

                            _dropDown.ScrollIntoView(_dropDown.Items[_dropDown.SelectedIndex]);
                        }
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape) {
                    CloseDropDown();

                    e.Handled = true;
                }
                else if ((e.Key == Key.Enter) || (e.Key == Key.Tab)) {
                    UpdateText();
                    CloseDropDown();

                    e.Handled = true;
                }
            }
            _processTextChange = true;
        }

        private void OnTextBoxLostFocus(object sender, EventArgs e) {
            _processTextChange = false;

            _timer.Stop();
            CloseDropDown();
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e) {
            if (_ignoreTextChange) {
                _ignoreTextChange = false;
                return;
            }

            _timer.Stop();
            CloseDropDown();

            if (_processTextChange && (AssociatedObject.Text.Length >= _minimumPrefixLength)) {
                _timer.Start();
            }
        }

        private void OnTimerTick(object sender, EventArgs e) {
            _timer.Stop();

            if (_request != null) {
                _request.CancelAsync();
                _request = null;
            }

            string prefix = AssociatedObject.Text;

            AutoCompleteCompletingEventArgs cea = null;
            if (_completingHandler != null) {
                cea = new AutoCompleteCompletingEventArgs(prefix);
                _completingHandler(this, cea);
                if (cea.IsDropDownSuppressed) {
                    return;
                }

                if (cea.CompletionItems != null) {
                    _prefix = prefix;
                    ShowDropDown(cea.CompletionItems);
                    return;
                }
            }

            if (_completionCache != null) {
                IList cachedItems = _completionCache.GetItems(prefix);
                if (cachedItems != null) {
                    _prefix = prefix;
                    ShowDropDown(cachedItems);

                    return;
                }
            }

            if (_serviceUri == null) {
                return;
            }

            _request = new WebClient();

            if (_resolvedServiceUri == null) {
                _resolvedServiceUri = _serviceUri;
                if (_serviceUri.IsAbsoluteUri == false) {
                    _resolvedServiceUri = new Uri(Application.Current.Host.Source, _serviceUri);
                }
            }

            StringBuilder queryBuilder = new StringBuilder("prefix=" + HttpUtility.UrlEncode(prefix));
            if ((cea != null) && (cea.ServiceParameters != null)) {
                foreach (KeyValuePair<string, string> entry in cea.ServiceParameters) {
                    queryBuilder.Append("&");
                    queryBuilder.Append(HttpUtility.UrlEncode(entry.Key));
                    queryBuilder.Append("=");
                    queryBuilder.Append(HttpUtility.UrlEncode(entry.Value));
                }
            }

            UriBuilder uriBuilder = new UriBuilder(_resolvedServiceUri);
            uriBuilder.Query = queryBuilder.ToString();

            Uri requestUri = uriBuilder.Uri;

            _prefix = prefix;
            _request.OpenReadCompleted += OnRequestCompleted;
            _request.OpenReadAsync(requestUri);
        }

        private void ShowDropDown(IList items) {
            Screen screen = Application.Current.RootVisual as Screen;
            if (screen == null) {
                throw new InvalidOperationException("AutoComplete requires the root visual to be a Screen.");
            }

            GeneralTransform transform = AssociatedObject.TransformToVisual((UIElement)Application.Current.RootVisual);
            Point transformedPoint = transform.Transform(new Point(0, 0));

            _dropDown = null;
            if (_dropDownTemplate == null) {
                _dropDown = new ListBox();
            }
            else {
                _dropDown = _dropDownTemplate.LoadContent() as ListBox;
                if (_dropDown == null) {
                    throw new InvalidOperationException("The DropDownTemplate must contain a ListBox control.");
                }
            }
            _dropDown.IsTabStop = false;
            _dropDown.ItemsSource = items;
            _dropDown.SelectionChanged += OnDropDownSelectionChanged;

            _dropDown.Width = AssociatedObject.ActualWidth;
            _dropDown.MaxHeight = AssociatedObject.ActualHeight * 5;
            _dropDown.RenderTransform = new TranslateTransform {
                X = transformedPoint.X,
                Y = transformedPoint.Y + AssociatedObject.ActualHeight - 1
            };

            _popup = screen.CreatePopup();
            _popup.Child = _dropDown;
            _popup.IsOpen = true;
        }

        private void UpdateText() {
            object selectedItem = _dropDown.SelectedItem;
            if (selectedItem == null) {
                return;
            }

            _ignoreTextChange = true;

            if (_completedHandler != null) {
                AutoCompleteCompletedEventArgs cea = new AutoCompleteCompletedEventArgs(_prefix, selectedItem);
                _completedHandler(this, cea);

                selectedItem = cea.SelectedItem;
            }

            string text = selectedItem.ToString();
            AssociatedObject.Text = text;
            AssociatedObject.Select(text.Length, 0);
        }


        private sealed class CompletionCache {

            private const int MaxEntries = 10;

            private string[] _prefixes;
            private Dictionary<string, IList> _itemsMap;

            public CompletionCache() {
                _prefixes = new string[MaxEntries];
                _itemsMap = new Dictionary<string, IList>();
            }

            public IList GetItems(string prefix) {
                IList items = null;
                _itemsMap.TryGetValue(prefix, out items);

                return items;
            }

            public void AddItems(string prefix, IList items) {
                string removedPrefix = null;

                int i = 0;
                int count = _itemsMap.Count;
                if (count > 0) {
                    for (i = 0; i < count; i++) {
                        if (String.Compare(_prefixes[i], prefix, StringComparison.Ordinal) == 0) {
                            break;
                        }
                    }
                    if (i == 0) {
                        // the prefix already exists and is first in the list
                        return;
                    }

                    // shift prefixes down by 1
                    int shiftIndex;
                    if (i < count) {
                        shiftIndex = i;
                    }
                    else {
                        shiftIndex = count;
                        if (shiftIndex == MaxEntries) {
                            shiftIndex--;
                        }
                    }

                    removedPrefix = _prefixes[shiftIndex];
                    for (int j = shiftIndex; j > 0; j--) {
                        _prefixes[j] = _prefixes[j - 1];
                    }
                }
                _prefixes[0] = prefix;
                _itemsMap[prefix] = items;

                if (removedPrefix != null) {
                    _itemsMap.Remove(removedPrefix);
                }
            }
        }
    }
}
