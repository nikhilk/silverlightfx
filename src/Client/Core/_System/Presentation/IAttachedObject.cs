// IAttachedObject.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Windows {

    /// <summary>
    /// Represents an object that can be attached to another DependencyObject.
    /// </summary>
    public interface IAttachedObject {

        /// <summary>
        /// Gets the DependencyObject that this object is attached to.
        /// </summary>
        DependencyObject AssociatedObject {
            get;
        }

        /// <summary>
        /// Attaches the object to the specified DependencyObject.
        /// </summary>
        /// <param name="associatedObject">The associated DepedencyObject.</param>
        void Attach(DependencyObject associatedObject);

        /// <summary>
        /// Detaches the object from its associated DependencyObject.
        /// </summary>
        void Detach();
    }
}
