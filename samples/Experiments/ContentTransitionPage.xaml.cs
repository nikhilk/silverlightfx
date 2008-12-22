using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SilverlightFX.Applications;

namespace Experiments {

    public partial class ContentTransitionPage : UserControl {

        public ContentTransitionPage() {
            InitializeComponent();
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e) {
            if (contentControl.Content is Button) {
                contentControl.Content = new Border {
                    Background = new SolidColorBrush(Colors.Black),
                    Child = new TextBlock {
                        Text = "Hello!",
                        Foreground = new SolidColorBrush(Colors.White)
                    },
                    Width = 300,
                    Height = 300
                };
            }
            else {
                contentControl.Content = new Button {
                    Content = "Click Me!",
                    Width = 300,
                    Height = 300
                };
            }
        }
    }
}
