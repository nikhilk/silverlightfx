// DataCommand.cs
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
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace System.Windows.Data {

    internal sealed class DataCommand : DelegateCommand {

        private object _dataItem;
        private MethodInfo _commandMethod;
        private PropertyInfo _canExecuteProperty;

        private DataCommand(object dataItem, MethodInfo commandMethod) {
            _dataItem = dataItem;
            _commandMethod = commandMethod;

            INotifyPropertyChanged propChangeNotifier = dataItem as INotifyPropertyChanged;
            if (propChangeNotifier != null) {
                _canExecuteProperty =
                    _dataItem.GetType().GetProperty("Can" + _commandMethod.Name,
                                                    BindingFlags.FlattenHierarchy |
                                                    BindingFlags.Instance | BindingFlags.Public);
                if (_canExecuteProperty != null) {
                    propChangeNotifier.PropertyChanged += OnDataItemPropertyChanged;
                }
            }
        }

        protected override void Execute(object parameter) {
            ParameterInfo[] parameters = _commandMethod.GetParameters();

            if ((parameters != null) && (parameters.Length != 0)) {
                _commandMethod.Invoke(_dataItem, new object[] { parameter });
            }
            else {
                _commandMethod.Invoke(_dataItem, null);
            }
        }

        public static ICommand GetDataItemCommand(object dataItem, string commandName) {
            if (String.IsNullOrEmpty(commandName)) {
                return null;
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            MethodInfo method = dataItem.GetType().GetMethod(commandName, flags);

            if (method == null) {
                return null;
            }

            return new DataCommand(dataItem, method);
        }

        private void OnDataItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            bool status = (bool)_canExecuteProperty.GetValue(_dataItem, null);
            UpdateStatus(status);
        }
    }
}
