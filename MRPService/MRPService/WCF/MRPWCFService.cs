using MRPService.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using MRPService.API.Classes;
using MRPService.LocalDatabase;
using MRPService.MRPService.Log;
using System.Net;
using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Requests;
using MRPService.Utilities;
using MRPService.VMWare;

namespace MRPService.WCF
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MRPWCFService : IMRPWCFService
    {
        #region workloads
        public List<Workload> ListWorkloads()
        {
            using (WorkloadSet db = new WorkloadSet())
            {
                return db.ModelRepository.Get();
            }
        }
        public Workload AddWorkload(Workload _addworkload)
        {
            try
            {
                using (WorkloadSet db = new WorkloadSet())
                {
                    _addworkload.id = Objects.RamdomGuid();
                    db.ModelRepository.Insert(_addworkload);                  
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Logger.log(String.Format("Error adding record: {0}", e.ToString()), Logger.Severity.Error);
            }
            return _addworkload;
        }
        public Workload UpdateWorkload(Workload _updateworkload)
        {
            Workload _update = null;
            try
            {
                using (WorkloadSet db = new WorkloadSet())
                {
                    _update = db.ModelRepository.GetById(_updateworkload.id);
                    Objects.Copy_Exclude_ID(_updateworkload, _update);
                    db.ModelRepository.Update(_update);
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
            return _update;
        }
        public void DestroyWorkload(Workload _destroyworkload)
        {
            try
            {
                using (WorkloadSet db = new WorkloadSet())
                {
                    db.ModelRepository.Delete(_destroyworkload);
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
        }
        #endregion
        #region Platforms
        public List<Platform> ListPlatforms()
        {
            using (PlatformSet db = new PlatformSet())
            {
                return db.ModelRepository.Get();
            }
        }
        public Platform AddPlatform(Platform _addplatform)
        {
            try
            {
                using (PlatformSet db = new PlatformSet())
                {
                    _addplatform.id = Objects.RamdomGuid();
                    db.ModelRepository.Insert(_addplatform);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Logger.log(String.Format("Error adding record: {0}", e.ToString()), Logger.Severity.Error);
            }
            return _addplatform;
        }
        public Platform UpdatePlatform(Platform _updateplatform)
        {
            Platform _update = null;
            try
            {
                using (PlatformSet db = new PlatformSet())
                {
                    _update = db.ModelRepository.GetById(_updateplatform.id);
                    Objects.Copy_Exclude_ID(_updateplatform, _update);
                    db.ModelRepository.Update(_update);
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
            return _update;
        }
        public void DestroyPlatform(Platform _destroyplatform)
        {
            try
            {
                using (PlatformSet db = new PlatformSet())
                {
                    db.ModelRepository.Delete(_destroyplatform);
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
        }
        public void RefreshPlatform(Platform _platform)
        {
            try {
                PlatformInventoryWorker _inventory = new PlatformInventoryWorker();
                _inventory.UpdateMCPPlatform(_platform);

            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Error);
            }
        }
        #endregion
        #region Credentials
        public List<Credential> ListCredentials()
        {
            using (CredentialSet db = new CredentialSet())
            {
                return db.ModelRepository.Get();
            }
        }
        public Credential AddCredential(Credential _addcredential)
        {
            try
            {
                using (CredentialSet db = new CredentialSet())
                {
                    _addcredential.id = Objects.RamdomGuid();
                    db.ModelRepository.Insert(_addcredential);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Logger.log(String.Format("Error adding record: {0}", e.ToString()), Logger.Severity.Error);
            }
            return _addcredential;
        }
        public Credential UpdateCredential(Credential _updatecredential)
        {
            Credential _update = null;
            try
            {
                using (CredentialSet db = new CredentialSet())
                {
                    _update = db.ModelRepository.GetById(_updatecredential.id);
                    Objects.Copy_Exclude_ID(_updatecredential, _update);
                    db.ModelRepository.Update(_update);
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
            return _update;
        }
        public void DestroyCredential(Credential _destroycredential)
        {
            try
            {
                using (CredentialSet db = new CredentialSet())
                {
                    db.ModelRepository.Delete(_destroycredential);
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
        }

        #endregion
        #region Caas Methods
        public List<Platform> ListDatacenters(string url, Credential _credential, int _platform_type)
        {
            List<Platform> _platforms = new List<Platform>();
            switch (_platform_type)
            {
                case 0:
                    ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(url), new NetworkCredential(_credential.username, _credential.password));
                    CaaS.Login().Wait();
                    CaaS.Infrastructure.GetDataCenters(new PageableRequest() { PageSize = 250 }).Result.ToList().ForEach(x => { _platforms.Add(new Platform() { datacenter = x.displayName, moid = x.id }); });
                    break;
                case 1:
                    ApiClient VMWare = new ApiClient(url, _credential.username, _credential.password);
                    VMWare.datacenter().DatacenterList().ForEach(x => { _platforms.Add(new Platform() { datacenter = x.Name, moid = x.MoRef.Value }); });
                    break;

            }
            return _platforms;
        }
        public PlatformDetails PlatformDetails(String _datacenterId, String _url, Credential _credential)
        {
            return new PlatformDetails( _datacenterId,  _url, _credential);
        }
        public Tuple<bool, String> Login(string _url, Credential _credential, int _platform_type)
        {
            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_url), new NetworkCredential(_credential.username, _credential.password));
            try
            {
                CaaS.Login().Wait();
                return Tuple.Create(true, "Success");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }
        #endregion

        public List<Event> Events()
        {
            using (MRPDatabase db = new MRPDatabase())
            {
                return db.Events.ToList();
            }
        }
        public workerInformation CollectionInformation()
        {
            workerInformation _notifier = new workerInformation();
            _notifier.agentId = Global.agent_id;
            _notifier.versionNumber = Global.version_number;
            _notifier.currentJobs = Global.worker_queue_count;
            return _notifier;
        }

    }
    public class workerInformation
    {
        public string agentId { set; get; }
        public string versionNumber { set; get; }
        public int currentJobs { set; get; }
    }


}
