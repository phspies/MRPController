using MRMPService.API.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using MRMPService.LocalDatabase;
using DD.CBU.Compute.Api.Contracts.General;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Directory;

namespace MRMPService.WCF
{
    [ServiceContract]
    public interface IMRPWCFService
    {

        #region Workloads
        [OperationContract]
        List<Workload> ListWorkloads();
        [OperationContract]
        Workload AddWorkload(Workload _addworkload);
        [OperationContract]
        Workload UpdateWorkload(Workload _updateworkload);
        [OperationContract]
        void DestroyWorkload(Workload _destroyworkload);
        #endregion

        #region Platforms
        [OperationContract]
        List<Platform> ListPlatforms();
        [OperationContract]
        Platform AddPlatform(Platform _addplatform);
        [OperationContract]
        Tuple<bool, String> Login(string _url, Credential _credential, int _platform_type);
        [OperationContract]
        Platform UpdatePlatform(Platform _updateplatform);
        [OperationContract]
        void DestroyPlatform(Platform _destroyplatform);
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
        void DestroyCredential(Credential _destroyCredential);
        #endregion

        [OperationContract]
        workerInformation CollectionInformation();
        [OperationContract]
        [ServiceKnownType(typeof(Status))]
        [ServiceKnownType(typeof(List<DatacenterType>))]
        List<Platform> ListDatacenters(string url, Credential _credential, int _platform_type);
        [OperationContract]
        PlatformDetails PlatformDetails(String _datacenterId, String _url, Credential _credential);
        List<Event> Events();
    }
}
