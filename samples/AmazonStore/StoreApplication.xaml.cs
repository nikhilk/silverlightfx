// StoreApplication.xaml.cs
//

using System;
using System.Windows;

namespace Store {

    public partial class StoreApplication : Application {

        public StoreApplication() {
            InitializeComponent();
        }

        // HACK: Why is this needed?
        public object FindName(string s) {
            return null;
        }
    }
}
