using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Events
{
    class DateSelectedChangedEvent : PubSubEvent<DateTime>
    {
        internal void Subscribe()
        {
            throw new NotImplementedException();
        }
    }
}
