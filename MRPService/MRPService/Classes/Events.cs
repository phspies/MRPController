using MRPService.LocalDatabase;
using System;

namespace MRPService.CloudMRP.Classes.Static_Classes
{
    static class CloudMRPEvents
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
