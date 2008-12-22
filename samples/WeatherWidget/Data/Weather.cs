// Weather.cs
//

using System;

namespace WeatherWidget.Data {

    public class Weather {

        private string _zipCode;
        private string _location;
        private string _timeStamp;

        private string _description;
        private string _imageUri;
        private string _temperature;

        private WeatherInformation[] _forecast;

        internal Weather(string zipCode, WeatherInformation[] forecast) {
            _zipCode = zipCode;
            _forecast = forecast;

            _timeStamp = " ";
            _description = " ";
            _location = " ";
            _temperature = " ";
        }

        public string Description {
            get {
                return _description;
            }
            internal set {
                _description = value;
            }
        }

        public WeatherInformation[] Forecast {
            get {
                return _forecast;
            }
        }

        public string ImageUri {
            get {
                return _imageUri;
            }
            internal set {
                _imageUri = value;
            }
        }

        public string Location {
            get {
                return _location;
            }
            internal set {
                _location = value;
            }
        }

        public string Temperature {
            get {
                return _temperature + "°F";
            }
            internal set {
                _temperature = value;
            }
        }

        public string TimeStamp {
            get {
                return _timeStamp;
            }
            internal set {
                _timeStamp = value;
            }
        }

        public string ZipCode {
            get {
                return _zipCode;
            }
        }
    }
}
