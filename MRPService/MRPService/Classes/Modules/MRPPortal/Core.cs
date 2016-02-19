using RestSharp;
using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using MRPService.MRPService.Log;

namespace MRPService.API
{
    class Core
    {
        private String _apibase, _endpoint;
        private ApiClient _client;

        public Core(ApiClient _CloudMRP)
        {
            _apibase = _CloudMRP.ApiBase;
            _client = _CloudMRP;
        }

        public type post<type>(Object _object) where type : new()
        {
            return (type)perform<type>(Method.POST, _object);
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
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    try {
                        responseobject = JsonConvert.DeserializeObject<type>(response.Content);
                        
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex.ToString(), Logger.Severity.Error);
                        Logger.log(JsonConvert.SerializeObject(_object), Logger.Severity.Error);
                        Logger.log(response.Content, Logger.Severity.Error);
                        break;
                    }
                    break;
                }
                else if (response.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    Logger.log(String.Format("Connection timeout to {0}", client.BuildUri(request).ToString()), Logger.Severity.Error);
                    Thread.Sleep(new TimeSpan(0,0,30));
                }
                else if (response.StatusCode == 0)
                {
                    Logger.log(String.Format("Unexpected error connecting to {0} with error ({1})", client.BuildUri(request).ToString(), response.ErrorMessage), Logger.Severity.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else
                {
                    Logger.log(String.Format("Unexpected API error on {0} with error ({1})", client.BuildUri(request).ToString(), response.ErrorMessage), Logger.Severity.Error);
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
