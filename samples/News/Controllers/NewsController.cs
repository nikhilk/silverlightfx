// NewsController.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Navigation;
using System.Linq;
using NewsWidget.Data;
using NewsWidget.Services;

namespace NewsWidget.Controllers {

    public sealed class NewsController : Controller {

        private INewsService _newsService;

        public NewsController([Dependency] INewsService newsService) {
            _newsService = newsService;
        }

        public Async<ActionResult> List() {
            Async<ActionResult> task = new Async<ActionResult>();
            _newsService.GetNews(/* limitToLastDay */ true, OnNewsItemsAvailable, task);

            return task;
        }

        private void OnNewsItemsAvailable(IEnumerable<NewsItem> newsItems, object userState) {
            IEnumerable<NewsItem> orderedItems = newsItems.AsQueryable().OrderByDescending(i => i.PublishDate);

            ViewActionResult result = View("List");
            result.ViewData["Items"] = orderedItems.ToArray();

            Async<ActionResult> task = (Async<ActionResult>)userState;
            task.Complete(result);
        }

        public Async<ActionResult> Search(string query) {
            if (String.IsNullOrEmpty(query)) {
                throw new ArgumentNullException("query");
            }

            Async<ActionResult> task = new Async<ActionResult>();
            _newsService.Search(query, OnNewsItemsAvailable, task);

            return task;
        }
    }
}
