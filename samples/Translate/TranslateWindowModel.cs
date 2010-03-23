// TranslateWindowModel.cs
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Translate.Services;

namespace Translate {

    public class TranslateWindowModel : ViewModel {

        private ObservableCollection<Language> _languages;

        private string _text;
        private string _translatedText;
        private Language _translatedLanguage;

        public TranslateWindowModel() {
            _languages = new ObservableCollection<Language>() {
                Language.German,
                Language.French,
                Language.Italian,
                Language.Portugese,
                Language.Spanish,
                Language.English
            };
        }

        public bool CanSpeak {
            get {
                return (String.IsNullOrEmpty(_translatedText) == false);
            }
        }

        public IEnumerable<Language> Languages {
            get {
                return _languages;
            }
        }

        public string Text {
            get {
                return _text;
            }
            set {
                _text = value;
                RaisePropertyChanged("Text");

                TranslatedText = String.Empty;
            }
        }

        public string TranslatedText {
            get {
                return _translatedText;
            }
            private set {
                _translatedText = value;
                RaisePropertyChanged("TranslatedText", "CanSpeak");
            }
        }

        public event EventHandler<StreamEventArgs> PlayStream;

        public void Speak() {
            Translator translator = new Translator();
            translator.GetAudioStream(_translatedText, _translatedLanguage.Code, delegate(Stream s) {
                if (PlayStream != null) {
                    PlayStream(this, new StreamEventArgs(s));
                }
            });
        }

        public void Translate(Language language) {
            if (String.IsNullOrEmpty(Text)) {
                return;
            }

            _translatedLanguage = language;

            Translator translator = new Translator();
            translator.Translate(_text, _translatedLanguage.Code, delegate(string translatedText) {
                TranslatedText = translatedText;
            });
        }
    }
}
