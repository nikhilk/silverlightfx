// ProductNameComparer.cs
//

using System;
using System.Collections.Generic;

namespace Store {

    public sealed class ProductNameComparer : IComparer<object> {

        public int Compare(object x, object y) {
            Product p1 = (Product)x;
            Product p2 = (Product)y;

            return String.Compare(p1.Title, p2.Title);
        }

        public override string ToString() {
            return "Name";
        }
    }
}
