// Photo.cs
//

using System;
using System.ComponentModel;

namespace FlickrTiles.Data {

    public class Photo : Model {

        public Uri PageUri {
            get;
            set;
        }

        public Uri PhotoUri {
            get;
            set;
        }

        public DateTime ShotOn {
            get;
            set;
        }

        public Uri ThumbnailUri {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }
    }
}
