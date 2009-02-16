// Catalog.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Store {

    public sealed class Catalog : Model {

        private IStore _store;

        private ObservableCollection<Product> _popularProducts;
        private ObservableCollection<Product> _bargainProducts;
        private ObservableCollection<Product> _searchedProducts;

        private IEnumerable<Product> _products;
        private bool _loading;
        private bool _clear;

        public Catalog(IStore store) {
            _store = store;
        }

        public bool IsLoading {
            get {
                return _loading;
            }
        }

        public IEnumerable<Product> Products {
            get {
                return _products;
            }
            private set {
                if (_products != value) {
                    _products = value;
                    RaisePropertyChanged("Products");
                }
            }
        }

        public int PriceRange {
            get {
                if (_products == null) {
                    return 0;
                }

                decimal maxPrice = 0m;
                foreach (Product p in _products) {
                    maxPrice = Math.Max(maxPrice, p.Price);
                }

                int simpleMaxPrice = (int)(maxPrice + 1m);
                simpleMaxPrice += simpleMaxPrice % 10;

                return simpleMaxPrice;
            }
        }

        public event EventHandler ProductsLoaded;

        public void LoadBargainProducts() {
            if (_bargainProducts != null) {
                Products = _bargainProducts;
                return;
            }

            _bargainProducts = new ObservableCollection<Product>();
            _loading = true;
            _store.GetBargainProducts(OnLoadBargainProducts);

            RaisePropertyChanged("IsLoading");
        }

        public void LoadPopularProducts() {
            if (_popularProducts != null) {
                Products = _popularProducts;
                return;
            }

            _popularProducts = new ObservableCollection<Product>();
            _loading = true;
            _store.GetPopularProducts(OnLoadPopularProducts);

            RaisePropertyChanged("IsLoading");
        }

        public void LoadProducts(string keywords) {
            if (String.IsNullOrEmpty(keywords)) {
                return;
            }

            if (_searchedProducts == null) {
                _searchedProducts = new ObservableCollection<Product>();
            }
            else {
                _clear = true;
            }

            _loading = true;
            _store.GetProducts(keywords, OnLoadProducts);

            RaisePropertyChanged("IsLoading");
        }

        private void OnLoadBargainProducts(IEnumerable<Product> productResults, bool completed) {
            UpdateProducts(_bargainProducts, productResults, completed);
        }

        private void OnLoadPopularProducts(IEnumerable<Product> productResults, bool completed) {
            UpdateProducts(_popularProducts, productResults, completed);
        }

        private void OnLoadProducts(IEnumerable<Product> productResults, bool completed) {
            UpdateProducts(_searchedProducts, productResults, completed);
        }

        private void UpdateProducts(ObservableCollection<Product> productList, IEnumerable<Product> productResults, bool completed) {
            if (productResults != null) {
                if (_clear) {
                    productList.Clear();
                    _clear = false;
                }

                foreach (Product p in productResults) {
                    productList.Add(p);
                }
            }

            Products = productList;

            if (ProductsLoaded != null) {
                ProductsLoaded(this, EventArgs.Empty);
            }

            if (completed) {
                _loading = false;
                RaisePropertyChanged("PriceRange", "IsLoading");
            }
        }
    }
}
