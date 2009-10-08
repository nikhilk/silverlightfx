// Tuple.cs
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
    /// Provides a set of utility methods for creating tuple instances.
    /// </summary>
    public static class Tuple {

        /// <summary>
        /// Creates a 2-tuple or a pair using the specified values.
        /// </summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <param name="first">The first component of the tuple.</param>
        /// <param name="second">The second component of the tuple.</param>
        /// <returns>The pair.</returns>
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second) {
            return new Tuple<T1, T2>(first, second);
        }

        /// <summary>
        /// Creates a 3-tuple or triple using the specified values.
        /// </summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <param name="first">The first component of the tuple.</param>
        /// <param name="second">The second component of the tuple.</param>
        /// <param name="third">The third component of the tuple.</param>
        /// <returns>The triple.</returns>
        public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third) {
            return new Tuple<T1, T2, T3>(first, second, third);
        }
    }

    /// <summary>
    /// A 2-tuple or pair.
    /// </summary>
    /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
    /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
    public struct Tuple<T1, T2> {

        private T1 _first;
        private T2 _second;

        /// <summary>
        /// Initializes an instance of a pair.
        /// </summary>
        /// <param name="first">The first component of the tuple.</param>
        /// <param name="second">The second component of the tuple.</param>
        public Tuple(T1 first, T2 second) {
            _first = first;
            _second = second;
        }

        /// <summary>
        /// Gets the first component of the tuple.
        /// </summary>
        public T1 First {
            get {
                return _first;
            }
        }

        /// <summary>
        /// Gets the second component of the tuple.
        /// </summary>
        public T2 Second {
            get {
                return _second;
            }
        }
    }

    /// <summary>
    /// A 3-tuple or triple.
    /// </summary>
    /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
    /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
    /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
    public struct Tuple<T1, T2, T3> {

        private T1 _first;
        private T2 _second;
        private T3 _third;

        /// <summary>
        /// Initializes an instance of a triple.
        /// </summary>
        /// <param name="first">The first component of the tuple.</param>
        /// <param name="second">The second component of the tuple.</param>
        /// <param name="third">The third component of the tuple.</param>
        public Tuple(T1 first, T2 second, T3 third) {
            _first = first;
            _second = second;
            _third = third;
        }

        /// <summary>
        /// Gets the first component of the tuple.
        /// </summary>
        public T1 First {
            get {
                return _first;
            }
        }

        /// <summary>
        /// Gets the second component of the tuple.
        /// </summary>
        public T2 Second {
            get {
                return _second;
            }
        }

        /// <summary>
        /// Gets the third component of the tuple.
        /// </summary>
        public T3 Third {
            get {
                return _third;
            }
        }
    }
}
