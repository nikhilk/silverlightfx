// IScriptExpressionNameResolver.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
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
    /// Resolves name references into object instances when executing a ScriptExpression.
    /// </summary>
    public interface IScriptExpressionNameResolver {

        /// <summary>
        /// Resolves the specified name within a ScriptExpression into an object instance.
        /// </summary>
        /// <param name="name">The name to resolve.</param>
        /// <returns>The resolved object instance.</returns>
        object ResolveName(string name);
    }
}
