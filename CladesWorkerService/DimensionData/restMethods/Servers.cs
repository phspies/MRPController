using cloudManage.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DoubleTakeProxyService.DimensionData.restMethods
{
    class Servers
    {
        public int httpResponseCode;
        public String returnMessage;
        public List<ServerDataModel> _servers = new List<ServerDataModel>();
        public Boolean network_filter;
        public String networkId;
        public String server_id;


        public Boolean serverPowerOn()
        {

            httpPost serverList = new httpPost();
            serverList.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/server/"+ server_id + "?start";
            serverList.httpMethod = "GET";
            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            serverList.postData = input_xdoc.ToString();
            serverList.doPost();
            httpResponseCode = serverList.responseCode;
            if (serverList.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(serverList.responseData);
                var ns = xdoc.Root.Name.Namespace;
                
                if (xdoc.Root.Element(ns + "result").Value == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    returnMessage = xdoc.Root.Element(ns + "resultCode").Value;
                    return false;
                }
            }
            else
            {
                returnMessage = serverList.responseMessage;
                return false;
            }
        }
        public Boolean serverPowerOff()
        {

            httpPost serverList = new httpPost();
            serverList.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/server/" + server_id + "?shutdown";
            serverList.httpMethod = "GET";
            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            serverList.postData = input_xdoc.ToString();
            serverList.doPost();
            httpResponseCode = serverList.responseCode;
            if (serverList.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(serverList.responseData);
                var ns = xdoc.Root.Name.Namespace;

                if (xdoc.Root.Element(ns + "result").Value == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    returnMessage = xdoc.Root.Element(ns + "resultCode").Value;
                    return false;
                }
            }
            else
            {
                returnMessage = serverList.responseMessage;
                return false;
            }
        }
        public Boolean serverDelete()
        {
            httpPost serverList = new httpPost();
            serverList.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/server/" + server_id + "?delete";
            serverList.httpMethod = "GET";
            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            serverList.postData = input_xdoc.ToString();
            serverList.doPost();
            httpResponseCode = serverList.responseCode;
            if (serverList.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(serverList.responseData);
                var ns = xdoc.Root.Name.Namespace;

                if (xdoc.Root.Element(ns + "result").Value == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    returnMessage = xdoc.Root.Element(ns + "resultCode").Value;
                    return false;
                }
            }
            else
            {
                returnMessage = serverList.responseMessage;
                return false;
            }
        }
        public Boolean serverList()
        {

            httpPost serverList = new httpPost();
            serverList.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/serverWithState?";
            if (network_filter)
            {
                serverList.url += "networkId=" + networkId;
            }
            serverList.httpMethod = "GET";
            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            serverList.postData = input_xdoc.ToString();
            serverList.doPost();
            httpResponseCode = serverList.responseCode;
            if (serverList.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(serverList.responseData);
                var ns = xdoc.Root.Name.Namespace;
                List<ServerDiskDataModel> _disks = new List<ServerDiskDataModel>();
                List<ServerMachineStatus> _status = new List<ServerMachineStatus>();
                foreach (XElement xe in xdoc.Root.Elements())
                {
                    String description = "";
                    String softwareLabel = "";
                    String publicIp = "";
                    String resourcePath = "";
                    if (xe.Element(ns + "resourcePath") != null) { resourcePath = xe.Element(ns + "resourcePath").Value; }
                    if (xe.Element(ns + "description") != null) { description = xe.Element(ns + "description").Value; }
                    if (xe.Element(ns + "softwareLabel") != null) { softwareLabel = xe.Element(ns + "softwareLabel").Value; }
                    if (xe.Element(ns + "publicIp") != null) { publicIp = xe.Element(ns + "publicIp").Value; }
                    foreach (XElement xe_disks in xe.Elements(ns + "disk"))
                    {
                        _disks.Add(new ServerDiskDataModel() { id = xe_disks.Attribute("id").Value, sizeGb = Convert.ToInt16(xe_disks.Attribute("sizeGb").Value) });
                    }
                    foreach (XElement xe_status in xe.Elements(ns + "machineStatus"))
                    {
                        _status.Add(new ServerMachineStatus() { name = xe_status.Attribute("name").Value, value = xe_status.Value });
                    }
                    _servers.Add(new ServerDataModel()
                    {
                        id = xe.Attribute("id").Value,
                        name = xe.Element(ns + "name").Value,
                        description = description,
                        operatingSystem = xe.Element(ns + "operatingSystem").Attribute("displayName").Value,
                        cpuCount = Convert.ToInt16(xe.Element(ns + "cpuCount").Value),
                        memoryMb = Convert.ToInt16(xe.Element(ns + "memoryMb").Value),
                        created = Convert.ToDateTime(xe.Element(ns + "created").Value),
                        disks = _disks,
                        resourcePath = resourcePath,
                        softwareLabel = softwareLabel,
                        sourceImageId = xe.Element(ns + "sourceImageId").Value,
                        networkId = xe.Element(ns + "networkId").Value,
                        machineName = xe.Element(ns + "machineName").Value,
                        privateIp = xe.Element(ns + "privateIp").Value,
                        publicIp = publicIp,
                        isStarted = XmlConvert.ToBoolean(xe.Element(ns + "isStarted").Value),
                        isDeployed = XmlConvert.ToBoolean(xe.Element(ns + "isDeployed").Value),
                        state = xe.Element(ns + "state").Value,
                        machineStatus = _status
                    });
                }
                return true;
            }
            else
            {
                returnMessage = serverList.responseMessage;
                return false;
            }
        }
    }
}
