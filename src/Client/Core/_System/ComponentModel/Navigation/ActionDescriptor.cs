// ActionDescriptor.cs
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
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Navigation {

    // TODO: Handle Action Filters

    internal sealed class ActionDescriptor {

        private MethodInfo _actionMethod;
        private bool _async;

        public ActionDescriptor(MethodInfo actionMethod, bool async) {
            _actionMethod = actionMethod;
            _async = async;
        }

        public bool IsAsync {
            get {
                return _async;
            }
        }

        public object Invoke(Controller controller, ActionInvocation action) {
            ParameterInfo[] parameters = _actionMethod.GetParameters();
            object[] parameterValues = null;

            if (parameters.Length != 0) {
                List<object> parameterList = new List<object>(parameters.Length);

                int paramIndex = 0;
                for (; paramIndex < action.Parameters.Count && paramIndex < parameters.Length; paramIndex++) {
                    parameterList.Add(action.Parameters[paramIndex]);
                }

                for (; paramIndex < parameters.Length; paramIndex++) {
                    string paramName = parameters[paramIndex].Name;
                    string paramValue = null;

                    action.NamedParameters.TryGetValue(paramName, out paramValue);
                    parameterList.Add(paramValue);
                }

                for (int i = 0; i < parameterList.Count; i++) {
                    Type parameterType = parameters[i].ParameterType;

                    object paramValue = parameterList[i];
                    if ((paramValue == null) || (parameterType.IsAssignableFrom(paramValue.GetType()) == false)) {
                        paramValue = Convert.ChangeType(paramValue, parameterType, CultureInfo.CurrentCulture);
                        parameterList[i] = paramValue;
                    }
                }

                parameterValues = parameterList.ToArray();
            }

            return _actionMethod.Invoke(controller, parameterValues);
        }
    }
}
