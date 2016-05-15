using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MRMPService.DoubleTake
{
    class Event : Core
    {
        public Event(Doubletake doubletake) : base(doubletake) { }

        public async Task<IEnumerable<EventLogModel>> GetAllEntries()
        {
            var api = new EventLogEntriesApi(_target_connection);
            ApiResponse<IEnumerable<EventLogModel>> response = await api.GetEventLogEntriesAsync();
            response.EnsureSuccessStatusCode();
            return response.Content;
        }

        public async Task<IEnumerable<EventLogModel>> GetManagementServiceEntries()
        {
            var api = new EventLogEntriesApi(_target_connection);
            ApiResponse<IEnumerable<EventLogModel>> response = await api.GetEventLogEntriesAsync("source eq 'Double-Take Management Service'");
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<IEnumerable<EventLogModel>> GetDoubleTakeEntries(int? _last_id = null)
        {
            var api = new EventLogEntriesApi(_target_connection);
            ApiResponse<IEnumerable<EventLogModel>> response = await api.GetEventLogEntriesAsync(null, _last_id);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<EventLogModel> GetSingleEntry(int id)
        {
            var api = new EventLogEntriesApi(_target_connection);
            ApiResponse<EventLogModel> response = await api.GetEventLogEntryAsync(id);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
    }
}
