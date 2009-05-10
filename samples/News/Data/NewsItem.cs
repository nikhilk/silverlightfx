// NewsItem.cs
//

using System;
using System.ComponentModel;

namespace NewsWidget.Data {

    public class NewsItem : Model {

        public string Headline {
            get;
            set;
        }

        public DateTime PublishDate {
            get;
            set;
        }

        public string Section {
            get;
            set;
        }

        public string Summary {
            get;
            set;
        }

        public Uri Uri {
            get;
            set;
        }
    }
}
