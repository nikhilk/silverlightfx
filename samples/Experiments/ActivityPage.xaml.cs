// ActivityPage.xaml.cs
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using SilverlightFX.UserInterface;

namespace Experiments {

    public class ActivityViewModel : ViewModel {

        private Async _activity;

        public Async Activity {
            get {
                return _activity;
            }
            private set {
                _activity = value;
                RaisePropertyChanged("Activity");
            }
        }

        public void Start() {
            Async async = new Async<int>();
            async.Message = "Test Async Activity";

            Activity = async;
        }

        public void StartCancelable() {
            Async async = new Async<int>(OnCancel, null);
            async.Message = "Test Cancelable Async Activity";

            Activity = async;
        }

        public void Stop() {
            ((Async<int>)Activity).Complete(0);
        }

        public void StopError() {
            ((Async<int>)Activity).Complete(new InvalidOperationException("Error"));
        }

        private void OnCancel(Async<int> async, object taskState) {
        }
    }

    public partial class ActivityPage : View {

        public ActivityPage() {
            InitializeComponent();
        }
    }
}
