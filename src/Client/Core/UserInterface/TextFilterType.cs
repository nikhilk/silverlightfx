// TextFilterType.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.UserInterface {

    /// <summary>
    /// The type of filters that can be applied to a TextBox.
    /// </summary>
    public enum TextFilterType {

        /// <summary>
        /// No filtering is done.
        /// </summary>
        None = 0,

        /// <summary>
        /// Input is restricted to numbers.
        /// </summary>
        Numbers = 1,

        /// <summary>
        /// Input is restricted to alphabets (A-Z and a-z).
        /// </summary>
        Alphabets = 2
    }
}
