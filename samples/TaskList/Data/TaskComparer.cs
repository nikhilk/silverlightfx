// TaskComparer.cs
//

using System;
using System.Collections;

namespace TaskList {

    public class TaskComparer : IComparer {

        public int Compare(object x, object y) {
            Task t1 = (Task)x;
            Task t2 = (Task)y;

            if (t1.DueDate < t2.DueDate) {
                return -1;
            }
            else if (t1.DueDate > t2.DueDate) {
                return 1;
            }
            else {
                return 0;
            }
        }
    }
}
