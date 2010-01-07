// EditTaskForm.Model.cs
//

using System;
using System.ComponentModel;

namespace TaskList {

    public class EditTaskFormModel : FormViewModel {

        private Task _task;
        private Task _originalTask;

        public Task Task {
            get {
                return _task;
            }
        }

        protected override void Commit() {
            _originalTask.Copy(_task);
            Complete(/* commit */ true);
        }

        public void Initialize(Task task) {
            _originalTask = task;
            _task = task.Clone();
        }
    }
}
