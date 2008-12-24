// OrderItem.cs
//

using System;
using System.ComponentModel;

namespace Store {

    public partial class OrderItem : Model {

        private Product _product;
        private int _quantity;

        public OrderItem(Product product, int quantity) {
            _product = product;
            _quantity = quantity;
        }

        public decimal Cost {
            get {
                return _product.Price * Quantity;
            }
        }

        public Product Product {
            get {
                return _product;
            }
        }

        public int Quantity {
            get {
                return _quantity;
            }
            set {
                if (_quantity != value) {
                    _quantity = value;
                    RaisePropertyChanged("Quantity", "Cost");
                }
            }
        }
    }
}
