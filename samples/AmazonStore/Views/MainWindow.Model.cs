// MainWindow.model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Store {

    public class MainWindowModel : Model {

        private ShoppingCart _cart;
        private Catalog _catalog;

        private IPredicate<object> _filter;
        private IComparer<object> _sort;

        private Product _selectedProduct;

        private string _navigateUrl;

        public MainWindowModel() {
            _cart = new ShoppingCart();
            _cart.OrderCheckedOut += OnCartOrderCheckedOut;

            _catalog = new Catalog();
            _catalog.ProductsLoaded += OnProductsLoaded;
            _catalog.LoadPopularProducts();
        }

        public Catalog Catalog {
            get {
                return _catalog;
            }
        }

        public string NavigateUrl {
            get {
                return _navigateUrl;
            }
        }

        public IPredicate<object> ProductFilter {
            get {
                return _filter;
            }
            set {
                if (_filter != value) {
                    _filter = value;
                    RaisePropertyChanged("ProductFilter");
                }
            }
        }

        public IComparer<object> ProductSort {
            get {
                return _sort;
            }
            set {
                if (_sort != value) {
                    _sort = value;
                    RaisePropertyChanged("ProductSort");
                }
            }
        }

        public Product SelectedProduct {
            get {
                return _selectedProduct;
            }
            set {
                _selectedProduct = value;
                RaisePropertyChanged("SelectedProduct");
            }
        }

        public ShoppingCart ShoppingCart {
            get {
                return _cart;
            }
        }

        public event EventHandler Navigation;

        private void OnCartOrderCheckedOut(object sender, EventArgs e) {
            _navigateUrl = _cart.CheckoutUrl;
            RaisePropertyChanged("NavigateUrl");

            if (Navigation != null) {
                Navigation(this, EventArgs.Empty);
            }
        }

        private void OnProductsLoaded(object sender, EventArgs e) {
            ProductFilter = null;
            SelectedProduct = _catalog.Products.FirstOrDefault();
        }
    }
}
