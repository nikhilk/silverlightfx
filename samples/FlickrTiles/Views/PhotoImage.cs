// PhotoImage.cs
//

using System;
using System.ComponentModel;
using System.Windows.Media;
using FlickrTiles.Data;

namespace FlickrTiles.Views {

    public class PhotoImage : Model {

        private Photo _photo;
        private ImageSource _thumbnail;
        private ImageSource _image;
        private bool _imageLoading;

        public PhotoImage(Photo photo, ImageSource thumbnail) {
            _photo = photo;
            _thumbnail = thumbnail;
        }

        public ImageSource Image {
            get {
                return _image;
            }
            set {
                _image = value;
            }
        }

        public bool IsImageLoading {
            get {
                return _imageLoading;
            }
            set {
                _imageLoading = value;
                RaisePropertyChanged("IsImageLoading");
            }
        }

        public Photo Photo {
            get {
                return _photo;
            }
        }

        public ImageSource Thumbnail {
            get {
                return _thumbnail;
            }
        }
    }
}
