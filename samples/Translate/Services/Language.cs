// Language.cs
//

using System;

namespace Translate.Services {

    public sealed class Language {

        internal static readonly Language English = new Language("English", "en");
        internal static readonly Language German = new Language("German", "de");
        internal static readonly Language French = new Language("French", "fr");
        internal static readonly Language Italian = new Language("Italian", "it");
        internal static readonly Language Portugese = new Language("Portugese", "pt");
        internal static readonly Language Russian = new Language("Russian", "ru");
        internal static readonly Language Spanish = new Language("Spanish", "es");

        private string _name;
        private string _code;

        public Language(string name, string code) {
            _name = name;
            _code = code;
        }

        public string Code {
            get {
                return _code;
            }
        }

        public string Name {
            get {
                return _name;
            }
        }
    }
}
