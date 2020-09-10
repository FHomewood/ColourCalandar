using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Scheduler
{
    public class DaySchedule
    {
        DateTime day;
        Color[] cols;
        DateTime[] times;
        string[] tasks;

        public DaySchedule(string line)
        {
            string[] data = line.Split(',');
            day = new DateTime(
                int.Parse(data[0].Substring(6, 4)),
                int.Parse(data[0].Substring(3, 2)),
                int.Parse(data[0].Substring(0, 2))
                );
            cols = new Color[data.Length - 1];
            times = new DateTime[data.Length - 1];
            tasks = new string[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
            {
                times[i-1] = day.AddHours(int.Parse(data[i].Substring(0, 2))).AddMinutes(int.Parse(data[i].Substring(3, 2)));
                cols[i-1] = (Color)ColorConverter.ConvertFromString("#" + data[i].Substring(6,6));
                tasks[i-1] = data[i].Substring(13);

            }
        }
        public DateTime Day
        {
            get { return day; }
        }
        public DateTime[] Time
        {
            get { return times; }
        }
        public string[] Task
        {
            get { return tasks; }
        }public Color[] Colour
        {
            get { return cols; }
        }
    }
}
