// MainViewModel.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TwitFaves.Data;

namespace TwitFaves {

    public class MainViewModel : ViewModel {

        private ITwitterService _twitterService;

        public MainViewModel(ITwitterService twitterService) {
            _twitterService = twitterService;
        }

        public Async<IEnumerable> GetTweets(string userName) {
            Async<IEnumerable> asyncTweets = new Async<IEnumerable>();

            _twitterService.GetTweets(userName, delegate(IEnumerable<Tweet> tweets) {
                if (tweets == null) {
                    asyncTweets.Complete(new Exception("Favorites for '" + userName + "' could not be loaded."));
                }
                else {
                    IEnumerable<TweetGroup> groupedTweets =
                        tweets.AsQueryable().
                        OrderByDescending(t => t.Date).
                        GroupByContiguous<Tweet, int, TweetGroup>(
                            t => TweetGroup.GetDaysGroupValue(t),
                            EqualityComparer<int>.Default,
                            (t, d) => new TweetGroup(t, d));

                    asyncTweets.Complete(groupedTweets);
                }
            });

            return asyncTweets;
        }

        public Async<Profile> GetProfile(string userName) {
            Async<Profile> asyncProfile = new Async<Profile>();

            _twitterService.GetProfile(userName, delegate(Profile profile) {
                if (profile == null) {
                    asyncProfile.Complete(new Exception("The profile for '" + userName + "' could not be loaded."));
                }
                else {
                    asyncProfile.Complete(profile);
                }
            });

            return asyncProfile;
        }
    }
}
