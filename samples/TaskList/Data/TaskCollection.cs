// TaskCollection.cs
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaskList {

    public class TaskCollection : ObservableCollection<Task> {

        public TaskCollection(Task[] items) {
            if (items != null) {
                for (int i = 0; i < items.Length; i++) {
                    Add(items[i]);
                }
            }
        }

        public Task[] ToArray() {
            Task[] items = new Task[Count];
            for (int i = 0; i < Count; i++) {
                items[i] = this[i];
            }

            return items;
        }
    }
}
