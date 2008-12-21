// EffectDirection.cs
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
    /// Determines how the animation is progressed over time.
    /// </summary>
    public enum EffectEasing {

        /// <summary>
        /// Linearly interpolates the animaton from start to finish.
        /// </summary>
        None,

        /// <summary>
        /// Accelerates the animation at start and then linearly interpolates
        /// the animation to finish.
        /// </summary>
        QuadraticIn,

        /// <summary>
        /// Decelerates the animation at finish after linearly interpolating
        /// the animation from start.
        /// </summary>
        QuadraticOut,

        /// <summary>
        /// Accelerates the animation at start, then linearly interpolates
        /// the animation toward the finish, and finally decelerates to the
        /// finishing point.
        /// </summary>
        QuadraticInOut,

        /// <summary>
        /// Bounces the animation at the start, and then interpolates the
        /// animation toward the finish.
        /// </summary>
        BounceIn,

        /// <summary>
        /// Starts the animation and bounces it toward the finish at the end.
        /// </summary>
        BounceOut,

        /// <summary>
        /// Bounces the animation at the start and toward the finish.
        /// </summary>
        BounceInOut,

        /// <summary>
        /// Starts the animation by going in reverse direction, and then proceeds
        /// towards the finish.
        /// </summary>
        BackIn,

        /// <summary>
        /// Starts the animation and goes beyond the finish and then back to finish.
        /// </summary>
        BackOut,

        /// <summary>
        /// Starts the animation by going back, then going beyond the finish, and then
        /// back to finish.
        /// </summary>
        BackInOut,

        /// <summary>
        /// Starts the animation by going back and forth at the start and then
        /// shooting toward the finish.
        /// </summary>
        ElasticIn,

        /// <summary>
        /// Starts the animation by shooting toward the finish, and then ending by
        /// going back and forth.
        /// </summary>
        ElasticOut,

        /// <summary>
        /// Starts the animation by going back and forth at the start, then shooting
        /// toward the finish, and ending by going back and forth.
        /// </summary>
        ElasticInOut
    }
}
