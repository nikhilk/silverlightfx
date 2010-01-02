// MainViewModel.cs
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Media;
using FlickrTiles.Data;

namespace FlickrTiles.Views {

    public class MainViewModel : ViewModel {

        private IPhotoGallery _photoGallery;

        private ObservableCollection<PhotoImage> _photos;
        private string _tags;
        private bool _searching;

        private PhotoImage _selectedPhoto;
        private bool _loadingImage;

        public MainViewModel()
            : this(new FlickrService()) {
        }

        internal MainViewModel(IPhotoGallery photoGallery) {
            _photoGallery = photoGallery;
            _photos = new ObservableCollection<PhotoImage>();
        }

        public bool IsSearching {
            get {
                return _searching;
            }
            private set {
                _searching = value;
                RaisePropertyChanged("IsSearching");
                RaisePropertyChanged("SearchText");
            }
        }

        public IEnumerable<PhotoImage> Photos {
            get {
                return _photos;
            }
        }

        public string SearchText {
            get {
                return _searching ? "Searching..." : "Search";
            }
        }

        public PhotoImage SelectedPhoto {
            get {
                return _selectedPhoto;
            }
            set {
                if ((value != null) && (value.Image == null)) {
                    if (_loadingImage) {
                        return;
                    }

                    _loadingImage = true;
                    value.IsImageLoading = true;
                    ImageLoader.LoadImage(value.Photo.PhotoUri, OnImageLoaded, value);
                }
                else {
                    _selectedPhoto = value;
                    RaisePropertyChanged("SelectedPhoto");
                }
            }
        }

        public void Search(string tags) {
            _tags = tags;

            IsSearching = true;
            _photoGallery.SearchPhotos(tags, OnPhotoGalleryPhotosSearched);
        }

        private void OnImageLoaded(ImageSource image, object context) {
            PhotoImage photo = (PhotoImage)context;
            photo.Image = image;
            photo.IsImageLoading = false;

            _loadingImage = false;
            if (image != null) {
                _selectedPhoto = photo;
                RaisePropertyChanged("SelectedPhoto");
            }
        }

        private void OnPhotoGalleryPhotosSearched(string tags, IEnumerable<Photo> photos) {
            if (tags != _tags) {
                return;
            }

            _photos.Clear();
            _loadingImage = false;
            SelectedPhoto = null;

            foreach (Photo photo in photos) {
                ImageLoader.LoadImage(photo.ThumbnailUri, OnThumbnailLoaded, photo);
            }
        }

        private void OnThumbnailLoaded(ImageSource image, object context) {
            if (image != null) {
                Photo loadedPhoto = (Photo)context;

                PhotoImage photoImage = new PhotoImage(loadedPhoto, image);
                _photos.Add(photoImage);

                if (_photos.Count == 1) {
                    IsSearching = false;
                }
            }
        }
    }
}
