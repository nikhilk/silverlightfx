using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Experiments {

    public class NumberModel : Model {

        private int _value;

        public bool CanDecrement {
            get {
                return _value > 0;
            }
        }

        public int Value {
            get {
                return _value;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("value");
                }

                _value = value;
                RaisePropertyChanged("Value");
                RaisePropertyChanged("CanDecrement");
                RaisePropertyChanged("IsSingleDigit");
            }
        }

        public bool IsSingleDigit {
            get {
                return _value < 10;
            }
        }

        public void Add(int number) {
            Value += number;
        }

        public void Decrement() {
            if (Value == 0) {
                throw new InvalidOperationException();
            }
            Value--;
        }

        public void Increment() {
            Value++;
        }
    }

    public partial class ViewModelPage : UserControl {

        public ViewModelPage() {
            InitializeComponent();
        }
    }
}
