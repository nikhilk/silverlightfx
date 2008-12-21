// Model.cs
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
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;

namespace System.ComponentModel {

    /// <summary>
    /// Provides a base class for implementing models that encapsulate
    /// data and behavior that is independent of the presentation.
    /// </summary>
    [DataContract]
    public abstract class Model : INotifyPropertyChanged {

        private static readonly Dictionary<string, PropertyChangedEventArgs> _eventArgsMap =
            new Dictionary<string, PropertyChangedEventArgs>();

        private PropertyChangedEventHandler _propChangedHandler;
        private SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes an instance of a Model.
        /// </summary>
        protected Model() {
            _syncContext = SynchronizationContext.Current;
        }

        private static PropertyChangedEventArgs GetEventArgs(string propertyName) {
            PropertyChangedEventArgs pe = null;
            if (_eventArgsMap.TryGetValue(propertyName, out pe) == false) {
                pe = new PropertyChangedEventArgs(propertyName);
                _eventArgsMap[propertyName] = pe;
            }

            return pe;
        }

        /// <summary>
        /// Raises a change notification event to signal a change in the
        /// specified property's value.
        /// </summary>
        /// <param name="propertyName">The property that has changed.</param>
        protected void RaisePropertyChanged(string propertyName) {
            if (String.IsNullOrEmpty(propertyName)) {
                throw new ArgumentNullException("propertyName");
            }

            if (_propChangedHandler == null) {
                return;
            }

            if (_syncContext != null) {
                _syncContext.Post(delegate(object state) {
                    if (_propChangedHandler != null) {
                        _propChangedHandler(this, GetEventArgs(propertyName));
                    }
                }, null);
            }
            else {
                _propChangedHandler(this, GetEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises a change notification event to signal a change in the
        /// specified properties.
        /// </summary>
        /// <param name="propertyNames">The properties that have changed.</param>
        protected void RaisePropertyChanged(params string[] propertyNames) {
            if ((propertyNames == null) || (propertyNames.Length == 0)) {
                throw new ArgumentNullException("propertyNames");
            }
            if (_propChangedHandler == null) {
                return;
            }

            if (_syncContext != null) {
                _syncContext.Post(delegate(object state) {
                    if (_propChangedHandler != null) {
                        foreach (string propertyName in propertyNames) {
                            _propChangedHandler(this, GetEventArgs(propertyName));
                        }
                    }
                }, null);
            }
            else {
                foreach (string propertyName in propertyNames) {
                    _propChangedHandler(this, GetEventArgs(propertyName));
                }
            }
        }

        #region Implementation of INotifyPropertyChanged
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
            add {
                _propChangedHandler = (PropertyChangedEventHandler)Delegate.Combine(_propChangedHandler, value);
            }
            remove {
                if (_propChangedHandler != null) {
                    _propChangedHandler = (PropertyChangedEventHandler)Delegate.Remove(_propChangedHandler, value);
                }
            }
        }
        #endregion
    }
}
