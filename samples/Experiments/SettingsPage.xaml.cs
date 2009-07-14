using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightFX.Applications;

namespace Experiments {

    public partial class SettingsPage : UserControl {

        public SettingsPage() {
            InitializeComponent();

            string name;
            string age;
            ApplicationContext.Current.Settings.TryGetValue("name", out name);
            ApplicationContext.Current.Settings.TryGetValue("age", out age);

            nameTextBox.Text = name ?? String.Empty;
            ageTextBox.Text = age ?? String.Empty;
        }

        private void OnOKButtonClick(object sender, RoutedEventArgs e) {
            ApplicationContext.Current.Settings["name"] = nameTextBox.Text.Trim();
            ApplicationContext.Current.Settings["age"] = ageTextBox.Text.Trim();
        }
    }
}
