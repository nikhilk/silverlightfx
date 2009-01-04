// IProceduralAnimationFactory.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// Provides the ability to create animation instances.
    /// </summary>
    public interface IProceduralAnimationFactory {

        /// <summary>
        /// Creates a new animation instance defined by the object.
        /// </summary>
        /// <returns>An animation instance that can be played.</returns>
        ProceduralAnimation CreateAnimation();
    }
}
