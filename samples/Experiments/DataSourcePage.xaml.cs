// DataSourcePage.xaml.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using SilverlightFX.UserInterface;

namespace Experiments {

    public class DataItem {

        public string Name { get; set; }

        public int Value { get; set; }

        public override string ToString() {
            return Name + " (" + Value + ")";
        }
    }

    public class DataViewModel : ViewModel {

        private List<DataItem> Items = new List<DataItem>() {
            new DataItem() { Name = "Foo" },
            new DataItem() { Name = "FooBar" },
            new DataItem() { Name = "Baz" },
            new DataItem() { Name = "FooBarBaz" }
        };

        public IEnumerable<DataItem> GetItems(string prefix) {
            List<DataItem> items = Items.Where(i => i.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (DataItem dataItem in items) {
                dataItem.Value++;
            }

            return items;
        }
    }

    [ViewModel(typeof(DataViewModel))]
    public partial class DataSourcePage : View {

        public DataSourcePage() {
            InitializeComponent();
        }
    }
}
