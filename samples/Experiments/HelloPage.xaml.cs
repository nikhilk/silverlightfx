// HelloPage.xaml.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightFX.UserInterface;

namespace Experiments {

    public partial class HelloPage : UserControl {

        public HelloPage() {
            InitializeComponent();
        }

        private void OnInputButtonClick(object sender, RoutedEventArgs e) {
            SimpleForm form = new SimpleForm();
            form.Closed += OnSimpleFormClosed;

            form.Show();
        }

        private void OnSimpleFormClosed(object sender, EventArgs e) {
            SimpleForm form = (SimpleForm)sender;
            if (form.Result == FormResult.OK) {
                greetingLabel.Text = "Hello " + form.Input + "!";
            }
        }
    }
}
