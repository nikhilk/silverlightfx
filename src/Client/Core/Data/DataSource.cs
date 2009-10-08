// DataSource.cs
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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SilverlightFX.Data {

    // TODO: Add support for paging

    /// <summary>
    /// A base class for data source controls. A data source control is responsible
    /// for managing loading of data into the presentation.
    /// </summary>
    [TemplateVisualState(Name = DataSource.IdleActivityState, GroupName = "ActivityStates")]
    [TemplateVisualState(Name = DataSource.LoadingActivityState, GroupName = "ActivityStates")]
    public abstract class DataSource : ContentControl, IAsyncControl {

        private const string IdleActivityState = "Idle";
        private const string LoadingActivityState = "Loading";

        private static readonly DependencyProperty AsyncDataProperty =
            DependencyProperty.Register("AsyncData", typeof(Async), typeof(ObjectDataSource), null);

        private static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable), typeof(ObjectDataSource), null);

        private static readonly DependencyProperty DataItemProperty =
            DependencyProperty.Register("DataItem", typeof(object), typeof(ObjectDataSource), null);

        private static readonly DependencyProperty IsLoadingAsyncDataProperty =
            DependencyProperty.Register("IsLoadingAsyncData", typeof(bool), typeof(ObjectDataSource),
                                        new PropertyMetadata(false));

        private bool _autoLoadData;
        private DispatcherTimer _dataLoadTimer;
        private DispatcherTimer _refreshTimer;
        private string _dataLoadStatusText;

        private EventHandler<CancelEventArgs> _loadingDataEventHandler;
        private EventHandler _dataLoadedEventHandler;
        private EventHandler _errorEventHandler;
        private EventHandler _asyncActivityChangedHandler;

        private DelegateCommand _loadDataCommand;

        /// <summary>
        /// Initializes an instance of a DataSource control.
        /// </summary>
        protected DataSource() {
            DefaultStyleKey = typeof(DataSource);

            _dataLoadTimer = new DispatcherTimer();
            _dataLoadTimer.Interval = TimeSpan.FromSeconds(1);
            _dataLoadTimer.Tick += OnDataLoadTimerTick;

            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.Zero;
            _refreshTimer.Tick += OnRefreshTimerTick;

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Gets the current asynchronously loading data. This is valid when
        /// IsLoadingAsyncData is true.
        /// </summary>
        public Async AsyncData {
            get {
                return (Async)GetValue(AsyncDataProperty);
            }
            private set {
                SetValue(AsyncDataProperty, value);
                if (_asyncActivityChangedHandler != null) {
                    _asyncActivityChangedHandler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Whether the data source should automatically load data upon startup.
        /// </summary>
        public bool AutoLoadData {
            get {
                return _autoLoadData;
            }
            set {
                _autoLoadData = value;
            }
        }

        /// <summary>
        /// Gets the current data loaded by the data source.
        /// </summary>
        public IEnumerable Data {
            get {
                return (IEnumerable)GetValue(DataProperty);
            }
            private set {
                SetValue(DataProperty, value);
            }
        }

        /// <summary>
        /// Gets the first item within the current data loaded by the data source.
        /// </summary>
        public object DataItem {
            get {
                return GetValue(DataItemProperty);
            }
            set {
                SetValue(DataItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the delay that is used when a load operation needs to
        /// be performed. This allows batching multiple changes into a single
        /// attempt to load data.
        /// </summary>
        public TimeSpan DataLoadDelay {
            get {
                return _dataLoadTimer.Interval;
            }
            set {
                _dataLoadTimer.Interval = value;
            }
        }

        /// <summary>
        /// Gets or sets the status text to use as a message when the data source
        /// is loading data.
        /// </summary>
        public string DataLoadStatusText {
            get {
                return _dataLoadStatusText;
            }
            set {
                _dataLoadStatusText = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration after which the data loaded by the data source
        /// is refreshed.
        /// </summary>
        public TimeSpan DataRefreshInterval {
            get {
                return _refreshTimer.Interval;
            }
            set {
                _refreshTimer.Interval = value;
                if (_refreshTimer.Interval == TimeSpan.Zero) {
                    _refreshTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Whether the data source is performing an asynchronous load operation.
        /// </summary>
        public bool IsLoadingAsyncData {
            get {
                return (bool)GetValue(IsLoadingAsyncDataProperty);
            }
            private set {
                SetValue(IsLoadingAsyncDataProperty, value);

                string visualState = value ? DataSource.LoadingActivityState : DataSource.IdleActivityState;
                VisualStateManager.GoToState(this, visualState, /* useTransitions */ true);
            }
        }

        /// <summary>
        /// Raised when data has been loaded by the data source.
        /// </summary>
        public event EventHandler DataLoaded {
            add {
                _dataLoadedEventHandler = (EventHandler)Delegate.Combine(_dataLoadedEventHandler, value);
            }
            remove {
                _dataLoadedEventHandler = (EventHandler)Delegate.Remove(_dataLoadedEventHandler, value);
            }
        }

        /// <summary>
        /// Raised when there is an error during data loading.
        /// </summary>
        public event EventHandler Error {
            add {
                _errorEventHandler = (EventHandler)Delegate.Combine(_errorEventHandler, value);
            }
            remove {
                _errorEventHandler = (EventHandler)Delegate.Remove(_errorEventHandler, value);
            }
        }

        /// <summary>
        /// Raised when data is starting to be loaded by the data source.
        /// </summary>
        public event EventHandler<CancelEventArgs> LoadingData {
            add {
                _loadingDataEventHandler = (EventHandler<CancelEventArgs>)Delegate.Combine(_loadingDataEventHandler, value);
            }
            remove {
                _loadingDataEventHandler = (EventHandler<CancelEventArgs>)Delegate.Remove(_loadingDataEventHandler, value);
            }
        }

        /// <summary>
        /// Clears the currently loaded data.
        /// </summary>
        public void ClearData() {
            Data = null;
        }

        /// <summary>
        /// Notifies the data source to load data.
        /// </summary>
        public void LoadData() {
            if (_dataLoadTimer.IsEnabled) {
                _dataLoadTimer.Stop();
            }

            _dataLoadTimer.Start();
        }

        /// <summary>
        /// Performs the work needed to actually load the data from the underlying source
        /// represented by this data source.
        /// </summary>
        /// <param name="retrieveEstimatedTotalCount">Whether to try and retrieve the estimated total count.</param>
        /// <param name="canceled">Whether the loading has been canceled.</param>
        /// <param name="estimatedTotalCount">The estimate total count of items in the underlying source. -1 if not available.</param>
        /// <returns>The data represented by an IEnumerable or an Async of IEnumerble.</returns>
        protected abstract object LoadDataCore(bool retrieveEstimatedTotalCount, out bool canceled, out int estimatedTotalCount);

        private void LoadDataNow() {
            if (_loadingDataEventHandler != null) {
                CancelEventArgs ce = new CancelEventArgs();
                _loadingDataEventHandler(this, ce);

                if (ce.Cancel) {
                    return;
                }
            }

            Async currentAsyncData = AsyncData;
            if (currentAsyncData != null) {
                AsyncData = null;
                IsLoadingAsyncData = false;
                if (currentAsyncData.CanCancel) {
                    currentAsyncData.Cancel();
                }
            }

            bool canceled;
            int estimatedTotalCount;

            // TODO: Pass in whether we care about estimated total count or not

            object data = LoadDataCore(/* retrieveEstimatedTotalCount */ false, out canceled, out estimatedTotalCount);

            if (canceled) {
                return;
            }

            if (data is Async) {
                Async asyncData = (Async)data;
                asyncData.Completed += OnAsyncDataLoaded;
                if (String.IsNullOrEmpty(asyncData.Message)) {
                    asyncData.Message = DataLoadStatusText;
                }

                IsLoadingAsyncData = true;
                AsyncData = asyncData;
            }
            else {
                IEnumerable enumerableData = data as IEnumerable;
                if ((enumerableData == null) && (data != null)) {
                    enumerableData = new object[] { data };
                }

                OnDataLoaded(enumerableData, estimatedTotalCount);
            }
        }

        private void OnAsyncDataLoaded(object sender, EventArgs e) {
            Async asyncData = AsyncData;

            if (asyncData == sender) {
                IEnumerable enumerableData = null;
                int estimatedTotalCount = -1;

                if (asyncData.IsCanceled == false) {
                    if (asyncData.HasError) {
                        if ((asyncData.IsErrorHandled == false) && (_errorEventHandler != null)) {
                            ErrorEventArgs errorEventArgs = new ErrorEventArgs(asyncData.Error);
                            _errorEventHandler(this, errorEventArgs);

                            if (errorEventArgs.IsHandled) {
                                asyncData.MarkErrorAsHandled();
                            }
                        }
                    }
                    else {
                        Async<IEnumerable> asyncEnumerable = asyncData as Async<IEnumerable>;
                        if (asyncEnumerable != null) {
                            enumerableData = asyncEnumerable.Result;
                        }
                        else if (asyncData is Async<Tuple<IEnumerable, int>>) {
                            Async<Tuple<IEnumerable, int>> asyncEnumerableAndCount = asyncData as Async<Tuple<IEnumerable, int>>;
                            enumerableData = asyncEnumerableAndCount.Result.First;
                            estimatedTotalCount = asyncEnumerableAndCount.Result.Second;
                        }
                        else {
                            if (asyncData.Result != null) {
                                enumerableData = new object[] { asyncData.Result };
                            }
                        }
                    }
                }

                IsLoadingAsyncData = false;
                AsyncData = null;
                OnDataLoaded(enumerableData, estimatedTotalCount);
            }
        }

        private void OnDataLoaded(IEnumerable data, int estimatedTotalCount) {
            Data = data;
            if (data != null) {
                foreach (object o in data) {
                    DataItem = o;
                    break;
                }
            }

            // TODO: Do something with estimatedTotalCount

            if (_dataLoadedEventHandler != null) {
                _dataLoadedEventHandler(this, EventArgs.Empty);
            }

            if (_refreshTimer.Interval != TimeSpan.Zero) {
                _refreshTimer.Start();
            }
        }

        /// <summary>
        /// Performs initialization work once the control has been loaded.
        /// </summary>
        protected virtual void OnLoaded() {
            if (_autoLoadData) {
                LoadDataNow();
            }
        }

        private void OnLoaded(object sender, EventArgs e) {
            _loadDataCommand = new DelegateCommand(LoadData);
            Resources.Add("LoadDataCommand", _loadDataCommand);

            Dispatcher.BeginInvoke(delegate() {
                OnLoaded();
            });
        }

        private void OnDataLoadTimerTick(object sender, EventArgs e) {
            _dataLoadTimer.Stop();
            LoadDataNow();
        }

        private void OnRefreshTimerTick(object sender, EventArgs e) {
            _refreshTimer.Stop();

            if (IsLoadingAsyncData == false) {
                _dataLoadTimer.Stop();
                LoadDataNow();
            }
        }

        /// <summary>
        /// Raises the Error event.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="e">Any exception associated with the error.</param>
        protected void RaiseError(string message, Exception e) {
            Exception error = new Exception(message, e);
            bool handled = false;

            if (_errorEventHandler != null) {
                ErrorEventArgs errorEventArgs = new ErrorEventArgs(error);

                _errorEventHandler(this, errorEventArgs);
                handled = errorEventArgs.IsHandled;
            }

            if (handled == false) {
                throw error;
            }
        }

        #region Implementation of IAsyncControl
        Async IAsyncControl.AsyncActivity {
            get {
                return AsyncData;
            }
        }

        event EventHandler IAsyncControl.AsyncActivityChanged {
            add {
                _asyncActivityChangedHandler = (EventHandler)Delegate.Combine(_asyncActivityChangedHandler, value);
            }
            remove {
                _asyncActivityChangedHandler = (EventHandler)Delegate.Remove(_asyncActivityChangedHandler, value);
            }
        }
        #endregion
    }
}
