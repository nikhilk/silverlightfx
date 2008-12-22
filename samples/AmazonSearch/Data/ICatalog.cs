// ICatalog.cs
//

using System;
using System.Collections.Generic;

namespace AmazonSearch.Data {

    internal interface ICatalog {

        void SelectProducts(string keyword, Action<string, IEnumerable<Product>> productsCallback);
    }
}
