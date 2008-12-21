// DataboundControl.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//              a license identical to this one.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Data {

    /// <summary>
    /// A base class for writing data-bound controls. This provides a data source property to
    /// all derived controls.
    /// </summary>
    public abstract class DataboundControl : Control {

        /// <summary>
        /// Represents the DataSource property on a DataboundControl.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(object), typeof(DataboundControl),
                                        new PropertyMetadata(OnDataSourcePropertyChanged));

        private DataList _dataList;

        /// <summary>
        /// Initializes an instance of a DataboundControl.
        /// </summary>
        internal DataboundControl() {
            Loaded += OnLoaded;
        }

        /// <summary>
        /// The data that this control is bound to currently.
        /// </summary>
        protected DataList DataList {
            get {
                return _dataList;
            }
            private set {
                OnDataListChanging();
                _dataList = value;
                OnDataListChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data source to display within this control.
        /// </summary>
        public object DataSource {
            get {
                return GetValue(DataSourceProperty);
            }
            set {
                SetValue(DataSourceProperty, value);
            }
        }

        /// <summary>
        /// Create a new DataList that abstracts the specified underlying data.
        /// </summary>
        /// <param name="data">The specified underlying data.</param>
        /// <returns>The DataView abstracting the specified data.</returns>
        protected virtual DataList CreateDataList(IEnumerable data) {
            return new DataList(data);
        }

        /// <summary>
        /// Indicates that the data associated with the control is changing.
        /// </summary>
        protected virtual void OnDataListChanging() {
        }

        /// <summary>
        /// Indicates that the data associated with the control has changed.
        /// </summary>
        protected virtual void OnDataListChanged() {
        }

        private static void OnDataSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            DataboundControl control = (DataboundControl)o;

            DataList dataList = e.NewValue as DataList;
            if ((dataList == null) && (e.NewValue != null)) {
                IEnumerable enumerableData = e.NewValue as IEnumerable;
                if (enumerableData == null) {
                    enumerableData = new object[] { e.NewValue };
                }
                dataList = control.CreateDataList(enumerableData);
            }

            control.DataList = dataList;
        }

        /// <summary>
        /// Indicates that the control has been loaded.
        /// </summary>
        /// <param name="e">The information associated with the Loaded event.</param>
        protected virtual void OnLoaded(RoutedEventArgs e) {
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            // The reason why we're invoking is to ensure we do all work after
            // the rest of the UI is loaded (to allow for cross-control dependencies)
            // and to allow the app developer to set additional properties in their
            // Loaded handler.

            Dispatcher.BeginInvoke(delegate() {
                OnLoaded(e);
            });
        }
    }
}
