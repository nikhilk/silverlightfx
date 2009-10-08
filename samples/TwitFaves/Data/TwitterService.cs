// TwitterService.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;

namespace TwitFaves.Data {

    [Service(typeof(ITwitterService))]
    public sealed class TwitterService : ITwitterService {

        private const string ProfileUriFormat = "http://pipes.yahooapis.com/pipes/pipe.run?_id=33459206c20c11fadeeb777d88e6d54d&_render=json&userName={0}&r={1}";
        private const string TweetsUriFormat = "http://pipes.yahooapis.com/pipes/pipe.run?_id=b9d8434a09e96ff3a46c7466e37b1298&_render=json&userName={0}&r={1}";

        public void GetProfile(string userName, Action<Profile> profileCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                Profile profile = null;

                if (e.Error == null) {
                    try {
                        JsonValue json = JsonValue.Parse(e.Result);
                        JsonValue jsonProfile = json["value"]["items"][0];

                        profile = new Profile();
                        profile.Name = jsonProfile["name"];
                        profile.ScreenName = jsonProfile["screen_name"];
                        profile.ImageUrl = ((string)jsonProfile["profile_image_url"]).Replace("_normal", "_bigger");
                        profile.Status = jsonProfile["status"]["text"];
                        profile.Friends = Int32.Parse(jsonProfile["friends_count"]);
                        profile.Followers = Int32.Parse(jsonProfile["followers_count"]);
                        profile.Updates = Int32.Parse(jsonProfile["statuses_count"]);
                    }
                    catch {
                        profile = null;
                    }
                }

                profileCallback(profile);
            };

            Uri requestUri = new Uri(String.Format(ProfileUriFormat, userName, new Random().Next()), UriKind.Absolute);
            webClient.DownloadStringAsync(requestUri);
        }

        public void GetTweets(string userName, Action<IEnumerable<Tweet>> tweetsCallback) {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e) {
                List<Tweet> tweets = null;

                if (e.Error == null) {
                    try {
                        tweets = new List<Tweet>();

                        JsonValue json = JsonValue.Parse(e.Result);
                        JsonArray jsonTweets = (JsonArray)json["value"]["items"];

                        foreach (JsonValue jsonTweet in jsonTweets) {
                            JsonValue jsonUser = jsonTweet["user"];

                            Tweet tweet = new Tweet();
                            tweet.ScreenName = jsonUser["screen_name"];
                            tweet.ImageUrl = ((string)jsonUser["profile_image_url"]).Replace("_normal", "_bigger");
                            tweet.Text = ProcessHashTags(jsonTweet["text"]);
                            tweet.Date = ParseDateTime(jsonTweet["created_at"]);

                            tweets.Add(tweet);
                        }
                    }
                    catch {
                        tweets = null;
                    }
                }

                tweetsCallback(tweets);
            };

            Uri requestUri = new Uri(String.Format(TweetsUriFormat, userName, new Random().Next()), UriKind.Absolute);
            webClient.DownloadStringAsync(requestUri);
        }

        private static DateTime ParseDateTime(string date) {
            string dayOfWeek = date.Substring(0, 3).Trim();
            string month = date.Substring(4, 3).Trim();
            string dayInMonth = date.Substring(8, 2).Trim();
            string time = date.Substring(11, 9).Trim();
            string offset = date.Substring(20, 5).Trim();
            string year = date.Substring(25, 5).Trim();

            date = String.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
            return DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc);
        }

        private static string ProcessHashTags(string text) {
            int hashTagIndex = 0;

            while ((hashTagIndex >= 0) && (hashTagIndex < text.Length)) {
                hashTagIndex = text.IndexOf('#', hashTagIndex);
                if (hashTagIndex >= 0) {
                    int endHashTagIndex = text.IndexOf(' ', hashTagIndex);
                    if (endHashTagIndex < 0) {
                        endHashTagIndex = text.Length;
                    }

                    string hashTag = text.Substring(hashTagIndex, endHashTagIndex - hashTagIndex);
                    string link = "http://twitter.com/search?q=%23" + hashTag.Substring(1) + "|" + hashTag;

                    text = text.Replace(hashTag, link);

                    hashTagIndex += link.Length;
                }
            }

            return text;
        }
    }
}
