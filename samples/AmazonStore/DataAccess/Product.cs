// Product.cs
//

using System;
using System.ComponentModel;

namespace Store {

    public sealed class Product : Model {

        private string _id;
        private string _description;
        private string _imageUri;
        private decimal _price;
        private string _productUri;
        private string _title;

        public string Description {
            get {
                return _description;
            }
            set {
                if (_description != value) {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public string ID {
            get {
                return _id;
            }
            set {
                if (_id != value) {
                    _id = value;
                }
            }
        }

        public string ImageUri {
            get {
                return _imageUri;
            }
            set {
                if (_imageUri != value) {
                    _imageUri = value;
                    RaisePropertyChanged("ImageUri");
                }
            }
        }

        public decimal Price {
            get {
                return _price;
            }
            set {
                if (_price != value) {
                    _price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        public string ProductUri {
            get {
                return _productUri;
            }
            set {
                if (_productUri != value) {
                    _productUri = value;
                    RaisePropertyChanged("ProductUri");
                }
            }
        }

        public string Title {
            get {
                return _title;
            }
            set {
                if (_title != value) {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }
    }
}
