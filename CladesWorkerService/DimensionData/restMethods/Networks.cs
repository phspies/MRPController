using cloudManage.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DoubleTakeProxyService.DimensionData.restMethods
{
    class Networks
    {
        public int httpResponseCode;
        public String returnMessage;
        public List<NetworkDataModel> _networks = new List<NetworkDataModel>();

        public bool networkList()
        {
            httpPost networkList = new httpPost();
            networkList.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/network";
            networkList.httpMethod = "GET";

            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            networkList.postData = input_xdoc.ToString();
            networkList.doPost();
            httpResponseCode = networkList.responseCode;
            if (networkList.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(networkList.responseData);
                var ns = xdoc.Root.Name.Namespace;
                foreach (XElement xe in xdoc.Root.Elements())
                {
                    String description = "";
                    if (xe.Element(ns + "description") != null)
                    {
                        description = description = xe.Element(ns + "description").Value.Replace(System.Environment.NewLine, "");
                    }

                    _networks.Add(new NetworkDataModel()
                    {
                        id = xe.Element(ns + "id").Value,
                        name = xe.Element(ns + "name").Value,
                        description = description,
                        resourcePath = xe.Element(ns + "resourcePath").Value,
                        multicast = xe.Element(ns + "multicast").Value
                    });
                }
                return true;
            }
            else
            {
                returnMessage = networkList.responseMessage;
                return false;
            }
        }
    }
}
