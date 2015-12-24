using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes;
using CloudMoveyWorkerService.Database;
using CloudMoveyWorkerService.Portal.Classes;

namespace CloudMoveyWorkerService.WCF
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class CloudMoveyService : ICloudMoveyService
    {
        LocalDB db = new LocalDB();
        #region workloads
        public List<Workload> ListWorkloads()
        {
            return db.Workloads.ToList();
        }
        public Workload AddWorkload(Workload _addworkload)
        {
            Workload _addedworkload = new Workload();
            try
            {
                _addedworkload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                _addedworkload = (Workload)db.Workloads.Add(_addworkload);
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0}", e.ToString()), EventLogEntryType.Error);
            }
            return _addedworkload;

        }
        public Workload UpdateWorkload(Workload _updateworkload)
        {
            Workload _update = null;
            try
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
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", e.Message), EventLogEntryType.Error);
            }
            return _update;
        }
        public bool DestroyWorkload(Workload _destroyworkload)
        {
            db.Workloads.Remove(_destroyworkload);
            db.SaveChanges();
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
            return db.Platforms.ToList();
        }
        public Platform AddPlatform(Platform _addplatform)
        {
            Platform _addedplatform = null;
            try
            {
                _addedplatform.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                return db.Platforms.Add(_addplatform);
            }
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0} | {1}", e.Message, e.InnerException.ToString()), EventLogEntryType.Error);
            }
            return _addedplatform;

        }
        public Platform UpdatePlatform(Platform _updateplatform)
        {
            Platform _update = null;
            try
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
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", e.Message), EventLogEntryType.Error);
            }
            return _update;
        }
        public bool DestroyPlatform(Platform _destroyplatform)
        {
            db.Platforms.Remove(_destroyplatform);
            db.SaveChanges();
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
                Global.event_log.WriteEntry(ex.ToString());
            }
        }
        #endregion
        #region Credentials
        public List<Credential> ListCredentials()
        {
            return db.Credentials.ToList();
        }
        public Credential AddCredential(Credential _addCredential)
        {
            Credential _addedCredential = null;
            try
            {
                _addCredential.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                _addCredential.human_type = (_addedCredential.credential_type == 0 ? "Platform" : "Workload");
                _addCredential.hash_value = _addedCredential.GetHashString();
                _addCredential = db.Credentials.Add(_addCredential);
                _addedCredential = db.Credentials.Add(_addCredential);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0} | {1}", e.Message, e.InnerException.ToString()), EventLogEntryType.Error);
            }
            return _addedCredential;

        }
        public Credential UpdateCredential(Credential _updateCredential)
        {
            Credential _update = null;
            try
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
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0} | {1}", e.Message, e.InnerException.ToString()), EventLogEntryType.Error);
            }
            return _update;
        }
        public bool DestroyCredential(Credential _destroyCredential)
        {
            db.Credentials.Remove(db.Credentials.Find(_destroyCredential.id));
            db.SaveChanges();
            return true;
        }

        #endregion
        #region Caas Methods
        public Object ListDatacenters(string url, Credential _credential)
        {
            DimensionData _caas = new DimensionData(url, _credential.username, _credential.password);
            return _caas.datacenter().datacenters();
        }
        public PlatformDetails PlatformDetails(String _datacenterId, String _url, Credential _credential)
        {
            return new PlatformDetails( _datacenterId,  _url, _credential);
        }
        public Object Account(string url, Credential _credential)
        {
            DimensionData _caas = new DimensionData(url, _credential.username, _credential.password);
            return _caas.account().myaccount();
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
            return db.Events.ToList();
        }
    }
    public class workerInformation
    {
        public string agentId { set; get; }
        public string versionNumber { set; get; }
        public int currentJobs { set; get; }
    }


}
namespace CloudMoveyWorkerService.WCF
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
