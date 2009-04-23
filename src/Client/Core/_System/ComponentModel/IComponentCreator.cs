// IComponentCreator.cs
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
    /// Represents a factory that can create object instances as needed.
    /// </summary>
    public interface IComponentCreator {

        /// <summary>
        /// Creates an instance of the component as needed.
        /// </summary>
        /// <param name="componentType">The type of component to be created.</param>
        /// <param name="container">The container that is requesting the creation of the object.</param>
        /// <param name="isSingleInstance">Whether the instance is to be treated as a singleton.</param>
        /// <returns>The instance created by this factory.</returns>
        object CreateInstance(Type componentType, IComponentContainer container, out bool isSingleInstance);
    }
}
