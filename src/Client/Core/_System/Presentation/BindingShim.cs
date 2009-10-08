// BoundParameter.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Windows.Data;

namespace System.Windows {

    internal sealed class BindingShim : IDisposable {

        private static int _instanceCount;

        private FrameworkElement _element;
        private Binding _binding;
        private object _value;
        private DependencyProperty _attachedDP;
        private Action _changedHandler;

        public BindingShim(FrameworkElement element, Binding binding, Action changedHandler) {
            _element = element;
            _binding = binding;

            _instanceCount++;
            _attachedDP = DependencyProperty.RegisterAttached("BoundParameter" + _instanceCount,
                                                              typeof(object), typeof(BindingShim),
                                                              new PropertyMetadata(OnChanged));

            _element.SetBinding(_attachedDP, _binding);
            _value = _element.GetValue(_attachedDP);

            // Initialize _changedHandler after setting up the binding so we don't invoke
            // it when the OnChanged handled gets called as a side-effect of setting the binding!
            _changedHandler = changedHandler;
        }

        public object Value {
            get {
                return _value;
            }
        }

        public void Dispose() {
            _element.ClearValue(_attachedDP);
            _element = null;
        }

        private void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            _value = e.NewValue;
            if (_changedHandler != null) {
                _changedHandler();
            }
        }
    }
}
