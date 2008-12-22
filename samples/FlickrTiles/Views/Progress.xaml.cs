// Progress.xaml.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FlickrTiles.Views {

    public partial class Progress : UserControl {

        public static DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Progress),
                                        new PropertyMetadata(OnIsActivePropertyChanged));

        public Progress() {
            InitializeComponent();
        }

        public bool IsActive {
            get {
                return (bool)GetValue(IsActiveProperty);
            }
            set {
                SetValue(IsActiveProperty, value);
            }
        }

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Progress p = (Progress)d;
            Canvas progressSpinner = p.progressSpinner;
            Storyboard storyboard = (Storyboard)progressSpinner.Resources["spinStoryboard"];

            if ((bool)e.NewValue) {
                progressSpinner.Visibility = Visibility.Visible;
                storyboard.Begin();
            }
            else {
                storyboard.Stop();
                progressSpinner.Visibility = Visibility.Collapsed;

                p.rootElement.Background = null;
            }
        }
    }
}
