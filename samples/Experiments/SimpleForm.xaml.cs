// SimpleForm.xaml.cs
//

using System;
using System.Windows.Controls;
using SilverlightFX.UserInterface;

namespace Experiments {

    public partial class SimpleForm : Form {

        private string _input;

        public SimpleForm() {
            InitializeComponent();
        }

        public string Input {
            get {
                return _input;
            }
        }

        private void OnOKButtonClick(object sender, System.Windows.RoutedEventArgs e) {
            _input = nameTextBox.Text;
            Close(FormResult.OK);
        }

        private void OnCancelButtonClick(object sender, System.Windows.RoutedEventArgs e) {
            Close(FormResult.Cancel);
        }
    }
}
