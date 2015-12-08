using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.Portal.Models;
using CloudMoveyWorkerService.CloudMoveyWorkerService.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CloudMoveyWorkerService.WCF
{
    [ServiceContract]
    public interface ICloudMoveyService
    {
        [OperationContract]
        workerInformation CollectionInformation();
        #region Failovergroups
        [OperationContract]
        List<Failovergroup> ListFailovergroups();
        [OperationContract]
        Failovergroup AddFailovergroup(Failovergroup _addfailovergroup);
        [OperationContract]
        Failovergroup UpdateFailovergroup(Failovergroup _updatefailovergroup);
        [OperationContract]
        bool DestroyFailovergroup(Failovergroup _destroyfailovergroup);
        #endregion

        #region Workloads
        [OperationContract]
        List<Workload> ListWorkloads();
        [OperationContract]
        Workload AddWorkload(Workload _addworkload);
        [OperationContract]
        Workload UpdateWorkload(Workload _updateworkload);
        [OperationContract]
        bool DestroyWorkload(Workload _destroyworkload);
        #endregion

        #region Platforms
        [OperationContract]
        List<Platform> ListPlatforms();
        [OperationContract]
        Platform AddPlatform(Platform _addplatform);
        [OperationContract]
        Platform UpdatePlatform(Platform _updateplatform);
        [OperationContract]
        bool DestroyPlatform(Platform _destroyplatform);
        [OperationContract]
        void RefreshPlatform(Platform _platform);
        #endregion

        #region Credentials
        [OperationContract]
        List<Credential> ListCredentials();
        [OperationContract]
        Credential AddCredential(Credential _addCredential);
        [OperationContract]
        Credential UpdateCredential(Credential _updateCredential);
        [OperationContract]
        bool DestroyCredential(Credential _destroyCredential);
        #endregion


        [OperationContract]
        [ServiceKnownType(typeof(Status))]
        [ServiceKnownType(typeof(DatacenterListType))]
        Object ListDatacenters(string url, Credential _credential);
        [OperationContract]
        [ServiceKnownType(typeof(ResponseType))]
        [ServiceKnownType(typeof(Account))]
        Object Account(string url, Credential _credential);
        [OperationContract]
        PlatformDetails PlatformDetails(String _datacenterId, String _url, Credential _credential);
        List<Event> Events();
    }
}
