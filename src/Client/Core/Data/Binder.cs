// Binder.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.ComponentModel;

namespace SilverlightFX.Data {

    /// <summary>
    /// An object that can be used to simulate element-to-element binding in
    /// Silverlight. This object is placed within resources and both elements
    /// are then bound to this object's Value property.
    /// </summary>
    public sealed class Binder : Model {

        private object _value;

        /// <summary>
        /// The value to be passed from one element to another during binding.
        /// </summary>
        public object Value {
            get {
                return _value;
            }
            set {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }
    }
}
