// ProceduralAnimationEasingFunction.cs
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

namespace System.Windows.Media.Glitz {

    /// <summary>
    /// A delegate used to define an easing function that can be used to
    /// vary the otherwise normal linear progression of an animation from
    /// its start state to end state.
    /// An easing function is basically f(t) where t = [0...1] such that
    /// it is continuous and f(0) == 0 and f(1) == 1.
    /// Easing functions can be used to add the illusion of acceleration
    /// and deceleration, as well as other sophisticated movement patterns.
    /// </summary>
    /// <param name="t">The current progress value derived from linear progression.</param>
    /// <returns>The modified value to use as the new progress value.</returns>
    public delegate double ProceduralAnimationEasingFunction(double t);
}
