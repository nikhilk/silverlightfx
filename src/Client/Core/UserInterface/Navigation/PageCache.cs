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

        public void AddPageReference(Page page) {
            if (page.KeepAlive == false) {
                return;
            }

            string cacheKey = GetCacheKey(page.Uri);

            if ((_pages.ContainsKey(cacheKey) == false) &&
                (_keepAlivePages.ContainsKey(cacheKey) == false)) {
                if (_keys.Count >= _cacheSize) {
                    string evictCacheKey = _keys[_keys.Count - 1];

                    _pages.Remove(evictCacheKey);
                }

                _pages[cacheKey] = page;
                _keys.Add(cacheKey);
            }
        }

        private string GetCacheKey(Uri pageUri) {
            return pageUri.ToString();
        }

        public Page GetPage(Uri uri) {
            string cacheKey = GetCacheKey(uri);
            Page page = null;

            if (_keepAlivePages.TryGetValue(cacheKey, out page)) {
                _keepAlivePages.Remove(cacheKey);
                return page;
            }

            if (_pages.TryGetValue(cacheKey, out page)) {
                int index = _keys.IndexOf(cacheKey);
                if (index != 0) {
                    for (int i = index; i > 0; i--) {
                        _keys[i] = _keys[i - 1];
                    }
                    _keys[0] = cacheKey;
                }

                return page;
            }

            return null;
        }

        public void RemovePageReference(Page page) {
            string cacheKey = GetCacheKey(page.Uri);

            if (page.KeepAlive == true) {
                if (_pages.ContainsKey(cacheKey)) {
                    _pages.Remove(cacheKey);
                    _keys.Remove(cacheKey);
                }

                _keepAlivePages[cacheKey] = page;
            }
        }
    }
}
