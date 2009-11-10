// FormatType.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.Data {

    /// <summary>
    /// The type of formatting to be used for formatting an object into a string
    /// representation.
    /// </summary>
    public enum FormatType {

        /// <summary>
        /// Represents positional tokens, with {0} representing the object.
        /// </summary>
        NumberedTokens = 0,

        /// <summary>
        /// Represents named tokens. The names in the format string are used to
        /// lookup properties on the object being formatted. The special {0} token
        /// represents the object itself.
        /// </summary>
        NamedTokens = 1
    }
}
