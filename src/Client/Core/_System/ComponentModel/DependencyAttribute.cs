// DependencyAttribute.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.ComponentModel {

    /// <summary>
    /// This attribute can be placed on a property, parameter or constructor.
    /// When placed on a property or parameter this can be used to mark a dependency
    /// and whether it is optional or not.
    /// When placed on a constructor, it marks the constructor as the one to use
    /// to inject dependencies if the object has more than one constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Constructor, Inherited = true, AllowMultiple = false)]
    public sealed class DependencyAttribute : Attribute {

        private bool _optional;

        /// <summary>
        /// Gets or sets whether the dependency is optional.
        /// </summary>
        public bool Optional {
            get {
                return _optional;
            }
            set {
                _optional = value;
            }
        }
    }
}
