using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SilverlightFX.UserInterface;

namespace Experiments {

    public class CityInfo {

        public string City {
            get;
            set;
        }

        public string Location {
            get {
                return "(" + State + ", " + ZipCode + ")";
            }
        }

        public string State {
            get;
            set;
        }

        public string ZipCode {
            get;
            set;
        }
    }

    public partial class DataEntryPage : UserControl {

        public DataEntryPage() {
            InitializeComponent();
        }

        private void OnCityAutoCompleteCompleted(object sender, AutoCompleteCompletedEventArgs e) {
            CityInfo cityInfo = (CityInfo)e.SelectedItem;

            zipTextBox.Text = cityInfo.ZipCode;
            e.SelectedItem = cityInfo.City;
        }

        private void OnZipCodeAutoCompleteCompleting(object sender, AutoCompleteCompletingEventArgs e) {
            List<string> strings = new List<string>();

            for (int i = 0; i < 10; i++) {
                string s = e.Prefix + i;
                strings.Add(s);
            }
            e.SetCompletionItems(strings);
        }
    }
}
