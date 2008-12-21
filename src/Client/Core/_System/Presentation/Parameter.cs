// Parameter.cs
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

namespace System.Windows {

    /// <summary>
    /// Represents the base class for declarative parameters.
    /// </summary>
    public abstract class Parameter {

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        /// <param name="element">The element that the action is associated with.</param>
        /// <returns>The value of the parameter.</returns>
        public abstract object GetValue(FrameworkElement element);
    }
}
