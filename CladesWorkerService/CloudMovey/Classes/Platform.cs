using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using CloudMoveyWorkerService.CloudMovey.Types;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class Platform
    {
        public static void mcp_getdatacenters(dynamic payload)
        {
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.username, (string)payload.payload.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.datacenter().datacenters()));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_gettemplates(dynamic payload)
        {
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.username, (string)payload.payload.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                List<Option> filter = new List<Option>();
                filter.Add(new Option() { option = "location", value = payload.payload.locationId });
                filter.Add(new Option() { option = "operatingSystemFamily", value = "WINDOWS" });
                filter.Add(new Option() { option = "state", value = "NORMAL" });
                CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.datacenter().templates(filter)));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_retrieveservers(dynamic payload)
        {
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = CaaS.datacenter().datacenters(options).datacenter[0];
                switch (dc.type)
                {
                    case "MCP 1.0":
                        List<Option> filter = new List<Option>();
                        filter.Add(new Option() { option = "location", value = payload.payload.platform.moid });
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.server().platformservers(filter)));
                        break;
                    case "MCP 2.0":
                        List<Option> _options = new List<Option>() { new Option() { option = "datacenterId", value = moid } };
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.mcp2servers().listservers(_options)));
                        break;
                }
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
        public static void mcp_retrievenetworks(dynamic payload)
        {
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            TasksObject tasks = new TasksObject(CloudMovey);
            DimensionData CaaS = new DimensionData((string)payload.payload.mcp.url, (string)payload.payload.platform.username, (string)payload.payload.platform.password, null);
            try
            {
                CloudMovey.task().progress(payload, "MCP Data Gathering", 50);
                string moid = payload.payload.platform.moid;
                List<Option> options = new List<Option>();
                options.Add(new Option() { option = "id", value = moid });
                DatacenterType dc = CaaS.datacenter().datacenters(options).datacenter[0];
                switch(dc.type)
                {
                    case "MCP 1.0":
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.network().networklist(moid)));
                        break;
                    case "MCP 2.0":
                        List<Option> _options = new List<Option>() { new Option() {option = "datacenterId", value = moid}};
                        CloudMovey.task().successcomplete(payload, JsonConvert.SerializeObject(CaaS.mcp2vlans().listvlan(_options)));
                        break;
                }
                
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(payload, e.ToString());
            }
        }
    }
}
