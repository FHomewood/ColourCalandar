using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class DaySchedule
    {
        DateTime day;
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
            times = new DateTime[data.Length - 1];
            tasks = new string[data.Length - 1];
            for (int i = 0; i < data.Length-1; i++)
            {
                times[i] = day.AddHours(int.Parse(data[i + 1].Substring(0, 2))).AddMinutes(int.Parse(data[i + 1].Substring(3, 2)));
                tasks[i] = data[i + 1].Substring(6);
            }
        }
    }
}
