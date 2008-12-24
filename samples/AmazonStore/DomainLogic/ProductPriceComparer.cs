// ProductPriceComparer.cs
//

using System;
using System.Collections.Generic;

namespace Store {

    public sealed class ProductPriceComparer : IComparer<object> {

        private bool _ascending;

        public ProductPriceComparer() {
            _ascending = true;
        }

        public bool Ascending {
            get {
                return _ascending;
            }
            set {
                _ascending = value;
            }
        }

        public int Compare(object x, object y) {
            Product p1 = (Product)x;
            Product p2 = (Product)y;
            if (p1.Price < p2.Price) {
                return _ascending ? -1 : 1;
            }
            else if (p1.Price > p2.Price) {
                return _ascending ? 1 : -1;
            }
            return String.Compare(p1.Title, p2.Title);
        }

        public override string ToString() {
            return _ascending ? "Price (Low to High)" : "Price (High to Low)";
        }
    }
}
