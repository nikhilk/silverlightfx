// SearchPage.xaml.cs
//

using System;
using System.Windows;
using System.Windows.Controls;

namespace Experiments {

    public partial class SearchPage : UserControl {

        public SearchPage() {
            InitializeComponent();
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e) {
            searchLabel.Text = "You searched for '" + searchTextBox.Text + "'...";
            // TODO: Do Search
        }
    }
}
