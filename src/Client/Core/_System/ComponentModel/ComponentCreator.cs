// ComponentCreator.cs
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

    internal interface IComponentCreator {
    }

    /// <summary>
    /// Represents a factory that can create object instances as needed.
    /// This is used primarily as a dependency in objects that need to create
    /// another object via the container, without having to take a dependency on the
    /// container itself.
    /// </summary>
    /// <typeparam name="T">The type of objects created by this creator.</typeparam>
    public sealed class ComponentCreator<T> : IComponentCreator where T : class {

        private IComponentContainer _container;

        /// <summary>
        /// Creates and initializes an instance of a ComponentCreator.
        /// </summary>
        /// <param name="container">The container to use to construct the component.</param>
        public ComponentCreator(IComponentContainer container) {
            _container = container;
        }

        /// <summary>
        /// Creates an instance of the component as needed.
        /// </summary>
        /// <returns>The instance created by this creator.</returns>
        T CreateInstance() {
            return _container.GetObject<T>();
        }
    }
}
