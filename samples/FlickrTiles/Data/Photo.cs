// Photo.cs
//

using System;
using System.ComponentModel;

namespace FlickrTiles.Data {

    public class Photo : Model {

        public string PageUrl {
            get;
            set;
        }

        public string PhotoUrl {
            get;
            set;
        }

        public DateTime ShotOn {
            get;
            set;
        }

        public string ThumbnailUrl {
            get;
            set;
        }

        public string Title {
            get;
            set;
        }
    }
}
