using System;

namespace MRMPService.LocalDatabase
{
    public class ManagerEvent
    {
        public int Id { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }
    }
}
