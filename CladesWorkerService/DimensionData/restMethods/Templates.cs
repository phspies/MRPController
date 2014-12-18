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
    class Templates
    {

        public int httpResponseCode;
        public String returnMessage;
        public List<TemplateDataModel> _templates = new List<TemplateDataModel>();

        //        <Server xmlns='http://oec.api.opsource.net/schemas/server'>
        //<name>My Server</name>
        //<description>My Server Description</description>
        //<vlanResourcePath>/oec/{org-id}/network/{net-id}</vlanResourcePath>
        //<imageResourcePath>/oec/base/image/{image-id}</imageResourcePath>
        //<administratorPassword>zyxw4321</administratorPassword>
        //<isStarted>true</isStarted>
        //</Server>


        public Boolean templateDeploy(String name, String description, String vlanResourcePath, String image_id, String administratorPassword, Boolean isStarted )
        {
            httpPost templateDeploy = new httpPost();
            templateDeploy.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/server";

            templateDeploy.httpMethod = "POST";
            //Build XML

            XNamespace xmlns = "http://oec.api.opsource.net/schemas/server";

            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(xmlns + "Server",
                    new XElement(xmlns + "name", name),
                    new XElement(xmlns + "description", description),
                    new XElement(xmlns + "vlanResourcePath", "/oec/" + Properties.Settings.Default.OrgID + "/network/" + vlanResourcePath),
                    new XElement(xmlns + "imageResourcePath", "/oec/base/image/" + image_id),
                    new XElement(xmlns + "administratorPassword", administratorPassword),
                    new XElement(xmlns + "isStarted", isStarted)
            ));
            input_xdoc.Root.Name = xmlns + input_xdoc.Root.Name.LocalName;
            Debug.Print(input_xdoc.ToString());
            templateDeploy.postData = input_xdoc.ToString();
            templateDeploy.doPost();
            httpResponseCode = templateDeploy.responseCode;
            if (templateDeploy.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(templateDeploy.responseData);
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
                returnMessage = templateDeploy.responseMessage;
                return false;
            }
        }
        public Boolean templateList()
        {

            httpPost templateList = new httpPost();
            templateList.url = "/oec/0.9/base/image";

            templateList.httpMethod = "GET";
            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            templateList.postData = input_xdoc.ToString();
            templateList.doPost();
            httpResponseCode = templateList.responseCode;
            if (templateList.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(templateList.responseData);
                var ns = xdoc.Root.Name.Namespace;
                List<ServerDiskDataModel> _disks = new List<ServerDiskDataModel>();
                List<ServerMachineStatus> _status = new List<ServerMachineStatus>();
                foreach (XElement xe in xdoc.Root.Elements())
                {
                    _templates.Add(new TemplateDataModel()
                    {
                        id = xe.Element(ns + "id").Value,
                        name = xe.Element(ns + "name").Value,
                        description = xe.Element(ns + "description").Value,
                        operatingSystem = xe.Element(ns + "operatingSystem").Element(ns + "displayName").Value,
                        cpuCount = Convert.ToInt16(xe.Element(ns + "cpuCount").Value),
                        memory = Convert.ToInt16(xe.Element(ns + "memory").Value),
                        created = Convert.ToDateTime(xe.Element(ns + "created").Value),
                        resourcePath = xe.Element(ns + "resourcePath").Value,
                    });
                }
                return true;
            }
            else
            {
                returnMessage = templateList.responseMessage;
                return false;
            }
        }
    }
}
