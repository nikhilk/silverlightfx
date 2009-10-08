// Tweet.cs
//

using System;

namespace TwitFaves.Data {

    public class Tweet {

        public DateTime Date {
            get;
            set;
        }

        public string Text {
            get;
            set;
        }

        public string ScreenName {
            get;
            set;
        }

        public string ImageUrl {
            get;
            set;
        }
    }
}
