// LayoutEasing.cs
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
    /// Determines how the layout is progressed over time.
    /// </summary>
    public enum LayoutEasing {

        /// <summary>
        /// Linearly interpolates the layout from start to finish.
        /// </summary>
        None,

        /// <summary>
        /// Accelerates the layout at start and then linearly interpolates
        /// the layout to finish.
        /// </summary>
        QuadraticIn,

        /// <summary>
        /// Decelerates the layout at finish after linearly interpolating
        /// the layout from start.
        /// </summary>
        QuadraticOut,

        /// <summary>
        /// Accelerates the layout at start, then linearly interpolates
        /// the layout toward the finish, and finally decelerates to the
        /// finishing point.
        /// </summary>
        QuadraticInOut,

        /// <summary>
        /// Bounces the layout at the start, and then interpolates the
        /// layout toward the finish.
        /// </summary>
        BounceIn,

        /// <summary>
        /// Starts the layout and bounces it toward the finish at the end.
        /// </summary>
        BounceOut,

        /// <summary>
        /// Bounces the layout at the start and toward the finish.
        /// </summary>
        BounceInOut,

        /// <summary>
        /// Starts the layout by going in reverse direction, and then proceeds
        /// towards the finish.
        /// </summary>
        BackIn,

        /// <summary>
        /// Starts the layout and goes beyond the finish and then back to finish.
        /// </summary>
        BackOut,

        /// <summary>
        /// Starts the layout by going back, then going beyond the finish, and then
        /// back to finish.
        /// </summary>
        BackInOut,

        /// <summary>
        /// Starts the layout by going back and forth at the start and then
        /// shooting toward the finish.
        /// </summary>
        ElasticIn,

        /// <summary>
        /// Starts the layout by shooting toward the finish, and then ending by
        /// going back and forth.
        /// </summary>
        ElasticOut,

        /// <summary>
        /// Starts the layout by going back and forth at the start, then shooting
        /// toward the finish, and ending by going back and forth.
        /// </summary>
        ElasticInOut
    }
}
