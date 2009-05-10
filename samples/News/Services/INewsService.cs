// INewsService.cs
//

using System;
using System.Collections.Generic;
using NewsWidget.Data;

namespace NewsWidget.Services {

    public interface INewsService {

        void GetNews(bool limitToLastDay, Action<IEnumerable<NewsItem>, object> newsItemsCallback, object userState);

        void Search(string query, Action<IEnumerable<NewsItem>, object> newsItemsCallback, object userState);
    }
}
