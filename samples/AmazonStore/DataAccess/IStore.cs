// IStore.cs
//

using System;
using System.Collections.Generic;

namespace Store {

    internal interface IStore {

        void GetPopularProducts(Action<IEnumerable<Product>> productsCallback);

        void GetProducts(string keyword, Action<IEnumerable<Product>> productsCallback);

        void SubmitOrder(Order order, Action<string> purchaseUrlCallback);
    }
}
