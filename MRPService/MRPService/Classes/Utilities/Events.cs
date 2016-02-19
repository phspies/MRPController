using MRPService.LocalDatabase;
using System;

namespace MRPService.Utilities
{
    static class MRPEvents
    {
        public static void add(Event _event)
        {
            MRPDatabase db = new MRPDatabase();

            _event.timestamp = DateTime.Now;
            _event.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
            db.Events.Add(_event);
            db.SaveChanges();
        }
    }
}
