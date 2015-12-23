using CloudMoveyWorkerService.Database;
using System;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
{
    static class CloudMoveyEvents
    {
        public static void add(Event _event)
        {
            LocalData _localdata = new LocalData();

            _event.timestamp = DateTime.Now;
            _localdata.insert_record<Event>(_event);
        }
    }
}
