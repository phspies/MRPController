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
    class myAccount
    {
        public int httpResponseCode;
        public String returnMessage;
        public String fullName;
        public String firstName;
        public String lastName;
        public String emailAddress;
        public String orgId;

        public bool accountDetails()
        {
            httpPost accDetails = new httpPost();
            accDetails.url = "/oec/0.9/myaccount";
            accDetails.httpMethod = "GET";

            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            accDetails.postData = input_xdoc.ToString();
            accDetails.doPost();
            httpResponseCode = accDetails.responseCode;
            if (accDetails.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(accDetails.responseData);
                var ns = xdoc.Root.Name.Namespace;

                fullName = xdoc.Root.Element(ns +"fullName").Value;
                firstName = xdoc.Root.Element(ns + "firstName").Value;
                lastName = xdoc.Root.Element(ns + "lastName").Value;
                emailAddress = xdoc.Root.Element(ns + "emailAddress").Value;
                orgId = xdoc.Root.Element(ns + "orgId").Value;
                return true;
            }
            else
            {
                returnMessage = accDetails.responseMessage;
                return false;
            }
        }
    }
}
