// ControllerDescriptor.cs
// Copyright (c) Nikhil Kothari, 2009. All Rights Reserved.
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

namespace System.ComponentModel.Navigation {

    internal sealed class ControllerDescriptor {

        private static readonly Dictionary<Type, ControllerDescriptor> _controllerDescriptors
            = new Dictionary<Type,ControllerDescriptor>();

        private Dictionary<string, ActionDescriptor> _actions;

        private ControllerDescriptor() {
            _actions = new Dictionary<string, ActionDescriptor>();
        }

        public static ActionDescriptor GetAction(Controller controller, string actionName) {
            ControllerDescriptor controllerDescriptor = GetDescriptor(controller.GetType());
            ActionDescriptor actionDescriptor;

            controllerDescriptor._actions.TryGetValue(actionName, out actionDescriptor);
            return actionDescriptor;
        }

        private static ControllerDescriptor GetDescriptor(Type controllerType) {
            ControllerDescriptor descriptor;
            if (_controllerDescriptors.TryGetValue(controllerType, out descriptor)) {
                return descriptor;
            }

            descriptor = new ControllerDescriptor();

            Type actionResultType = typeof(ActionResult);
            Type taskActionResultType = typeof(Task<ActionResult>);
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance |
                                        BindingFlags.FlattenHierarchy;

            MethodInfo[] methods = controllerType.GetMethods(bindingFlags);
            foreach (MethodInfo method in methods) {
                ActionDescriptor action = null;
                if (method.ReturnType.IsAssignableFrom(taskActionResultType)) {
                    action = new ActionDescriptor(method, /* async */ true);
                }
                else if (method.ReturnType.IsAssignableFrom(actionResultType)) {
                    action = new ActionDescriptor(method, /* async */ false);
                }
                else {
                    continue;
                }

                string actionName = method.Name;
                if (descriptor._actions.ContainsKey(actionName)) {
                    throw new InvalidOperationException(controllerType.Name + " has a duplicate action named " + actionName);
                }
                descriptor._actions[actionName] = action;
            }

            _controllerDescriptors[controllerType] = descriptor;
            return descriptor;
        }
    }
}
