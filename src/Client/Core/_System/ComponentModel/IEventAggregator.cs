// IEventAggregator.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
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
    /// Provides a simple event aggregation or pub/sub mechanism for allowing
    /// different components to broadcast and listen to messages or events without
    /// being coupled to each other.
    /// </summary>
    public interface IEventAggregator {

        /// <summary>
        /// Broadcasts an event. The event is sequentially handled by all subscribers.
        /// Any errors that occur are ignored.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event argument.</typeparam>
        /// <param name="eventArgs">The data associated with the event.</param>
        void Publish<TEvent>(TEvent eventArgs) where TEvent : EventArgs;

        /// <summary>
        /// Subscribes the specified handler to listen to events of the specified
        /// type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event argument.</typeparam>
        /// <param name="eventHandler">The event handler to be invoked when the event occurs.</param>
        /// <returns>An opaque cookie that can be used to unsubscribe subsequently.</returns>
        object Subscribe<TEvent>(Action<TEvent> eventHandler) where TEvent : EventArgs;

        /// <summary>
        /// Unsubscribes a previous event handler from subsequent events.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event argument.</typeparam>
        /// <param name="subscriptionCookie">The cookie that represents the subscription.</param>
        void Unsubscribe<TEvent>(object subscriptionCookie);
    }
}
