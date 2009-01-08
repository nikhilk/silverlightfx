// EffectComposition.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.UserInterface.Effects {

    /// <summary>
    /// Represents how a set of effects are composed together.
    /// </summary>
    public enum EffectComposition {

        /// <summary>
        /// Runs the set of effects one after another.
        /// </summary>
        Sequence = 0,

        /// <summary>
        /// Runs the set of effects together at the same time.
        /// </summary>
        Parallel = 1
    }
}
