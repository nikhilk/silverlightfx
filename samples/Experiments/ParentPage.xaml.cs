using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SilverlightFX.UserInterface;

namespace Experiments {

    public class ParentPageModel : ViewModel {

        private int _value;

        public int Value {
            get {
                return _value;
            }
            private set {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        public NumberEditModel CreateNumberEditTask() {
            NumberEditModel model = new NumberEditModel(Value);
            model.Completed += delegate(object sender, EventArgs e) {
                if (model.IsCommitted) {
                    Value = model.Number;
                }
            };

            return model;
        }
    }

    public partial class ParentPage : ViewUserControl {

        public ParentPage() {
            InitializeComponent();
        }
    }
}
