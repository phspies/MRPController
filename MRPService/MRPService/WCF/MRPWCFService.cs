using MRPService.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using MRPService.API.Classes;
using MRPService.LocalDatabase;
using MRPService.MRPService.Log;
using System.Net;
using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Directory;
using DD.CBU.Compute.Api.Contracts.Network20;
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
            using (MRPDatabase db = new MRPDatabase())
            {
                return db.Workloads.ToList();
            }
        }
        public Workload AddWorkload(Workload _addworkload)
        {
            Workload _addedworkload = new Workload();
            try
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    _addedworkload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    _addedworkload = (Workload)db.Workloads.Add(_addworkload);
                    db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Logger.log(String.Format("Error adding record: {0}", e.ToString()), Logger.Severity.Error);
            }
            return _addedworkload;

        }
        public Workload UpdateWorkload(Workload _updateworkload)
        {
            Workload _update = null;
            try
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    _update = db.Workloads.FirstOrDefault(d => d.id == _updateworkload.id);
                    IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateworkload, _update);
                    if (_changes.Count() > 0)
                    {
                        Copy(_updateworkload, _update);
                        _update.hash_value = _update.GetHashString();
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
            return _update;
        }
        public bool DestroyWorkload(Workload _destroyworkload)
        {
            using (MRPDatabase db = new MRPDatabase())
            {
                db.Workloads.Remove(_destroyworkload);
                db.SaveChanges();
            }
            return true;
        }
        #endregion
        public workerInformation CollectionInformation()
        {
            workerInformation _notifier = new workerInformation();
            _notifier.agentId = Global.agent_id;
            _notifier.versionNumber = Global.version_number;
            _notifier.currentJobs = Global.worker_queue_count;
            return _notifier;
        }
        #region Platforms
        public List<Platform> ListPlatforms()
        {
            using (MRPDatabase db = new MRPDatabase())
            {
                return db.Platforms.ToList();
            }
        }
        public Platform AddPlatform(Platform _addplatform)
        {
            Platform _addedplatform = null;
            try
            {
                _addplatform.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                using (MRPDatabase db = new MRPDatabase())
                {
                    _addedplatform = db.Platforms.Add(_addplatform);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error adding record: {0} | {1}", e.Message, e.InnerException.ToString()), Logger.Severity.Error);
            }
            return _addedplatform;

        }
        public Platform UpdatePlatform(Platform _updateplatform)
        {
            Platform _update = null;
            try
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    _update = db.Platforms.FirstOrDefault(d => d.id == _updateplatform.id);
                    IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateplatform, _update);
                    if (_changes.Count() > 0)
                    {
                        Copy(_updateplatform, _update);
                        _update.hash_value = _update.GetHashString();
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0}", e.Message), Logger.Severity.Error);
            }
            return _update;
        }
        public bool DestroyPlatform(Platform _destroyplatform)
        {
            using (MRPDatabase db = new MRPDatabase())
            {
                db.Platforms.Remove(_destroyplatform);
                db.SaveChanges();
            }
            return true;
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
            using (MRPDatabase db = new MRPDatabase())
            {
                return db.Credentials.ToList();
            }
        }
        public Credential AddCredential(Credential _addCredential)
        {
            Credential _addedCredential = null;
            try
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    _addCredential.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    _addCredential.human_type = (_addCredential.credential_type == 0 ? "Platform" : "Workload");
                    _addedCredential = db.Credentials.Add(_addCredential);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error adding record: {0} | {1}", e.Message, e.InnerException.ToString()), Logger.Severity.Error);
            }
            return _addedCredential;

        }
        public Credential UpdateCredential(Credential _updateCredential)
        {
            Credential _update = null;
            try
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    _update = db.Credentials.Find(_updateCredential.id);
                    _update.human_type = (_updateCredential.credential_type == 0 ? "Platform" : "Workload");
                    IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateCredential, _update);
                    if (_changes.Count() > 0)
                    {
                        Copy(_updateCredential, _update);
                        _update.hash_value = _update.GetHashString();
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.log(String.Format("Error updating record: {0} | {1}", e.Message, e.InnerException.ToString()), Logger.Severity.Error);
            }
            return _update;
        }
        public bool DestroyCredential(Credential _destroyCredential)
        {
            using (MRPDatabase db = new MRPDatabase())
            {
                db.Credentials.Remove(db.Credentials.Find(_destroyCredential.id));
                db.SaveChanges();
            }
            return true;
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
        public AccountWithPhoneNumber Account(string url, Credential _credential)
        {
            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(url), new NetworkCredential(_credential.username, _credential.password));
            CaaS.Login().Wait();
            return CaaS.Account.GetAdministratorAccount(_credential.username).Result;
        }
        #endregion
        static void Copy(object a, object b)
        {
            foreach (PropertyInfo propA in a.GetType().GetProperties())
            {
                if (propA.Name != "id")
                {
                    PropertyInfo propB = b.GetType().GetProperty(propA.Name);
                    propB.SetValue(b, propA.GetValue(a, null), null);
                }
            }
        }
        public List<Event> Events()
        {
            using (MRPDatabase db = new MRPDatabase())
            {
                return db.Events.ToList();
            }
        }
    }
    public class workerInformation
    {
        public string agentId { set; get; }
        public string versionNumber { set; get; }
        public int currentJobs { set; get; }
    }


}
namespace MRPService.WCF
{
    public static class Extensions
    {
        public static IEnumerable<string> EnumeratePropertyDifferences<T>(this T obj1, T obj2)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> changes = new List<string>();

            foreach (PropertyInfo pi in properties.Where(x => x.Name != "hash_value"))
            {
                object value1 = typeof(T).GetProperty(pi.Name).GetValue(obj1, null);
                object value2 = typeof(T).GetProperty(pi.Name).GetValue(obj2, null);

                if (value1 != value2 && (value1 == null || !value1.Equals(value2)))
                {
                    changes.Add(string.Format("Property {0} changed from {1} to {2}", pi.Name, value1, value2));
                }
            }
            return changes;
        }
    }
}
