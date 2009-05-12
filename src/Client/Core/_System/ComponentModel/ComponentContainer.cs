// ComponentContainer.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.ComponentModel {

    /// <summary>
    /// Encapsulates a scope allowing registering of objects and the facility to create object
    /// instances whose dependencies are resolved against the registered objects.
    /// </summary>
    public sealed class ComponentContainer : IComponentContainer {

        private IServiceProvider _serviceProvider;
        private Dictionary<Type, object> _registeredTypes;

        /// <summary>
        /// Creates an instance of a CompositionContainer.
        /// </summary>
        public ComponentContainer()
            : this(null) {
        }

        /// <summary>
        /// Creates an instance of a CompositionContainer that automatically
        /// provides access to any of the services accessible via the specified
        /// service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider providing access to inherited services.</param>
        public ComponentContainer(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;

            _registeredTypes = new Dictionary<Type, object>();
            _registeredTypes[typeof(IComponentContainer)] = this;
        }

        private object CreateObject(Type objectType) {
            ConstructorInfo[] ctors = objectType.GetConstructors();
            if ((ctors == null) || (ctors.Length == 0)) {
                return null;
            }

            ConstructorInfo selectedCtor = null;

            for (int i = 0; i < ctors.Length; i++) {
                ConstructorInfo ci = ctors[i];
                ParameterInfo[] parameters = ci.GetParameters();

                object[] attrs = ci.GetCustomAttributes(typeof(DependencyAttribute), /* inherit */ false);
                if ((attrs != null) && (attrs.Length != 0)) {
                    selectedCtor = ci;
                    break;
                }

                if (selectedCtor == null) {
                    selectedCtor = ci;
                }
                else if (selectedCtor.GetParameters().Length < parameters.Length) {
                    selectedCtor = ci;
                }
            }

            if (selectedCtor != null) {
                ParameterInfo[] parameters = selectedCtor.GetParameters();
                object[] paramValues = null;

                if (parameters.Length != 0) {
                    paramValues = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++) {
                        object paramValue = GetObject(parameters[i].ParameterType);
                        if (paramValue == null) {
                            bool optional = false;

                            object[] attrs = parameters[i].GetCustomAttributes(typeof(DependencyAttribute), /* inherit */ false);
                            if ((attrs != null) && (attrs.Length != 0)) {
                                DependencyAttribute dependency = (DependencyAttribute)attrs[0];
                                optional = dependency.Optional;
                            }

                            if (optional == false) {
                                return null;
                            }
                        }

                        paramValues[i] = paramValue;
                    }
                }

                return selectedCtor.Invoke(paramValues);
            }

            return null;
        }

        private object GetObject(Type objectType) {
            object instance;
            if (_registeredTypes.TryGetValue(objectType, out instance)) {
                IComponentCreator creator = instance as IComponentCreator;
                if (creator != null) {
                    bool singleInstance = false;
                    instance = creator.CreateInstance(objectType, this, out singleInstance);

                    if (instance != null) {
                        if (((IComponentContainer)this).InitializeObject(instance)) {
                            if (singleInstance) {
                                _registeredTypes[objectType] = instance;
                            }
                        }
                        else {
                            instance = null;
                        }
                    }
                }
            }
            else {
                if (_serviceProvider != null) {
                    instance = _serviceProvider.GetService(objectType);
                }

                if (instance == null) {
                    instance = CreateObject(objectType);
                    if (((IComponentContainer)this).InitializeObject(instance) == false) {
                        instance = null;
                    }
                }
            }

            return instance;
        }

        #region Implementation of ICompositionContainer
        object IComponentContainer.GetObject(Type objectType) {
            if (objectType == null) {
                throw new ArgumentNullException("objectType");
            }

            return GetObject(objectType);
        }

        TObject IComponentContainer.GetObject<TObject>() {
            return (TObject)GetObject(typeof(TObject));
        }

        bool IComponentContainer.InitializeObject(object objectInstance) {
            if (objectInstance == null) {
                throw new ArgumentNullException("objectInstance");
            }

            Type objectType = objectInstance.GetType();
            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if ((properties == null) || (properties.Length == 0)) {
                return true;
            }

            foreach (PropertyInfo pi in properties) {
                if (pi.GetSetMethod() == null) {
                    continue;
                }

                object[] attrs = pi.GetCustomAttributes(typeof(DependencyAttribute), /* inherit */ true);
                if ((attrs == null) || (attrs.Length == 0)) {
                    continue;
                }

                object propertyValue = ((IComponentContainer)this).GetObject(pi.PropertyType);
                if (propertyValue != null) {
                    pi.SetValue(objectInstance, propertyValue, null);
                }
                else {
                    DependencyAttribute dependency = (DependencyAttribute)attrs[0];
                    if (dependency.Optional == false) {
                        return false;
                    }
                }
            }

            return true;
        }

        void IComponentContainer.RegisterObject(object objectInstance) {
            if (objectInstance == null) {
                throw new ArgumentNullException("objectInstance");
            }

            Type objectType = objectInstance.GetType();
            object[] attrs = objectType.GetCustomAttributes(typeof(ServiceAttribute), /* inherit */ true);

            if ((attrs != null) && (attrs.Length != 0)) {
                for (int i = 0; i < attrs.Length; i++) {
                    ServiceAttribute service = (ServiceAttribute)attrs[i];

                    if (objectInstance is IComponentCreator) {
                        ((IComponentContainer)this).RegisterCreator(service.ServiceType, (IComponentCreator)objectInstance);
                    }
                    else {
                        ((IComponentContainer)this).RegisterObject(service.ServiceType, objectInstance);
                    }
                }
            }
            else {
                ((IComponentContainer)this).RegisterObject(objectType, objectInstance);
            }
        }

        void IComponentContainer.RegisterObject(Type objectType, object objectInstance) {
            if (objectType == null) {
                throw new ArgumentNullException("objectType");
            }
            if (objectInstance == null) {
                throw new ArgumentNullException("objectInstance");
            }

            _registeredTypes[objectType] = objectInstance;
        }

        void IComponentContainer.RegisterCreator(Type objectType, IComponentCreator objectCreator) {
            if (objectType == null) {
                throw new ArgumentNullException("objectType");
            }
            if (objectCreator == null) {
                throw new ArgumentNullException("objectCreator");
            }

            _registeredTypes[objectType] = objectCreator;
        }
        #endregion
    }
}
