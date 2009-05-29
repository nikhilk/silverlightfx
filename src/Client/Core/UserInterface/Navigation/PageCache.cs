// PageCache.cs
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

    internal sealed class PageCache {

        private Dictionary<string, Page> _keepAlivePages;
        private Dictionary<string, Page> _pages;
        private List<string> _keys;
        private int _cacheSize;

        public PageCache(int cacheSize) {
            _cacheSize = Math.Max(10, cacheSize);

            _keepAlivePages = new Dictionary<string, Page>();
            _pages = new Dictionary<string, Page>();
            _keys = new List<string>();
        }

        public void AddPage(Page page, Uri pageUri) {
            if (page.Cache.HasValue && (page.Cache == false)) {
                return;
            }

            string cacheKey = GetCacheKey(pageUri);

            if (page.Cache.HasValue && (page.Cache == true)) {
                _keepAlivePages.Add(cacheKey, page);
                return;
            }

            if (_keys.Count >= _cacheSize) {
                _pages.Remove(_keys[_keys.Count - 1]);
                _keys.RemoveAt(_keys.Count - 1);
            }

            _keys.Insert(0, cacheKey);
            _pages.Add(cacheKey, page);
        }

        private string GetCacheKey(Uri pageUri) {
            string url = pageUri.ToString();
            int fragmentIndex = url.IndexOf('#');

            if (fragmentIndex > 0) {
                url = url.Substring(0, fragmentIndex);
            }
            return url;
        }

        public Page GetPage(Uri uri) {
            string cacheKey = GetCacheKey(uri);
            Page page = null;

            if (_keepAlivePages.TryGetValue(cacheKey, out page)) {
                _keepAlivePages.Remove(cacheKey);
                return page;
            }

            if (_pages.TryGetValue(cacheKey, out page)) {
                _pages.Remove(cacheKey);

                int index = _keys.IndexOf(cacheKey);
                _keys.RemoveAt(index);

                return page;
            }

            return null;
        }
    }
}
