using RestSharp;
using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Types.API;

namespace MRMPService.MRMPAPI
{
    class Core
    {
        private String _endpoint;
        private MRMP_ApiClient _client;
        static string api_prefix = "/api/v1";

        public Core(MRMP_ApiClient _MRP)
        {
            _client = _MRP;
        }

        public type post<type>(Object _object) where type : new()
        {
            return (type)perform<type>(Method.POST, _object);
        }
        public type put<type>(Object _object) where type : new()
        {
            return (type)perform<type>(Method.PUT, _object);
        }

        public object perform<type>(Method _method, Object _object) where type : new()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            var client = new RestClient();
            client.FollowRedirects = false;
            client.BaseUrl = new Uri("https://www.mrplatform.net");
            RestRequest request = new RestRequest();
            client.FollowRedirects = false;
            request.Resource = endpoint;
            request.Method = _method;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.AddHeader("Accept-Encoding", "gzip");
            request.JsonSerializer = new JsonSerializer();
            request.AddJsonBody(_object);
            request.Timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            client.RemoveDefaultParameter("Accept");
            client.AddDefaultParameter("Accept", "application/json", ParameterType.HttpHeader);
            object responseobject = null;
            while (true)
            {
                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized)
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
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    try
                    {
                        responseobject = JsonConvert.DeserializeObject<ResultType>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex.ToString(), Logger.Severity.Error);
                        Logger.log(JsonConvert.SerializeObject(_object), Logger.Severity.Error);
                        Logger.log(response.Content, Logger.Severity.Error);
                        break;
                    }
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
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    try
                    {
                        responseobject = JsonConvert.DeserializeObject<ResultType>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex.ToString(), Logger.Severity.Error);
                        Logger.log(JsonConvert.SerializeObject(_object), Logger.Severity.Error);
                        Logger.log(response.Content, Logger.Severity.Error);
                        break;
                    }
                    throw new System.ArgumentException(((ResultType)responseobject).result.message);
                }
                else
                {
                    Logger.log(String.Format("Unexpected API error on {0} with error ({1})", client.BuildUri(request).ToString(), response.ErrorMessage), Logger.Severity.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
            }
            return responseobject;

        }

        public String endpoint
        {
            get
            {
                return api_prefix + this._endpoint;
            }
            set
            {
                this._endpoint = value;
            }
        }
    }
}
