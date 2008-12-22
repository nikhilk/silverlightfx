// Product.cs
//

using System;
using System.ComponentModel;

namespace AmazonSearch.Data {

    public class Product : Model {

        public string ASIN {
            get;
            set;
        }

        public string By {
            get;
            set;
        }

        public string ImageUrl {
            get;
            set;
        }

        public string ItemUrl {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }
    }
}
