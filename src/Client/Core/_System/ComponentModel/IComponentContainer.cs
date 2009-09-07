// IComponentContainer.cs
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
    /// Encapsulates the functionality of a container that defines a scope of
    /// composition where objects can be registered and dependencies can be resolved.
    /// </summary>
    public interface IComponentContainer {

        /// <summary>
        /// Gets an instance of an object for the specified object type.
        /// </summary>
        /// <param name="objectType">The type of object to retrieve.</param>
        /// <returns>The resulting object; null if the object could not be retrieved.</returns>
        object GetObject(Type objectType);

        /// <summary>
        /// Gets an instance of an object for the specified object type.
        /// </summary>
        /// <typeparam name="TObject">The type of object to retrieve.</typeparam>
        /// <returns>The resulting object; null if the object could not be retrieved.</returns>
        TObject GetObject<TObject>();

        /// <summary>
        /// Initializes an object instance by resolving its dependencies against
        /// objects registered in the container.
        /// </summary>
        /// <param name="objectInstance">The object to initialize.</param>
        /// <returns>true if the object could be initialized; false if it couldn't.</returns>
        bool InitializeObject(object objectInstance);

        /// <summary>
        /// Registers an object instance with the container. If the object is marked
        /// with ServiceAttribute metadata, then the object is registered for those
        /// service types; otherwise it is registered as the implementation of its own
        /// type.
        /// </summary>
        /// <param name="objectInstance">The object to register.</param>
        void RegisterObject(object objectInstance);

        /// <summary>
        /// Registers an object instance for the specified type with the container.
        /// </summary>
        /// <param name="objectType">The type of object this instance corresponds to.</param>
        /// <param name="objectInstance">The object to register.</param>
        void RegisterObject(Type objectType, object objectInstance);

        /// <summary>
        /// Registers an object factory for the specified type with the container.
        /// </summary>
        /// <param name="objectType">The type of object this factory corresponds to.</param>
        /// <param name="objectFactory">The factory to register.</param>
        void RegisterFactory(Type objectType, IComponentFactory objectFactory);
    }
}
