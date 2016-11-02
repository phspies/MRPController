using RestSharp;
using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Types.API;
using Newtonsoft.Json.Serialization;

namespace MRMPService.MRMPAPI
{
    class Core : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Core()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
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
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 20;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            var client = new RestClient();
            client.FollowRedirects = false;
            client.Timeout = 60 * 1000;
            client.BaseUrl = new Uri("https://www.mrplatform.net/");
            RestRequest request = new RestRequest();
            client.FollowRedirects = false;
            client.Proxy = null;
            request.Resource = endpoint;
            request.Timeout = 60 * 1000;
            request.Method = _method;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.AddHeader("Accept-Encoding", "gzip");
            request.JsonSerializer = new JsonSerializer();
            request.AddJsonBody(_object);
            request.Timeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            client.RemoveDefaultParameter("Accept");
            client.AddDefaultParameter("Accept", "application/json", ParameterType.HttpHeader);
            client.AddDefaultParameter("MANAGERID", Global.manager_id, ParameterType.HttpHeader);
            client.AddDefaultParameter("ORGANIZATIONID", Global.organization_id, ParameterType.HttpHeader);

            object responseobject = null;
            while (true)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    try {
                        responseobject = JsonConvert.DeserializeObject<type>(response.Content, new JsonSerializerSettings
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore,
                            Error = HandleDeserializationError
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
                    try
                    {
                        responseobject = JsonConvert.DeserializeObject<ResultType>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex.ToString(), Logger.Severity.Error);
                        Logger.log(JsonConvert.SerializeObject(_object), Logger.Severity.Error);
                        Logger.log(response.Content, Logger.Severity.Error);
                        throw new Exception(String.Format("Error in API call: {0}", ex.GetBaseException().Message));
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
            client = null;
            request = null;
            return responseobject;

        }
        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
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
