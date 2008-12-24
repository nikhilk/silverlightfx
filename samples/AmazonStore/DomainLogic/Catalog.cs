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

        private ObservableCollection<Product> _products;
        private bool _loading;

        public Catalog()
            : this(new AmazonService()) {
        }

        internal Catalog(IStore store) {
            _store = store;
            _products = new ObservableCollection<Product>();
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

        public void LoadPopularProducts() {
            _loading = true;
            _store.GetPopularProducts(OnLoadProducts);

            RaisePropertyChanged("IsLoading");
        }

        public void LoadProducts(string keywords) {
            if (String.IsNullOrEmpty(keywords)) {
                return;
            }

            _loading = true;
            _store.GetProducts(keywords, OnLoadProducts);

            RaisePropertyChanged("IsLoading");
        }

        private void OnLoadProducts(IEnumerable<Product> productResults) {
            if (productResults != null) {
                _products.Clear();

                foreach (Product p in productResults) {
                    _products.Add(p);
                }

                if (ProductsLoaded != null) {
                    ProductsLoaded(this, EventArgs.Empty);
                }
            }

            _loading = false;
            RaisePropertyChanged("PriceRange", "IsLoading");
        }
    }
}
