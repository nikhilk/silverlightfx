// NewsWidgetModel.model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NewsWidget {

    public sealed class NewsWidgetModel : Model {

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

        public Uri SearchUrl {
            get {
                if (String.IsNullOrEmpty(_searchText)) {
                    return null;
                }
                return new Uri("/News/Search?query=" + _searchText, UriKind.Relative);
            }
        }
    }
}
