using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace CloudMoveyWorkerService.CloudMovey
{
    class Core
    {
        private String _apibase, _endpoint;
        private CloudMovey _client;

        public Core(CloudMovey _CloudMovey)
        {
            _apibase = _CloudMovey.ApiBase;
            _client = _CloudMovey;
        }

        public Object post<type>(Object _object) where type : new()
        {
            return perform<type>(Method.POST, _object);
        }
        public Object put<type>(Object _object) where type : new()
        {
            return perform<type>(Method.PUT, _object);
        }

        public object perform<type>(Method _method, Object _object) where type : new()
        {
            var client = new RestClient();
            client.FollowRedirects = false;
            client.BaseUrl = new Uri(apibase);
            RestRequest request = new RestRequest();
            client.FollowRedirects = false;
            request.Resource = endpoint;
            request.Method = _method;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.JsonSerializer = new JsonSerializer();
            request.AddJsonBody(_object);

            client.RemoveDefaultParameter("Accept");
            client.AddDefaultParameter("Accept", "application/json", RestSharp.ParameterType.HttpHeader);
            object responseobject = null;
            while (true)
            {
                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try {
                        responseobject = JsonConvert.DeserializeObject<type>(response.Content);
                        
                    }
                    catch (Exception ex)
                    {
                        Global.event_log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                        Global.event_log.WriteEntry(JsonConvert.SerializeObject(_object), EventLogEntryType.Error);
                        Global.event_log.WriteEntry(response.Content, EventLogEntryType.Error);
                        break;
                    }
                    break;
                }
                else if (response.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    Global.event_log.WriteEntry(String.Format("Connection timeout to {0}", client.BuildUri(request).ToString()), EventLogEntryType.Error);
                    Thread.Sleep(new TimeSpan(0,0,30));
                }
                else if (response.StatusCode == 0)
                {
                    Global.event_log.WriteEntry(String.Format("Unexpected error connecting to {0} with error ({1})", client.BuildUri(request).ToString(), response.ErrorMessage), EventLogEntryType.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else
                {
                    Global.event_log.WriteEntry(String.Format("Unexpected API error on {0} with error ({1})", client.BuildUri(request).ToString(), response.Content), EventLogEntryType.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
            }
            return responseobject;

        }
        public String apibase
        {
            get
            {
                return this._apibase;
            }
            set
            {
                this._apibase = value;
            }
        }
        public String endpoint
        {
            get
            {
                return this._endpoint;
            }
            set
            {
                this._endpoint = value;
            }
        }
    }
}
