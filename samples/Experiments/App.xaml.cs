// App.xaml.cs
//

using System;
using System.Windows;
using SilverlightFX.Applications;

namespace Experiments {

    public partial class App : XApplication {

        public App() {
            InitializeComponent();
        }

        public new static App Current {
            get {
                return Application.Current as App;
            }
        }
    }
}
