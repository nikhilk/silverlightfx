// IController.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace System.ComponentModel.Navigation {

    /// <summary>
    /// Represents the public view of a controller that allows the consumer to invoke
    /// actions defined on the controller.
    /// </summary>
    public interface IController {

        /// <summary>
        /// Executes an action method, as specified in the supplied invocation. If the action is
        /// completed synchronously, the callback is invoked immediately.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        /// <param name="callback">The async callback to invoke on completion.</param>
        /// <param name="asyncState">Any state to pass to the the callback.</param>
        /// <returns></returns>
        IAsyncResult BeginExecute(ActionInvocation action, AsyncCallback callback, object asyncState);

        /// <summary>
        /// Completes the execution of the currently invoked action method.
        /// </summary>
        /// <param name="asyncResult">The async result from starting the invokation.</param>
        /// <returns>The result of invoking the action.</returns>
        ActionResult EndExecute(IAsyncResult asyncResult);
    }
}
