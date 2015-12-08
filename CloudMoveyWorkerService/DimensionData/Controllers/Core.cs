using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Collections;
using RestSharp.Authenticators;

namespace CloudMoveyWorkerService.CaaS
{
    class Core
    {
        private String _apibase, _username, _password, _datacenter, _endpoint;
        private List<Option> _urloptions = new List<Option>();
        private DimensionData _client;
        public ResponseType _lastresponse;

        public Core(DimensionData _dimensiondata)
        {
            _apibase = _dimensiondata.ApiBase;
            _username = _dimensiondata.Username;
            _password = _dimensiondata.Password;
            _datacenter = _dimensiondata.Datacenter;
            _client = _dimensiondata;
        }
        public Boolean orgendpoint(String _endpoint)
        {
            if (_client.OrganizationId == null)
            {
                var response = _client.account().myaccount();
                if (response.GetType() == typeof(Account))
                {
                    _client.OrganizationId = (response as Account).orgId;
                    this.endpoint = "/caas/2.0/" + _client.OrganizationId + _endpoint;
                    return true;
                }
                else
                {
                    _lastresponse = (ResponseType)response;
                    return false;
                }
            }
            this.endpoint = "/caas/2.0/" + _client.OrganizationId + _endpoint;
            return false;
        }
        public void simpleendpoint(String _endpoint)
        {
            this.endpoint = "/oec/0.9" + _endpoint;
        }

        public Object get<type>(Object _object, bool _simple) where type : new()
        {
            return perform<type>(Method.GET, _object, _simple);
        }
        public Object post<type>(Object _object, bool _simple) where type : new()
        {
            return perform<type>(Method.POST, _object, _simple);
        }
        public Object put<type>(Object _object, bool _simple) where type : new()
        {
            return perform<type>(Method.PUT, _object, _simple);
        }

        public Object perform<type>(Method _method, Object _object, bool _simple = false) where type : new()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(apibase);
            client.FollowRedirects = false;
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest();
            request.Resource = endpoint;
            request.RequestFormat = DataFormat.Xml;
            request.Method = _method;
            if (urloptions != null)
            {
                foreach (Option option in urloptions) {
                    request.AddQueryParameter(option.option, option.value);
                }
            }
            if (_simple)
            {
                if (!Object.ReferenceEquals(null, _object))
                {
                    Global.event_log.WriteEntry(_object.ToQueryString());
                    request.AddParameter("application/x-www-form-urlencoded", _object.ToQueryString(), ParameterType.RequestBody);
                }
            }
            else
            {
                request.AddParameter("application/xml", SerializeObject(_object), ParameterType.RequestBody);
            }
            client.RemoveDefaultParameter("Accept");
            client.AddDefaultParameter("Accept", "application/xml,text/xml", RestSharp.ParameterType.HttpHeader);
            
            var response = client.Execute(request);
            var serializer = new XmlSerializer(typeof(type));

            var responseobject = new Object();
            
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                using (var reader = new StringReader(response.Content))
                {
                    try
                    {
                        responseobject = (type)serializer.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {
                        Global.event_log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                        Global.event_log.WriteEntry(response.Content, EventLogEntryType.Error);
                    }
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                {
                    Global.event_log.WriteEntry("Access Denied Error",EventLogEntryType.Error);
                    responseobject =  new ResponseType() { message = "Access Denied" };
                }
            }
            return responseobject;
        }
        public String username
        {
            get
            {
                return this._username;
            }
            set
            {
                this._username = value;
            }
        }
        public String password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
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
        public List<Option> urloptions
        {
            get
            {
                return this._urloptions;
            }
            set
            {
                this._urloptions = value;
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
        public string SerializeObject(object obj)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlDoc.Load(ms);
                return xmlDoc.InnerXml;
            }
        }
    
    }
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }
    }
    public static class UrlHelpers
    {
        public static string ToQueryString(this object request, string separator = ",")
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }
    }

}
