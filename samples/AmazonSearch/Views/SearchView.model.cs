// SearchView.model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using AmazonSearch.Data;

namespace AmazonSearch.Views {

    public class SearchViewModel : Model {

        private ICatalog _catalog;

        private IEnumerable<Product> _products;
        private string _keyword;
        private bool _searching;

        public SearchViewModel()
            : this(new Catalog()) {
        }

        internal SearchViewModel(ICatalog catalog) {
            _catalog = catalog;
        }

        public bool CanSearch {
            get {
                return !IsSearching;
            }
        }

        public bool IsSearching {
            get {
                return _searching;
            }
            private set {
                _searching = value;
                RaisePropertyChanged("IsSearching", "CanSearch");
            }
        }

        public IEnumerable<Product> Products {
            get {
                return _products;
            }
            private set {
                _products = value;
                RaisePropertyChanged("Products");
            }
        }

        public void Search(string keyword) {
            _keyword = keyword;

            IsSearching = true;
            _catalog.SelectProducts(keyword, OnCatalogProductsSelected);
        }

        private void OnCatalogProductsSelected(string keyword, IEnumerable<Product> products) {
            if (keyword != _keyword) {
                return;
            }

            Products = products;
            IsSearching = false;
        }
    }
}
