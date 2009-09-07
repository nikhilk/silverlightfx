// WeakDelegateReference.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Reflection;

namespace SilverlightFX.Applications {

    internal sealed class WeakDelegateReference {

        private readonly WeakReference _weakReference;
        private readonly MethodInfo _method;
        private readonly Type _delegateType;

        public WeakDelegateReference(Delegate @delegate) {
            _weakReference = new WeakReference(@delegate.Target);
            _method = @delegate.Method;
            _delegateType = @delegate.GetType();
        }

        public Delegate Delegate {
            get {
                if (_method.IsStatic) {
                    return Delegate.CreateDelegate(_delegateType, null, _method);
                }

                object target = _weakReference.Target;
                if (target != null) {
                    return Delegate.CreateDelegate(_delegateType, target, _method);
                }
                return null;
            }
        }
    }
}
