// WeatherInformation.cs
//

using System;

namespace WeatherWidget.Data {

    public class WeatherInformation {

        private string _day;
        private string _date;
        private string _dayImageUrl;
        private string _dayDescription;
        private string _nightImageUrl;
        private string _nighDescription;
        private string _low;
        private string _high;

        internal WeatherInformation() {
        }

        public string Date {
            get {
                return _date;
            }
            internal set {
                _date = value;
            }
        }

        public string Day {
            get {
                return _day;
            }
            internal set {
                _day = value;
            }
        }

        public string DayDescription {
            get {
                return _dayDescription;
            }
            internal set {
                _dayDescription = value;
            }
        }

        public string DayImageUrl {
            get {
                return _dayImageUrl;
            }
            internal set {
                _dayImageUrl = value;
            }
        }

        public string Low {
            get {
                return _low + "°F";
            }
            internal set {
                _low = value;
            }
        }

        public string High {
            get {
                return _high + "°F";
            }
            internal set {
                _high = value;
            }
        }

        public string NightDescription {
            get {
                return _nighDescription;
            }
            internal set {
                _nighDescription = value;
            }
        }

        public string NightImageUrl {
            get {
                return _nightImageUrl;
            }
            internal set {
                _nightImageUrl = value;
            }
        }
    }
}
