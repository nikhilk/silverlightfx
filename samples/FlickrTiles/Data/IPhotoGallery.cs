// IPhotoGallery.cs
//

using System;
using System.Collections.Generic;

namespace FlickrTiles.Data {

    public interface IPhotoGallery {

        void SearchPhotos(string tag, Action<string, IEnumerable<Photo>> photosCallback);
    }
}
