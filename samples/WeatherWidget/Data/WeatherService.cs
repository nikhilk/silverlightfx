// WeatherService.cs
//

using System;
using System.IO;
using System.Json;
using System.Net;

namespace WeatherWidget.Data {

    public sealed class WeatherService {

        private const string ServiceUriFormat = "http://pipes.yahooapis.com/pipes/pipe.run?_id=WrXN95ta3RG3WFMqbrsjiw&zip={0}&_render=json";

        private HttpWebRequest _request;
        private string _zipCode;

        public IAsyncResult BeginGetWeather(string zipCode, AsyncCallback callback, object state) {
            _zipCode = zipCode;

            Uri uri = new Uri(String.Format(ServiceUriFormat, zipCode));

            _request = (HttpWebRequest)HttpWebRequest.Create(uri);
            _request.Method = "GET";

            return _request.BeginGetResponse(callback, state);
        }

        public Weather EndGetWeather(IAsyncResult asyncResult) {
            Weather weather = null;

            try {
                HttpWebResponse response = (HttpWebResponse)_request.EndGetResponse(asyncResult);
                if (response.StatusCode == HttpStatusCode.OK) {
                    using (Stream responseStream = response.GetResponseStream()) {
                        StreamReader sr = new StreamReader(responseStream);

                        string json = sr.ReadToEnd();
                        weather = ParseWeather(_zipCode, json);
                    }
                }
            }
            catch {
            }

            return weather;
        }

        private static Weather ParseWeather(string zipCode, string json) {
            JsonValue jsonObject = JsonValue.Parse(json);
            JsonValue location = jsonObject["value"]["items"][0]["loc"];
            JsonValue currentConditions = jsonObject["value"]["items"][0]["cc"];
            JsonValue forecastConditions = jsonObject["value"]["items"][0]["dayf"]["day"];

            WeatherInformation[] forecastItems = new WeatherInformation[4];
            Weather weather = new Weather(zipCode, forecastItems);

            weather.Location = location["dnam"];
            weather.TimeStamp = currentConditions["lsup"];

            weather.Description = currentConditions["t"];
            weather.Temperature = currentConditions["tmp"];
            weather.ImageUri = "/Images/" + currentConditions["icon"] + ".png";

            for (int i = 1; i < 5; i++) {
                WeatherInformation wi = new WeatherInformation();
                JsonValue forecast = forecastConditions[i];
                JsonValue dayInfo = forecast["part"][0];
                JsonValue nightInfo = forecast["part"][1];

                wi.Day = forecast["t"].ToString().Substring(1, 3);
                wi.Date = forecast["dt"];
                wi.Low = forecast["low"];
                wi.High = forecast["hi"];
                wi.DayDescription = dayInfo["t"];
                wi.DayImageUrl = "/Images/" + dayInfo["icon"] + ".png";
                wi.NightDescription = nightInfo["t"];
                wi.NightImageUrl = "/Images/" + nightInfo["icon"] + ".png";

                forecastItems[i - 1] = wi;
            }

            return weather;
        }
    }
}
