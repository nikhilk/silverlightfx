// Translator.cs
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Browser;

namespace Translate.Services {

    internal sealed class Translator {

        private const string SupportedLanguagesUriFormat = "http://api.microsofttranslator.com/V2/Http.svc/GetLanguagesForSpeak?appId={0}";
        private const string TranslateUriFormat = "http://api.microsofttranslator.com/V2/Http.svc/Translate?appId={0}&to={2}&text={1}&from=en";
        private const string SpeakUriFormat = "http://api.microsofttranslator.com/V2/Http.svc/Speak?appId={0}&text={1}&language={2}";

        public void GetLanguages(Action<IEnumerable<Language>> languagesCallback) {
            WebClient client = new WebClient();

            client.OpenReadCompleted += (sender, e) => {
                    if (e.Error == null) {
                        DataContractSerializer dcs = new DataContractSerializer(typeof(List<string>));
                        List<string> languageCodes = dcs.ReadObject(e.Result) as List<string>;

                        IEnumerable<Language> languages =
                            from langCode in languageCodes
                            orderby langCode
                            select new Language(langCode, langCode);

                        languagesCallback(languages.ToArray());
                    }
                };

            string appID = Application.Current.Host.InitParams["TranslationApiKey"];
            string url = String.Format(SupportedLanguagesUriFormat, appID);
            client.OpenReadAsync(new Uri(url, UriKind.Absolute));
        }

        public void Translate(string text, string languageCode, Action<string> translationCallback) {
            WebClient client = new WebClient();

            client.OpenReadCompleted += (sender, e) => {
                if (e.Error == null) {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(string));
                    string translatedText = dcs.ReadObject(e.Result) as string;

                    translationCallback(translatedText);
                }
            };

            string appID = Application.Current.Host.InitParams["TranslationApiKey"];
            string url = String.Format(TranslateUriFormat, appID, HttpUtility.UrlEncode(text), languageCode);
            client.OpenReadAsync(new Uri(url, UriKind.Absolute));
        }

        public void GetAudioStream(string text, string languageCode, Action<Stream> audioCallback) {
            WebClient client = new WebClient();

            client.OpenReadCompleted += (sender, e) => {
                if (e.Error == null) {
                    audioCallback(e.Result);
                }
            };

            string appID = Application.Current.Host.InitParams["TranslationApiKey"];
            string url = String.Format(SpeakUriFormat, appID, HttpUtility.UrlEncode(text), languageCode);
            client.OpenReadAsync(new Uri(url, UriKind.Absolute));
        }
    }
}
