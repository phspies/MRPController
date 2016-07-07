
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{
    public class Events : Core
    {
        public Events(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public eventsPage getEventLogsByFilter_Method(UserEventLogsFilter userEventLogsFilter_object, string direction = null, string sort_by = null, int? page = null, int? pagesize = null)
        {
            endpoint = "/events/filtered";
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }

            mediatype = "application/json";
            return post<eventsPage>(userEventLogsFilter_object);
        }


        public restIntegerSet getTrackedEventIDs_Method()
        {
            endpoint = "/events/tracked";
            mediatype = "application/json";
            return get<restIntegerSet>();
        }


        public eventsPage getEventLogsByUID_Method(long eventId, int? limit = null, string direction = null, string sort_by = null, int? page = null, int? pagesize = null)
        {
            endpoint = "/events/{eventId}";
            endpoint.Replace("{eventId}", eventId.ToString());
            if (limit != null) { url_params.Add(new KeyValuePair<string, string>("limit", limit.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }

            mediatype = "application/json";
            return get<eventsPage>();
        }


        public eventsPage getAllEventLogs_Method(long? minEvent = null, string direction = null, string sort_by = null, int? page = null, int? pagesize = null)
        {
            endpoint = "/events";
            if (minEvent != null) { url_params.Add(new KeyValuePair<string, string>("minEvent", minEvent.ToString())); }
            if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString())); }
            if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString())); }
            if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString())); }
            if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString())); }

            mediatype = "application/json";
            return get<eventsPage>();
        }
    }
}
