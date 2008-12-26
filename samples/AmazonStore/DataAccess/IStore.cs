// IStore.cs
//

using System;
using System.Collections.Generic;

namespace Store {

    public interface IStore {

        void GetPopularProducts(Action<IEnumerable<Product>, bool> productsCallback);

        void GetProducts(string keyword, Action<IEnumerable<Product>, bool> productsCallback);

        void SubmitOrder(Order order, Action<string> purchaseUrlCallback);
    }
}
