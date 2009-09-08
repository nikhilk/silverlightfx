// ComponentFactory.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SilverlightFX.Applications {

    /// <summary>
    /// Represents a factory that can create instances of the specified component type.
    /// </summary>
    public sealed class ComponentFactory : IGenericComponentFactory {

        private Type _componentType;
        private bool _singleton;

        /// <summary>
        /// Gets or sets the type of the component represented by this ComponentFactory.
        /// </summary>
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type ComponentType {
            get {
                return _componentType;
            }
            set {
                _componentType = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this factory should create a single instance of its
        /// associated component type.
        /// </summary>
        public bool IsSingleton {
            get {
                return _singleton;
            }
            set {
                _singleton = value;
            }
        }

        #region Implementation of IComponentCreator
        object IComponentFactory.CreateInstance(Type componentType, IComponentContainer container, out bool isSingleInstance) {
            if (_componentType == null) {
                throw new InvalidOperationException("The ComponentType of a ComponentFactory must be set.");
            }

            Debug.Assert(_componentType == componentType);

            isSingleInstance = _singleton;
            return Activator.CreateInstance(_componentType);
        }
        #endregion

        #region Implementation of IGenericComponentCreator
        Type IGenericComponentFactory.ComponentType {
            get {
                if (_componentType == null) {
                    throw new InvalidOperationException("The ComponentType of a ComponentFactory must be set.");
                }

                return _componentType;
            }
        }
        #endregion
    }
}
