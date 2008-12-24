// Order.cs
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Store {

    public class Order : Model {

        private ObservableCollection<OrderItem> _items;

        public Order() {
            _items = new ObservableCollection<OrderItem>();
        }

        public bool IsEmpty {
            get {
                return _items.Count == 0;
            }
        }

        public IEnumerable<OrderItem> Items {
            get {
                return _items;
            }
        }

        public decimal Total {
            get {
                decimal total = 0m;

                foreach (OrderItem item in _items) {
                    total += item.Cost;
                }

                return total;
            }
        }

        public void AddItem(Product product, int quantity) {
            if (quantity == 0) {
                quantity = 1;
            }

            OrderItem item = new OrderItem(product, quantity);
            ((INotifyPropertyChanged)item).PropertyChanged += OnItemPropertyChanged;
            _items.Add(item);

            UpdateTotal();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            UpdateTotal();
        }

        public void RemoveItem(OrderItem item) {
            _items.Remove(item);
            ((INotifyPropertyChanged)item).PropertyChanged -= OnItemPropertyChanged;

            UpdateTotal();
        }

        internal void UpdateTotal() {
            RaisePropertyChanged("Total");
        }
    }
}
