// CompletionService.cs
// Copyright (c) Nikhil Kothari, 2008. All Rights Reserved.
// http://www.nikhilk.net
//
// Silverlight.FXis an application framework for building RIAs with Silverlight.
// This project is licensed under the BSD license. See the accompanying License.txt
// file for more information.
// For updated project information please visit http://projects.nikhilk.net/SilverlightFX.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SilverlightFX.Services {

    /// <summary>
    /// This class represents the base class for implementing services that are invoked
    /// to request information about items to show in an AutoComplete UI on the client.
    /// The service is implemented as an HTTP handler (.ashx).
    /// </summary>
    /// <typeparam name="T">The type of items returned from the service.</typeparam>
    public abstract class CompletionService<T> : IHttpHandler {

        /// <summary>
        /// Gets the list of items corresponding to the supplied prefix text that
        /// has been entered by the user in an auto-complete UI.
        /// </summary>
        /// <param name="prefix">The prefix text to be matched.</param>
        /// <returns>The list of matching items.</returns>
        protected abstract IEnumerable<T> GetCompletionItems(string prefix);

        #region Implementation of IHttpHandler
        bool IHttpHandler.IsReusable {
            get {
                return false;
            }
        }

        void IHttpHandler.ProcessRequest(HttpContext context) {
            HttpRequest request = context.Request;
            string prefix = request.QueryString["prefix"];

            if (String.IsNullOrEmpty(prefix)) {
                throw new HttpException(400, "Bad Request");
            }

            T[] items = GetCompletionItems(prefix).ToArray();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseText = serializer.Serialize(items);

            context.Response.ContentType = "text/json";
            context.Response.Write(responseText);
        }
        #endregion
    }
}
