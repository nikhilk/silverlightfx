// WidgetModel.cs
//

using System;
using System.ComponentModel;
using WeatherWidget.Data;
using SilverlightFX.Applications;

namespace WeatherWidget {

    public class WidgetModel : Model {

        private Weather _weather;

        private string _zipCode;
        private IAsyncResult _currentAsyncResult;

        public WidgetModel() {
            string zipCode;
            if (ApplicationContext.Current.Settings.TryGetValue("ZipCode", out zipCode)) {
                if (String.IsNullOrEmpty(zipCode) == false) {
                    LookupWeather(zipCode);
                }
            }
        }

        public bool IsLoading {
            get {
                return _currentAsyncResult != null;
            }
        }

        public string Status {
            get {
                if (IsLoading) {
                    return "Loading " + _zipCode + "...";
                }
                if (_weather != null) {
                    return _weather.TimeStamp;
                }
                return String.Empty;
            }
        }

        public Weather Weather {
            get {
                return _weather;
            }
        }

        public string ZipCode {
            get {
                return _zipCode;
            }
        }

        public void LookupWeather(string zipCode) {
            if (String.IsNullOrEmpty(zipCode) || (zipCode == _zipCode)) {
                return;
            }

            _zipCode = zipCode;

            WeatherService weatherService = new WeatherService();
            _currentAsyncResult = weatherService.BeginGetWeather(zipCode, LookupWeatherCallback, weatherService);

            RaisePropertyChanged("Weather", "IsLoading", "Status", "ZipCode");
        }

        private void LookupWeatherCallback(IAsyncResult asyncResult) {
            if (asyncResult == _currentAsyncResult) {
                _currentAsyncResult = null;

                WeatherService weatherService = (WeatherService)asyncResult.AsyncState;

                _weather = weatherService.EndGetWeather(asyncResult);
                if (_weather != null) {
                    ApplicationContext.Current.Settings["ZipCode"] = _zipCode;
                }

                RaisePropertyChanged("Weather", "IsLoading", "Status", "ZipCode");
            }
        }
    }
}
