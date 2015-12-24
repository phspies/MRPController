using CloudMoveyWorkerService.Database;
using System;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
{
    static class CloudMoveyEvents
    {
        public static void add(Event _event)
        {
            LocalDB db = new LocalDB();

            _event.timestamp = DateTime.Now;
            _event.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
            db.Events.Add(_event);
            db.SaveChanges();
        }
    }
}
