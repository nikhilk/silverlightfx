// TaskStorage.cs
//

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace TaskList {

    internal static class TaskStorage {

        public static Task[] LoadTasks() {
            Task[] tasks = null;

            try {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                if (storage.FileExists("Tasks.xml")) {
                    using (Stream tasksStream = storage.OpenFile("Tasks.xml", FileMode.Open)) {
                        DataContractSerializer reader = new DataContractSerializer(typeof(Task[]));

                        tasks = (Task[])reader.ReadObject(tasksStream);

                        IPredicate<Task> filter = new AllTasksFilter();
                        foreach (Task task in tasks) {
                            task.Filter(filter);
                        }
                    }
                }
            }
            catch {
            }

            return tasks;
        }

        public static void SaveTasks(Task[] tasks) {
            try {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

                using (Stream tasksStream = storage.OpenFile("Tasks.xml", FileMode.Create)) {
                    DataContractSerializer writer = new DataContractSerializer(typeof(Task[]));

                    writer.WriteObject(tasksStream, tasks);
                    tasksStream.Flush();
                }
            }
            catch {
            }
        }
    }
}
