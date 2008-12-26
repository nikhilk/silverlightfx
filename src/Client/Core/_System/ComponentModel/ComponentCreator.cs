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

    /// <summary>
    /// Represents a factory method that can create object instances as needed.
    /// </summary>
    /// <param name="objectType">The type of object to create.</param>
    /// <param name="container">The container that is requesting the creation of the object.</param>
    /// <param name="isSingleInstance">Whether the returned object is to be treated as a singleton.</param>
    /// <returns></returns>
    public delegate object ComponentCreator(Type objectType, IComponentContainer container, out bool isSingleInstance);
}
