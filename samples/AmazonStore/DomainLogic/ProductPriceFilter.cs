// ProductPriceFilter.cs
//

using System;
using System.Collections.Generic;

namespace Store {

    public sealed class ProductPriceFilter : IPredicate<object> {

        private int _priceRange;

        public ProductPriceFilter(int priceRange) {
            _priceRange = priceRange;
        }

        public int PriceRange {
            get {
                return _priceRange;
            }
        }

        public bool Filter(object item) {
            return ((Product)item).Price <= _priceRange;
        }
    }
}
