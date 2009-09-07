// IUserNotificationService.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;

namespace SilverlightFX.Applications {

    /// <summary>
    /// Provides the ability to notify the user with a message.
    /// </summary>
    public interface IUserNotificationService {

        /// <summary>
        /// Shows the specified message to the user.
        /// </summary>
        /// <param name="message">The text of the notification.</param>
        /// <param name="caption">The caption of the notification.</param>
        void ShowMessage(string message, string caption);

        /// <summary>
        /// Shows the specified message to the user, and returns whether the
        /// user chose to commit or cancel the request.
        /// </summary>
        /// <param name="message">The text of the notification.</param>
        /// <param name="caption">The caption of the notification.</param>
        /// <returns>true if the user committed; false otherwise.</returns>
        bool ShowPrompt(string message, string caption);
    }
}
