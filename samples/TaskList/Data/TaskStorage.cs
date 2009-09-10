// TaskStorage.cs
//

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace TaskList {

    public class TaskDTO {

        public DateTime DueDate {
            get;
            set;
        }

        public bool IsCompleted {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }
    }

    internal static class TaskStorage {

        public static Task[] LoadTasks() {
            Task[] tasks = null;

            try {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                if (storage.FileExists("Tasks.xml")) {
                    using (Stream tasksStream = storage.OpenFile("TaskList.xml", FileMode.Open)) {
                        DataContractSerializer reader = new DataContractSerializer(typeof(TaskDTO[]));

                        TaskDTO[] taskObjects = (TaskDTO[])reader.ReadObject(tasksStream);

                        tasks = new Task[taskObjects.Length];
                        for (int i = 0; i < taskObjects.Length; i++) {
                            tasks[i] = new Task() {
                                Name = taskObjects[i].Name,
                                DueDate = taskObjects[i].DueDate,
                                IsCompleted = taskObjects[i].IsCompleted
                            };
                        }

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

                using (Stream tasksStream = storage.OpenFile("TaskList.xml", FileMode.Create)) {
                    DataContractSerializer writer = new DataContractSerializer(typeof(TaskDTO[]));

                    TaskDTO[] taskObjects = new TaskDTO[tasks.Length];
                    for (int i = 0; i < tasks.Length; i++) {
                        taskObjects[i] = new TaskDTO() {
                            Name = tasks[i].Name,
                            DueDate = tasks[i].DueDate,
                            IsCompleted = tasks[i].IsCompleted
                        };
                    }

                    writer.WriteObject(tasksStream, taskObjects);
                    tasksStream.Flush();
                }
            }
            catch {
            }
        }
    }
}
