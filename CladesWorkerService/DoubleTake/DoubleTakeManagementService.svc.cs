using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using CladesWorkerService;

namespace CladesWorkerService
{
    public class DoubleTakeManagementService
    {
        public System.Diagnostics.EventLog DoubleTakeProxyLog = new System.Diagnostics.EventLog("Application", ".", "Double-Take JSON Service");

        public String GetProductInfo(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }
            ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(BuildUri(request, "/DoubleTake/Common/Contract/ManagementService").targetUri.Uri));
            MgtServiceFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(iMgrSrc.GetProductInfo());
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }
            return json;
        }
        public String GetImages(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(BuildUri(request, "/DoubleTake/Common/Contract/ManagementService").targetUri.Uri));
            MgtServiceFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(iMgrSrc.GetImages(null));
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }

            return json;
        }
        private UriModel BuildUri(JobManagerRequest request, String method)
        {
            string portNumber = "6325";
            string bindingScheme = "http://";
            UriModel UriObject = new UriModel();

            if (request.credentials.sourceIPAddress != null)
            {
                UriObject.sourceShortUri = new UriBuilder(bindingScheme + request.credentials.sourceIPAddress + ":" + portNumber);
                UriObject.sourceShortUri.UserName = Uri.EscapeDataString(request.credentials.sourceUserAccount);
                UriObject.sourceShortUri.Password = Uri.EscapeDataString(request.credentials.sourceUserPassword);
                UriObject.sourceUri = new UriBuilder(bindingScheme + request.credentials.sourceIPAddress + ":" + portNumber + method);

            }
            if (request.credentials.targetIPAddress != null)
            {
                UriObject.targetShortUri = new UriBuilder(bindingScheme + request.credentials.targetIPAddress + ":" + portNumber);
                UriObject.targetShortUri.UserName = Uri.EscapeDataString(request.credentials.targetUserAccount);
                UriObject.targetShortUri.Password = Uri.EscapeDataString(request.credentials.targetUserPassword);
                UriObject.targetUri = new UriBuilder(bindingScheme + request.credentials.targetIPAddress + ":" + portNumber + method);
                Debug.WriteLine("ShortURI:" + UriObject.targetShortUri);
                Debug.WriteLine("LongURI:" + UriObject.targetUri);
            }
            return UriObject;
        }

    }
}
