// Tweet.cs
//

using System;
using System.Collections.Generic;

namespace TwitFaves.Data {

    public class TweetGroup : Group<int, Tweet> {

        private string _imageUrl;

        public TweetGroup(Tweet tweet, int daysOld)
            : base(tweet, daysOld) {
            _imageUrl = tweet.ImageUrl;
        }

        public string GroupName {
            get {
                if (DaysOld == 0) {
                    return "Today";
                }
                if (DaysOld == 1) {
                    return "Yesterday";
                }
                if (DaysOld <= 7) {
                    return "This Week";
                }
                return "Older";
            }
        }

        public int DaysOld {
            get {
                return Key;
            }
        }

        public IEnumerable<Tweet> Tweets {
            get {
                return this;
            }
        }

        public static int GetDaysGroupValue(Tweet tweet) {
            int days = (DateTime.UtcNow - tweet.Date).Days;
            if (days <= 0) {
                return 0;
            }
            if (days == 1) {
                return days;
            }
            if (days <= 7) {
                return 7;
            }
            return Int32.MaxValue;
        }
    }
}
