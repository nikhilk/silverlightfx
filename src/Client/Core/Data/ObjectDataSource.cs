// ObjectDataSource.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FX is an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SilverlightFX.Data {

    /// <summary>
    /// An object that provides data to controls within the user interface. It relies
    /// on the associated view model to perform data loading.
    /// </summary>
    public class ObjectDataSource : DataSource {

        private string _queryName;
        private ParameterCollection _queryParameters;

        /// <summary>
        /// Initializes an instance of an ObjectDataSource.
        /// </summary>
        public ObjectDataSource() {
            _queryParameters = new ParameterCollection();
            _queryParameters.ParametersChanged += OnQueryParametersChanged;
        }

        /// <summary>
        /// The name of the method implemented by the view model that is invoked
        /// to load the data.
        /// </summary>
        public string QueryName {
            get {
                return _queryName ?? String.Empty;
            }
            set {
                _queryName = value;
            }
        }

        /// <summary>
        /// The list of parameters used in invoking the query method.
        /// </summary>
        public ParameterCollection QueryParameters {
            get {
                return _queryParameters;
            }
        }

        /// <internalonly />
        protected override object LoadDataCore(bool retrieveEstimatedTotalCount, out bool canceled, out int estimatedTotalCount) {
            canceled = false;
            estimatedTotalCount = -1;

            if (QueryName.Length == 0) {
                return null;
            }

            object model = ViewModelAttribute.GetCurrentViewModel(this);
            if (model == null) {
                return null;
            }

            MethodInfo queryMethod = null;
            try {
                queryMethod = model.GetType().GetMethod(QueryName);
            }
            catch (Exception e) {
                RaiseError("Could not find a method named '" + QueryName + "' on the associated model.", e);
                return null;
            }

            if (queryMethod.GetParameters().Length != QueryParameters.Count) {
                RaiseError("There is a mismatch in paramters on '" + QueryName + "' and the QueryParameters collection.", null);
                return null;
            }

            object[] parameterValues = null;
            int outParameterIndex = -1;
            if (queryMethod.GetParameters().Length != 0) {
                bool ignoreParameters;

                parameterValues = _queryParameters.GetParameterValues(queryMethod, /* honorIgnoredValues */ true,
                                                                      out ignoreParameters,
                                                                      out outParameterIndex);
                if (ignoreParameters) {
                    canceled = true;
                    return null;
                }
            }

            object queryResult = queryMethod.Invoke(model, parameterValues);
            if ((outParameterIndex != -1) && (parameterValues[outParameterIndex] is int)) {
                estimatedTotalCount = (int)parameterValues[outParameterIndex];
            }

            return queryResult;
        }

        /// <internalonly />
        protected override void OnLoaded() {
            _queryParameters.Initialize(this);
            base.OnLoaded();
        }

        private void OnQueryParametersChanged(object sender, EventArgs e) {
            if (AutoLoadData) {
                LoadData();
            }
        }
    }
}
