// ShoppingCart.cs
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Store {

    public class ShoppingCart : Model {

        private IStore _store;

        private Order _order;
        private bool _isCheckingOut;
        private string _checkoutUrl;

        public ShoppingCart()
            : this(new AmazonService()) {
        }

        internal ShoppingCart(IStore store) {
            _store = store;
        }

        public bool CanCheckout {
            get {
                return (_order.IsEmpty == false) && !IsCheckingOut;
            }
        }

        public string CheckoutUrl {
            get {
                return _checkoutUrl;
            }
        }

        public Order Order {
            get {
                if (_order == null) {
                    _order = new Order();
                }
                return _order;
            }
        }

        public bool IsCheckingOut {
            get {
                return _isCheckingOut;
            }
            private set {
                _isCheckingOut = value;

                RaisePropertyChanged("IsCheckingOut", "CanCheckout");
            }
        }

        public event EventHandler OrderCheckedOut;

        public void AddItem(Product product, int quantity) {
            Order.AddItem(product, quantity);
            RaisePropertyChanged("CanCheckout");
        }

        public void Checkout() {
            if (CanCheckout) {
                IsCheckingOut = true;
                _store.SubmitOrder(_order, OnCheckoutCompleted);
            }
        }

        private void OnCheckoutCompleted(string purchaseUrl) {
            if (String.IsNullOrEmpty(purchaseUrl) == false) {
                _checkoutUrl = purchaseUrl;
                RaisePropertyChanged("CheckoutUrl");

                if (OrderCheckedOut != null) {
                    OrderCheckedOut(this, EventArgs.Empty);
                }

                _order = null;
                _checkoutUrl = null;
                RaisePropertyChanged("Order", "CheckoutUrl");
            }
            IsCheckingOut = false;
        }

        public void RemoveItem(OrderItem item) {
            Order.RemoveItem(item);
            RaisePropertyChanged("CanCheckout");
        }
    }
}
