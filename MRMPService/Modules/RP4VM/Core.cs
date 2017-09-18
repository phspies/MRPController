using RestSharp;
using System;
using System.Net;
using System.Threading;
using MRMPService.MRMPService.Log;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.RP4VMAPI;
using System.Collections.Generic;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MRMPService.Utilities;

namespace MRMPService.RP4VMTypes
{
    public class Core
    {
        private String _endpoint;
        private RP4VM_ApiClient _client;
        static string api_prefix = "/fapi/rest/4_3";
        public string mediatype = "";
        public List<KeyValuePair<string, string>> url_params = new List<KeyValuePair<string, string>>();

        public Core(RP4VM_ApiClient _RP4VM)
        {
            _client = _RP4VM;
        }

        public void post(Object _object)
        {
            object _temp = perform<restString>(Method.POST, _object);
        }
        public void put(Object _object)
        {
            object _temp = perform<restString>(Method.PUT, _object);
        }

        public void post()
        {
            object _temp = perform<restString>(Method.POST, null);
        }
        public void put()
        {
            object _temp = perform<restString>(Method.PUT, null);
        }
        public void delete()
        {
            object _temp = perform<restString>(Method.DELETE, null);
        }
        public void delete(Object _object)
        {
            object _temp = perform<restString>(Method.DELETE, _object);
        }
        public type post<type>() where type : new()
        {
            return (type)perform<type>(Method.POST, null);
        }
        public type post<type>(Object _object) where type : new()
        {
            return (type)perform<type>(Method.POST, _object);
        }
        public type put<type>(Object _object) where type : new()
        {
            return (type)perform<type>(Method.PUT, _object);
        }
        public type get<type>() where type : new()
        {
            return (type)perform<type>(Method.GET, null);
        }

        public object perform<type>(Method _method, Object _object) where type : new()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            var client = new RestClient();
            client.FollowRedirects = false;
            client.BaseUrl = new Uri(_client.url);
            client.Authenticator = new HttpBasicAuthenticator(_client.username, _client.password);
            client.FollowRedirects = false;

            RestRequest request = new RestRequest();
            request.Resource = endpoint;
            request.Method = _method;
            //request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = String.Format("{0}; charset=utf-8", mediatype);
            request.JsonSerializer = new RestSharp.Serializers.JsonSerializer();
            request.AddJsonBody(_object);
            if (url_params.Count > 0)
            {
                foreach (var _param in url_params)
                {
                    request.AddParameter(_param.Key, _param.Value);
                }
            }

            //client.RemoveDefaultParameter("Accept");
            //client.AddDefaultParameter("Accept", "application/json", RestSharp.ParameterType.HttpHeader);
            object responseobject = null;
            while (true)
            {
                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    try
                    {
                        responseobject = JsonConvert.DeserializeObject<type>(response.Content, new JsonSerializerSettings
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore,
                            Error = HandleDeserializationError,
							StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                        });
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
                    Logger.log(JsonConvert.SerializeObject(_object), Logger.Severity.Error);
                    Logger.log(response.Content, Logger.Severity.Error);
                    throw new Exception(response.Content);
                }
                else if (response.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    Logger.log(String.Format("Connection timeout to {0}", client.BuildUri(request).ToString()), Logger.Severity.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
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
                    throw new System.ArgumentException((String.Join(",", ((ResultType)responseobject).result.message)));
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
        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }

}
