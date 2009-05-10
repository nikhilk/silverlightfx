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

        public Task<ActionResult> List() {
            Task<ActionResult> task = new Task<ActionResult>();
            _newsService.GetNews(/* limitToLastDay */ true, OnNewsItemsAvailable, task);

            return task;
        }

        private void OnNewsItemsAvailable(IEnumerable<NewsItem> newsItems, object userState) {
            IEnumerable<NewsItem> orderedItems = newsItems.AsQueryable().OrderByDescending(i => i.PublishDate);
            IEnumerable<NewsItem> viewModel = orderedItems.ToArray();

            ViewActionResult result = new ViewActionResult("List", viewModel);

            Task<ActionResult> task = (Task<ActionResult>)userState;
            task.Complete(result);
        }

        public Task<ActionResult> Search(string query) {
            if (String.IsNullOrEmpty(query)) {
                throw new ArgumentNullException("query");
            }

            Task<ActionResult> task = new Task<ActionResult>();
            _newsService.Search(query, OnNewsItemsAvailable, task);

            return task;
        }
    }
}
