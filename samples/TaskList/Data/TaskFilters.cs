// TaskFilters.cs
//

using System;
using System.Collections.Generic;

namespace TaskList {

    public class AllTasksFilter : IPredicate<Task> {

        public bool Filter(Task item) {
            return true;
        }

        public override string ToString() {
            return "All Tasks";
        }
    }

    public class ActiveTasksFilter : IPredicate<Task> {

        public bool Filter(Task item) {
            return item.Status != TaskStatus.Completed;
        }

        public override string ToString() {
            return "Active Tasks";
        }
    }

    public class OverdueTasksFilter : IPredicate<Task> {

        public bool Filter(Task item) {
            return item.Status == TaskStatus.Overdue;
        }

        public override string ToString() {
            return "Overdue Tasks";
        }
    }

    public class TodayTasksFilter : IPredicate<Task> {

        public bool Filter(Task item) {
            return item.DueDate <= DateTime.Today;
        }

        public override string ToString() {
            return "Today's Tasks";
        }
    }
}
