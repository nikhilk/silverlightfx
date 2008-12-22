using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SilverlightFX.Applications;
using SilverlightFX.UserInterface;

namespace Experiments {

    public class Person : Model {

        private string _name;
        private int _age;

        public int Age {
            get {
                return _age;
            }
            set {
                _age = value;
                RaisePropertyChanged("Age");
            }
        }

        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
    }

    public class ListModel : Model {

        private ObservableCollection<Person> _items;
        private int _index;

        public ListModel() {
            _items = new ObservableCollection<Person>() {
                new Person() { Name = "Nikhil Kothari", Age = 32 },
                new Person() { Name = "Ishaan Kothari", Age = 0 }
            };
            _index = 0;
        }

        public object CurrentItem {
            get {
                if (_index < 0) {
                    return null;
                }
                return _items[_index];
            }
        }

        public IEnumerable<Person> Items {
            get {
                return _items;
            }
        }

        public void AddItem(string name, int age) {
            _items.Add(new Person() {
                Name = name,
                Age = age
            });
        }

        public void RemoveItem(Person item) {
            bool changeCurrentItem = (CurrentItem == item);
            if (changeCurrentItem) {
                if (_index != 0) {
                    _index--;
                }
            }

            _items.Remove(item);
            if (_index >= _items.Count) {
                _index--;
                changeCurrentItem = true;
            }

            if (changeCurrentItem) {
                RaisePropertyChanged("CurrentItem");
            }
        }
    }

    public partial class DetailViewPage : Window {

        public DetailViewPage() {
            InitializeComponent();
        }
    }
}
