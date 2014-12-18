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
    class Reports
    {
        public int httpResponseCode;
        public String returnMessage;
        public DateTime _startDate;
        public DateTime _endDate;
        public List<DetailReportModel> _reportDetails;

        public bool detailUsage()
        {
            httpPost detailusage = new httpPost();
            detailusage.url = "/oec/0.9/" + Properties.Settings.Default.OrgID + "/report/usageDetailed?startDate=" + _startDate + "&endDate=" + _endDate;
            detailusage.httpMethod = "GET";

            //Build XML
            var input_xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
            );

            detailusage.postData = input_xdoc.ToString();
            detailusage.doPost();
            httpResponseCode = detailusage.responseCode;
            if (detailusage.responseCode == 200)
            {
                XDocument xdoc = XDocument.Parse(detailusage.responseData);
                var ns = xdoc.Root.Name.Namespace;
                foreach (XElement xe in xdoc.Root.Elements())
                {
                    String description = "";
                    if (xe.Element(ns + "description") != null)
                    {
                        description = description = xe.Element(ns + "description").Value.Replace(System.Environment.NewLine, "");
                    }

                    //_reportDetails.Add(new DetailReportModel()
                    //{
                    //    id = xe.Element(ns + "id").Value,
                    //    name = xe.Element(ns + "name").Value,
                    //    description = description,
                    //    resourcePath = xe.Element(ns + "resourcePath").Value,
                    //    multicast = xe.Element(ns + "multicast").Value
                    //});
                }
                return true;
            }
            else
            {
                returnMessage = detailusage.responseMessage;
                return false;
            }
        }
    }
}
