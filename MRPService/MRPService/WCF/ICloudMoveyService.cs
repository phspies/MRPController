using MRPService.API.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using MRPService.LocalDatabase;
using DD.CBU.Compute.Api.Contracts.General;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Directory;

namespace MRPService.WCF
{
    [ServiceContract]
    public interface ICloudMRPService
    {

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
        workerInformation CollectionInformation();
        [OperationContract]
        [ServiceKnownType(typeof(Status))]
        [ServiceKnownType(typeof(List<DatacenterType>))]
        List<DatacenterType> ListDatacenters(string url, Credential _credential);
        [OperationContract]
        [ServiceKnownType(typeof(ResponseType))]
        [ServiceKnownType(typeof(Account))]
        AccountWithPhoneNumber Account(string url, Credential _credential);
        [OperationContract]
        PlatformDetails PlatformDetails(String _datacenterId, String _url, Credential _credential);
        List<Event> Events();
    }
}
