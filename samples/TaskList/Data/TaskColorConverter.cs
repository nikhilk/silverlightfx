// TaskColorConverter.cs
//

using System;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskList {

    public class TaskColorConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            TaskStatus status = (TaskStatus)value;

            if (status == TaskStatus.Completed) {
                return new SolidColorBrush(Color.FromArgb(0x80, 0x00, 0x00, 0x00));
            }
            else if (status == TaskStatus.Overdue) {
                return new SolidColorBrush(Colors.Red);
            }
            else {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
