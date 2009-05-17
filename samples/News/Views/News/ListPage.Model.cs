// ListPage.Model.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NewsWidget.Data;

namespace NewsWidget.Views.News {

    public class ListPageModel : Model {

        public IEnumerable<NewsItem> Items {
            get;
            set;
        }
    }
}
