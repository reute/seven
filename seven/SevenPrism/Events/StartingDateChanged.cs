using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Events
{
    public class DateSelectedChangedEvent : PubSubEvent<TimePeriod>
    {}

    public struct TimePeriod
    {
        public DateTime FromDate, ToDate;

        public TimePeriod(DateTime from, DateTime to)
        {
            FromDate = from;
            ToDate = to;
        }
    }
}
