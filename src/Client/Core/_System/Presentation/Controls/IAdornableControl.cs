// IAdornableControl.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Windows.Controls {

    /// <summary>
    /// Represents a control that provides an adornment layer.
    /// </summary>
    public interface IAdornableControl {

        /// <summary>
        /// Gets whether the control can be adorned in its present state.
        /// </summary>
        bool CanAdorn {
            get;
        }

        /// <summary>
        /// Gets whether the control is currently being adorned.
        /// </summary>
        bool HasAdornments {
            get;
        }

        /// <summary>
        /// Adds an adornment to the control.
        /// </summary>
        /// <param name="element">The element representing the adornment.</param>
        void AddAdornment(UIElement element);

        /// <summary>
        /// Removes an adornment from the control.
        /// </summary>
        /// <param name="element">The element representing the adornment.</param>
        void RemoveAdornment(UIElement element);
    }
}
