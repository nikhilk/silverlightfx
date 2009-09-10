// NumberEditForm.xaml.cs
//

using System;
using System.Windows.Controls;
using SilverlightFX.UserInterface;
using System.ComponentModel;

namespace Experiments {

    public class NumberEditModel : TaskViewModel {

        private int _number;

        public NumberEditModel(int number) {
            _number = number;
        }

        public int Number {
            get {
                return _number;
            }
            set {
                _number = value;
                RaisePropertyChanged("Number");
            }
        }

        protected override void Commit() {
            if (_number != 0) {
                Complete(/* commit */ true);
            }
        }
    }

    public partial class NumberEditForm : Form {

        public NumberEditForm(NumberEditModel model)
            : base(model) {
            InitializeComponent();
        }
    }
}
