// UriData.cs
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

namespace SilverlightFX.UserInterface.Navigation {

    internal sealed class UriData {

        private static readonly Uri DummyBaseUri = new Uri("http://server/", UriKind.Absolute);

        private Uri _originalUri;
        private UriBuilder _uriBuilder;

        public UriData(Uri originalUri) {
            _originalUri = originalUri;

            Uri fullUri = new Uri(DummyBaseUri, originalUri);
            _uriBuilder = new UriBuilder(fullUri);
        }

        public Uri OriginalUri {
            get {
                return _originalUri;
            }
        }

        public Uri GetUri() {
            Uri currentUri = _uriBuilder.Uri;
            string relativeUrl = currentUri.ToString().Substring(13);

            return new Uri(relativeUrl, UriKind.Relative);
        }

        public IDictionary<string, string> GetQueryString() {
            string queryString = _uriBuilder.Query;
            if (queryString.Length <= 1) {
                return null;
            }

            queryString = queryString.Substring(1);

            Dictionary<string, string> items = new Dictionary<string, string>();

            string[] nameValuePairs = queryString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < nameValuePairs.Length; i++) {
                string[] parts = nameValuePairs[i].Split('=');

                if (parts.Length == 1) {
                    items[parts[0]] = String.Empty;
                }
                else {
                    items[parts[0]] = parts[1];
                }
            }

            return items;
        }

        public IList<string> GetPath() {
            string path = _uriBuilder.Path;
            if (path.Equals("/", StringComparison.Ordinal)) {
                return null;
            }

            return new List<string>(path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public void SetFragment(string fragment) {
            _uriBuilder.Fragment = fragment;
        }

        public bool TryGetFragment(out string fragment) {
            string currentFragment = _uriBuilder.Fragment;
            if (currentFragment.Length > 1) {
                fragment = currentFragment.Substring(1);
                return true;
            }

            fragment = null;
            return false;
        }
    }
}
