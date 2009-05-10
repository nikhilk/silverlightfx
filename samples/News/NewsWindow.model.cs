// NewsWindow.model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NewsWidget {

    public sealed class NewsWindowModel : Model {

        private string _searchText;

        public string SearchText {
            get {
                return _searchText;
            }
            set {
                _searchText = value;
                RaisePropertyChanged("SearchText", "SearchUrl");
            }
        }

        public string SearchUrl {
            get {
                if (String.IsNullOrEmpty(_searchText)) {
                    return null;
                }
                return "/News/Search?query=" + _searchText;
            }
        }
    }
}
