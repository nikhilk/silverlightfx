// ProductView.xaml.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AmazonSearch.Data;

namespace AmazonSearch.Views {

    public class ProductToToolTip : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Product product = (Product)value;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock {
                Text = product.Title,
                FontWeight = FontWeights.Bold
            });
            stackPanel.Children.Add(new TextBlock {
                Text = product.By
            });

            return stackPanel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public partial class ProductView : UserControl {
        public ProductView() {
            InitializeComponent();
        }
    }
}
