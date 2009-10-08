// ITwitterService.cs
//

using System;
using System.Collections.Generic;

namespace TwitFaves.Data {

    public interface ITwitterService {

        void GetProfile(string userName, Action<Profile> profileCallback);

        void GetTweets(string userName, Action<IEnumerable<Tweet>> tweetsCallback);
    }
}
