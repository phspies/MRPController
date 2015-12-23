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
        LocalData _localdata = new LocalData();
        #region workloads
        public List<Workload> ListWorkloads()
        {
            return _localdata.get_as_list<Workload>();
        }
        public Workload AddWorkload(Workload _addworkload)
        {
            Workload _addedworkload = new Workload();
            try
            {
                _addedworkload = (Workload)_localdata.insert_record<Workload>(_addworkload);
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
                _update = _localdata.get_as_list<Workload>().FirstOrDefault(d => d.id == _updateworkload.id);
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateworkload, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updateworkload, _update);
                    _update.hash_value = _update.GetHashString();
                    _localdata.update_record<Workload>(_update);
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
            _localdata.delete_record<Workload>(_destroyworkload.id);
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
            return _localdata.get_as_list<Platform>();
        }
        public Platform AddPlatform(Platform _addplatform)
        {
            Platform _addedplatform = null;
            try
            {
                return _localdata.insert_record<Platform>(_addplatform);
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
                _update = _localdata.get_as_list<Platform>().FirstOrDefault(d => d.id == _updateplatform.id);
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateplatform, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updateplatform, _update);
                    _update.hash_value = _update.GetHashString();
                    _localdata.update_record<Platform>(_update);
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
            _localdata.delete_record<Platform>(_destroyplatform.id);
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
            List<Credential> _credentials = _localdata.get_as_list<Credential>();
            return _credentials;
        }
        public Credential AddCredential(Credential _addCredential)
        {
            Credential _addedCredential = null;
            try
            {
                _addedCredential = _localdata.insert_record<Credential>(_addCredential);
                _addedCredential.human_type = (_addedCredential.credential_type == 0 ? "Platform" : "Workload");
                _addedCredential.hash_value = _addedCredential.GetHashString();
                _localdata.update_record<Credential>(_addedCredential);
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
                _update = _localdata.get_record<Credential>(_updateCredential.id);
                _update.human_type = (_updateCredential.credential_type == 0 ? "Platform" : "Workload");
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateCredential, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updateCredential, _update);
                    _update.hash_value = _update.GetHashString();
                    _localdata.update_record<Credential>(_update);
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
            _localdata.delete_record<Credential>(_destroyCredential.id);
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
            return _localdata.get_as_list<Event>();
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
