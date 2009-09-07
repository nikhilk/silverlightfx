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

        public Uri ImageUri {
            get;
            set;
        }

        public Uri ItemUri {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }
    }
}
