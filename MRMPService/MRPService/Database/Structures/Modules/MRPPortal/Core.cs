using RestSharp;
using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using MRMPService.MRMPService.Log;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Utilities;

namespace MRMPService.Modules.MRMPPortal
{
    public class Core
    {
        private String _endpoint;
        private MRMPApiClient _client;
        private string api_prefix = "/api/v1";

        public Core(MRMPApiClient _mrmp_base)
        {
            _client = _mrmp_base;
        }

        public type post<type>(object _object) where type : new()
        {
            return (type)perform<type>(Method.POST, _object);
        }
        public type put<type>(object _object) where type : new()
        {
            return (type)perform<type>(Method.PUT, _object);
        }
        public type get<type>(object _object) where type : new()
        {
            return (type)perform<type>(Method.GET, _object);
        }

        public object perform<type>(Method _method, Object _object) where type : new()
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            var client = new RestClient();
            client.FollowRedirects = false;
            client.BaseUrl = new Uri(MRMPServiceBase.api_base);
            RestRequest request = new RestRequest();
            client.FollowRedirects = false;
            client.Proxy = null;
            request.Resource = endpoint;

            request.Method = _method;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.AddHeader("Accept-Encoding", "gzip");
            request.JsonSerializer = new JsonSerializer();
            if (_method == Method.GET)
            {
                request.AddObject(_object);
            }
            else
            {
                request.AddJsonBody(_object);
            }
            request.Timeout = (int)TimeSpan.FromSeconds(240).TotalMilliseconds;
            client.Timeout = (int)TimeSpan.FromSeconds(240).TotalMilliseconds;

            client.RemoveDefaultParameter("Accept");
            client.AddDefaultParameter("Accept", "application/json", ParameterType.HttpHeader);
            client.AddDefaultParameter("MANAGERID", MRMPServiceBase.manager_id, ParameterType.HttpHeader);
            client.AddDefaultParameter("ORGANIZATIONID", MRMPServiceBase.organization_id, ParameterType.HttpHeader);

            object responseobject = null;
            while (true)
            {
                var restResponse = client.Execute(request);
                if (restResponse.StatusCode == HttpStatusCode.OK || restResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    try
                    {
                        responseobject = JsonConvert.DeserializeObject<type>(restResponse.Content, new JsonSerializerSettings
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
                        Logger.log(restResponse.Content, Logger.Severity.Error);
                        throw new Exception(ex.GetBaseException().Message);
                    }
                    break;
                }
                else if (restResponse.StatusCode == HttpStatusCode.BadRequest || restResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    ResultType _result = new ResultType();
                    try
                    {
                        _result = JsonConvert.DeserializeObject<ResultType>(restResponse.Content);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex.ToString(), Logger.Severity.Error);
                        Logger.log(restResponse.Content, Logger.Severity.Error);
                        throw new Exception(_result.result.ToString());
                    }
                    Logger.log(String.Format("{0} {1}", _result.result.ToString(), restResponse.Content), Logger.Severity.Error);
                    throw new Exception(_result.result.ToString());

                }
                else if (restResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    Logger.log(String.Format("Connection timeout to {0}", client.BuildUri(request).ToString()), Logger.Severity.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else if (restResponse.StatusCode == 0)
                {
                    Logger.log(String.Format("Unexpected error connecting to {0} with error ({1})", client.BuildUri(request).ToString(), restResponse.ErrorMessage), Logger.Severity.Error);

                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else
                {
                    Logger.log(String.Format("Unexpected API error on {0} with error ({1})", client.BuildUri(request).ToString(), restResponse.ErrorMessage), Logger.Severity.Error);
                    throw new Exception(String.Format("{0} {1}", restResponse.StatusCode, restResponse.ErrorMessage));
                }
            }
            client = null;
            request = null;
            return responseobject;

        }
        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            Logger.log(String.Format("JSON Deserialization Error: {0}", currentError), Logger.Severity.Fatal);
            errorArgs.ErrorContext.Handled = true;
        }

        public String endpoint
        {
            get
            {
                return api_prefix + _endpoint;
            }
            set
            {
                _endpoint = value;
            }
        }
    }
}
