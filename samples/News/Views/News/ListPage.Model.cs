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
            get;
            set;
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
