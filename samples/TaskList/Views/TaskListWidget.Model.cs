// TaskListWidget.Model.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TaskList {

    public class TaskListWidgetModel : Model {

        private static readonly IPredicate<Task>[] filters = new IPredicate<Task>[] {
            new AllTasksFilter(),
            new ActiveTasksFilter(),
            new OverdueTasksFilter(),
            new TodayTasksFilter()
        };

        private TaskCollection _tasks;
        private Task _newTask;

        private IPredicate<Task> _selectedFilter;

        public TaskListWidgetModel() {
            Task[] tasks = TaskStorage.LoadTasks();

            _tasks = new TaskCollection(tasks);
            _selectedFilter = filters[0];
        }

        public IPredicate<Task>[] Filters {
            get {
                return filters;
            }
        }

        public Task NewTask {
            get {
                if (_newTask == null) {
                    _newTask = new Task();
                }
                return _newTask;
            }
        }

        public IPredicate<Task> SelectedFilter {
            get {
                return _selectedFilter;
            }
            set {
                if (_selectedFilter != value) {
                    _selectedFilter = value;
                    RaisePropertyChanged("SelectedFilter");

                    foreach (Task task in _tasks) {
                        task.Filter(_selectedFilter);
                    }
                }
            }
        }

        public IComparer SelectedSort {
            get {
                return new TaskComparer();
            }
        }

        public IEnumerable<Task> Tasks {
            get {
                return _tasks;
            }
        }

        public void AddTask() {
            _tasks.Add(NewTask);
            TaskStorage.SaveTasks(_tasks.ToArray());

            _newTask = null;
            RaisePropertyChanged("NewTask");
        }

        public void CompleteTask(Task task) {
            task.IsCompleted = !task.IsCompleted;
            TaskStorage.SaveTasks(_tasks.ToArray());
        }

        public void DeleteTask(Task task) {
            _tasks.Remove(task);
            TaskStorage.SaveTasks(_tasks.ToArray());
        }
    }
}
