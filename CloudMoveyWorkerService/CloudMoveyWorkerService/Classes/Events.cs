using CloudMoveyWorkerService.Database;
using System;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
{
    static class CloudMoveyEvents
    {
        public static void add(Event _event)
        {
            _event.timestamp = DateTime.Now;
            LocalData.insert<Event>(_event);
        }
    }
}
