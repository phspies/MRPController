using CladesWorkerService.CaaS;
using DoubleTake.Common.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.Classes
{
    class DoubleTake
    {
        public static void dt_install(dynamic payload)
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            clades.task().progress(payload, "DT Connection", 50);
            ChannelFactory<ProxyPushInstallerClient> CoreFactory =
                new ChannelFactory<ProxyPushInstallerClient>("DefaultBinding_CoreService_CoreService",
                    new EndpointAddress(BuildUri(payload, "/DoubleTake/Common/Contract/ManagementService").targetUri.Uri));
            TasksObject tasks = new TasksObject(clades);
            ProxyPushInstallerClient iCoreSrc = CoreFactory.CreateChannel();
    
            try
            {
                string[] licenses = {"aa","bb"};
                clades.task().progress(payload, "DT Data Gathering", 50);
                ServiceConnectionParameters connection = new ServiceConnectionParameters();
                connection.Address = "";
                connection.UserName = "";
                connection.Password = "";
                connection.Domain = "";
               

                PushInstallationOptions pushOptions = new PushInstallationOptions();

                PushInstallToken token = iCoreSrc.Install(connection, pushOptions);
                PushInstallStatus status = new PushInstallStatus();
                
                while (
                    status.status != InstallStatus.Canceled &&
                    status.status != InstallStatus.Success &&
                    status.status != InstallStatus.Error)
                {
                    Thread.Sleep(1000);
                    status = iCoreSrc.GetInstallStatus(connection, token);
                }

                //clades.task().successcomplete(payload, JsonConvert.SerializeObject( ));
                
            }
            catch (Exception e)
            {
                clades.task().failcomplete(payload, e.ToString());
            }
        }
        public static void dt_getproductinformation(dynamic payload)
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            clades.task().progress(payload, "DT Connection", 50);
            ChannelFactory<IManagementService> MgtServiceFactory = 
                new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", 
                    new EndpointAddress(BuildUri(payload, "/DoubleTake/Common/Contract/ManagementService").targetUri.Uri));
            TasksObject tasks = new TasksObject(clades);
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
            try
            {
                clades.task().progress(payload, "DT Data Gathering", 50);
                clades.task().successcomplete(payload, JsonConvert.SerializeObject(iMgrSrc.GetProductInfo()));
            }
            catch (Exception e)
            {
                clades.task().failcomplete(payload, e.ToString());
            }
        }
        public static void dt_getimages(dynamic payload)
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            clades.task().progress(payload, "DT Connection", 50);
            ChannelFactory<IManagementService> MgtServiceFactory =
                new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService",
                    new EndpointAddress(BuildUri(payload, "/DoubleTake/Common/Contract/ManagementService").targetUri.Uri));
            TasksObject tasks = new TasksObject(clades);
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
            try
            {
                clades.task().progress(payload, "DT Data Gathering", 50);
                clades.task().successcomplete(payload, JsonConvert.SerializeObject(iMgrSrc.GetImages(null)));
            }
            catch (Exception e)
            {
                clades.task().failcomplete(payload, e.ToString());
            }
        }

        private UriModel BuildUri(dynamic request, String method)
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
            }
            return UriObject;
        }
    }
}
