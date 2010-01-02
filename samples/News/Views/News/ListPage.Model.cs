// ListPage.Model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewsWidget.Data;

namespace NewsWidget.Views.News {

    public class ListPageModel : Model {

        private bool _filterToday;
        private IEnumerable<NewsItem> _items;

        public bool FilterToday {
            get {
                return _filterToday;
            }
            set {
                _filterToday = value;
                RaisePropertyChanged("FilterToday", "ListItems");
            }
        }

        public IEnumerable<NewsItem> Items {
            get {
                return _items;
            }
            set {
                _items = value;
                RaisePropertyChanged("Items", "ListItems");
            }
        }

        public IEnumerable<NewsItem> ListItems {
            get {
                if (_filterToday == false) {
                    return Items;
                }

                return Items.Where(item => item.PublishDate.Date == DateTime.UtcNow.Date);
            }
        }
    }
}
