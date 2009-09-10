// EditTaskForm.cs
//

using System;
using System.Windows.Controls;
using SilverlightFX.UserInterface;

namespace TaskList {

    public partial class EditTaskForm : Form {

        public EditTaskForm(EditTaskFormModel model)
            : base(model) {
            InitializeComponent();
        }
    }
}
