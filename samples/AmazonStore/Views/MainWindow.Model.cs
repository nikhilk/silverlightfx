// MainWindow.model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SilverlightFX.Applications;

namespace Store {

    public class MainWindowModel : Model {

        private Catalog _catalog;
        private ShoppingCart _cart;
        private IExternalNavigationService _navigationService;

        private IPredicate<object> _filter;
        private IComparer<object> _sort;

        private Product _selectedProduct;

        public MainWindowModel(Catalog catalog, ShoppingCart cart, IExternalNavigationService navigationService) {
            _catalog = catalog;
            _catalog.ProductsLoaded += OnProductsLoaded;
            _catalog.LoadPopularProducts();

            _cart = cart;
            _cart.OrderCheckedOut += OnCartOrderCheckedOut;

            _navigationService = navigationService;
        }

        public Catalog Catalog {
            get {
                return _catalog;
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

        private void OnCartOrderCheckedOut(object sender, EventArgs e) {
            _navigationService.Navigate(new Uri(_cart.CheckoutUrl), "amazon");
        }

        private void OnProductsLoaded(object sender, EventArgs e) {
            ProductFilter = null;
            SelectedProduct = _catalog.Products.FirstOrDefault();
        }
    }
}
