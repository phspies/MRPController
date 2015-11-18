using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Collections;
using CloudMoveyWorkerService.CaaS.Models;

namespace CloudMoveyWorkerService.DoubleTakeNS
{
    class Core
    {
        private String _apibase, _username, _password, _datacenter, _endpoint;
        private List<Option> _urloptions = new List<Option>();


        public Core(DT _dimensiondata)
        {
            _apibase = _dimensiondata.ApiBase;
            _username = _dimensiondata.Username;
            _password = _dimensiondata.Password;
            _datacenter = _dimensiondata.Datacenter;
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
