using CloudMoveyWorkerService.CloudMovey.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes
{
    static class CloudMoveyEvents
    {
        public static void add(Event _event)
        {
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            _event.id = Guid.NewGuid().ToString().Replace("-", "").GetSHA1Hash();
            _event.timestamp = DateTime.Now;
            dbcontext.Events.Add(_event);
        }
    }
}
