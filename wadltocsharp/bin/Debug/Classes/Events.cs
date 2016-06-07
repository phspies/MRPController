using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Events : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public EventsPage getEventLogsByFilter_Method(userEventLogsFilter userEventLogsFilter_object,string direction=null,string sort_by=null,int? page=null,int? pagesize=null)
{
	endpoint = "/events/filtered";
if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString()));}
if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString()));}
if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString()));}
if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString()));}

	mediatype="application/json";
	return post<EventsPage>(userEventLogsFilter_object);
}


public RestIntegerSet getTrackedEventIDs_Method()
{
	endpoint = "/events/tracked";
	mediatype="application/json";
	return get<RestIntegerSet>();
}


public EventsPage getEventLogsByUID_Method(long eventId,int? limit=null,string direction=null,string sort_by=null,int? page=null,int? pagesize=null)
{
	endpoint = "/events/{eventId}";
	endpoint.Replace("{eventId}",eventId.ToString());
if (limit != null) { url_params.Add(new KeyValuePair<string, string>("limit", limit.ToString()));}
if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString()));}
if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString()));}
if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString()));}
if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString()));}

	mediatype="application/json";
	return get<EventsPage>();
}


public EventsPage getAllEventLogs_Method(long? minEvent=null,string direction=null,string sort_by=null,int? page=null,int? pagesize=null)
{
	endpoint = "/events";
if (minEvent != null) { url_params.Add(new KeyValuePair<string, string>("minEvent", minEvent.ToString()));}
if (direction != null) { url_params.Add(new KeyValuePair<string, string>("direction", direction.ToString()));}
if (sort_by != null) { url_params.Add(new KeyValuePair<string, string>("sort_by", sort_by.ToString()));}
if (page != null) { url_params.Add(new KeyValuePair<string, string>("page", page.ToString()));}
if (pagesize != null) { url_params.Add(new KeyValuePair<string, string>("pagesize", pagesize.ToString()));}

	mediatype="application/json";
	return get<EventsPage>();
}



}
}
