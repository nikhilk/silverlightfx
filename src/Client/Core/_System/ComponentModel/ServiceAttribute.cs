// ServiceAttribute.cs
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
    /// This attribute can be placed on a class to mark it as an implementation of the
    /// specified service type.
    /// If the class implements IComponentFactory, it is considered to be a factory of
    /// the specified service type, rather than the implementation itself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class ServiceAttribute : Attribute {

        private Type _serviceType;

        /// <summary>
        /// Creates an instance of ServiceAttribute.
        /// </summary>
        /// <param name="serviceType">The type of service this attribute represents.</param>
        public ServiceAttribute(Type serviceType) {
            _serviceType = serviceType;
        }

        /// <summary>
        /// The type of service associated with this attribute.
        /// </summary>
        public Type ServiceType {
            get {
                return _serviceType;
            }
        }
    }
}
