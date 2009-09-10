// Task.cs
//

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TaskList {

    public class Task : Model {

        private string _name;
        private DateTime _dueDate;
        private bool _completed;

        private bool _visible;

        public Task() {
            _dueDate = DateTime.Today;
            _visible = true;
        }

        public DateTime DueDate {
            get {
                return _dueDate;
            }
            set {
                _dueDate = value;
                RaisePropertyChanged("DueDate", "Status");
            }
        }

        public bool IsCompleted {
            get {
                return _completed;
            }
            set {
                if (_completed != value) {
                    _completed = value;
                    RaisePropertyChanged("IsCompleted", "Status");
                }
            }
        }

        public bool IsVisible {
            get {
                return _visible;
            }
        }

        public string Name {
            get {
                return _name ?? String.Empty;
            }
            set {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public TaskStatus Status {
            get {
                if (_completed) {
                    return TaskStatus.Completed;
                }
                if (DateTime.Today > _dueDate) {
                    return TaskStatus.Overdue;
                }
                return TaskStatus.Active;
            }
        }

        public Task Clone() {
            return new Task() {
                Name = Name,
                DueDate = DueDate,
                IsCompleted = IsCompleted
            };
        }

        public void Copy(Task task) {
            Name = task.Name;
            DueDate = task.DueDate;
            IsCompleted = task.IsCompleted;
        }

        public void Filter(IPredicate<Task> filter) {
            bool visible = filter.Filter(this);
            if (visible != _visible) {
                _visible = visible;
                RaisePropertyChanged("IsVisible");
            }
        }
    }
}
