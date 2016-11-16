using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRPService.Classes.Utilities
{
    class UpdateDNS
    {
        private void UpdateARecord(string strDNSZone, string strHostName, string strIPAddress)
        {
            ManagementScope mgmtScope = new ManagementScope(@"\\.\Root\MicrosoftDNS");
            ManagementClass mgmtClass = null;
            ManagementBaseObject mgmtParams = null;
            ManagementObjectSearcher mgmtSearch = null;
            ManagementObjectCollection mgmtDNSRecords = null;
            string strQuery;

            strQuery = string.Format("SELECT * FROM MicrosoftDNS_AType WHERE OwnerName = '{0}.{1}'", strHostName, strDNSZone);

            mgmtScope.Connect();

            mgmtSearch = new ManagementObjectSearcher(mgmtScope, new ObjectQuery(strQuery));

            mgmtDNSRecords = mgmtSearch.Get();

            // Multiple A records with the same record name, but different IPv4 addresses, skip.
            if (mgmtDNSRecords.Count > 1)
            {
                // Take appropriate action here.
            }
            // Existing A record found, update record.
            else if (mgmtDNSRecords.Count == 1)
            {
                foreach (ManagementObject mgmtDNSRecord in mgmtDNSRecords)
                {
                    if (mgmtDNSRecord["RecordData"].ToString() != strIPAddress)
                    {
                        mgmtParams = mgmtDNSRecord.GetMethodParameters("Modify");
                        mgmtParams["IPAddress"] = strIPAddress;

                        mgmtDNSRecord.InvokeMethod("Modify", mgmtParams, null);
                    }

                    break;
                }
            }
            // A record does not exist, create new record.
            else
            {
                mgmtClass = new ManagementClass(mgmtScope, new ManagementPath("MicrosoftDNS_AType"), null);

                mgmtParams = mgmtClass.GetMethodParameters("CreateInstanceFromPropertyData");
                mgmtParams["DnsServerName"] = Environment.MachineName;
                mgmtParams["ContainerName"] = strDNSZone;
                mgmtParams["OwnerName"] = string.Format("{0}.{1}", strHostName.ToLower(), strDNSZone);
                mgmtParams["IPAddress"] = strIPAddress;

                mgmtClass.InvokeMethod("CreateInstanceFromPropertyData", mgmtParams, null);
            }
        }
    }
}
