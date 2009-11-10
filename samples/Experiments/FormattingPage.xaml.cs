using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Experiments {

    public class Employee {
        public string FirstName {
            get;
            set;
        }
        public string LastName {
            get;
            set;
        }
        public int Number {
            get;
            set;
        }
        public DateTime JoinDate {
            get;
            set;
        }
    }

    public partial class FormattingPage : UserControl {
        public FormattingPage() {
            InitializeComponent();

            Employee emp = new Employee() {
                FirstName = "Nikhil",
                LastName = "Kothari",
                Number = 123,
                JoinDate = new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            DataContext = emp;
        }
    }
}
