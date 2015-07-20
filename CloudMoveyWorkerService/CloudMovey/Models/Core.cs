using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using RestSharp.Contrib;
using System.Collections;
using CloudMoveyWorkerService.CloudMovey.Models;
using RestSharp.Deserializers;
using MySerializerNamespace;

namespace CloudMoveyWorkerService.CloudMovey
{
    class Core
    {
        private String _apibase, _username, _password, _endpoint;
        private List<Option> _urloptions = new List<Option>();
        private CloudMovey _client;

        public Core(CloudMovey _CloudMovey)
        {
            _apibase = _CloudMovey.ApiBase;
            _username = _CloudMovey.Username;
            _password = _CloudMovey.Password;
            _client = _CloudMovey;
        }
        public Object post(Object _object) 
        {
            return perform(Method.POST, _object);
        }
        public Object put(Object _object) 
        {
            return perform(Method.PUT, _object);
        }

        public Object perform(Method _method, Object _object)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(apibase);
            var request = new RestRequest();
            request.Resource = endpoint;
            request.Method = _method;
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddJsonBody(_object);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }
            else
            {
                return new Error();
            }
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
