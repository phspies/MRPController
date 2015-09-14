using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using CloudMoveyWorkerService.CloudMovey;
using CloudMoveyWorkerService.CloudMovey.Models;
using CloudMoveyWorkerService.CloudMovey.Sqlite.Models;
using CloudMoveyWorkerService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using CloudMoveyWorkerService.CloudMovey.Classes.Static_Classes;

namespace CloudMoveyWorkerService.WCF
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class CloudMoveyService : ICloudMoveyService
    {
        #region failovergroups
        public List<Failovergroup> ListFailovergroups()
        {
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            var failovergroups = from Failovergroup in dbcontext.Failovergroups select Failovergroup;
            return failovergroups.ToList<Failovergroup>();
        }
        public Failovergroup AddFailovergroup(Failovergroup _addfailovergroup)
        {
            Failovergroup _addedfailovergroup = null;
            try
            {
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _addfailovergroup.id = Guid.NewGuid().ToString().Replace("-", "").GetSHA1Hash();
                _addedfailovergroup = dbcontext.Failovergroups.Add(_addfailovergroup);
                CloudMoveyEvents.add(new Event() { entity = _addfailovergroup.group, severity = 0, component ="New Failover Group", summary = _addfailovergroup.ToString() });
                dbcontext.SaveChanges();

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0}", e.ToString()), EventLogEntryType.Error);
            }
            return _addedfailovergroup;

        }
        public Failovergroup UpdateFailovergroup(Failovergroup _updatefailovergroup)
        {
            Failovergroup _update = null;
            try
            {
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _update = dbcontext.Failovergroups.FirstOrDefault(d => d.id == _updatefailovergroup.id);
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updatefailovergroup, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updatefailovergroup, _update);
                    CloudMoveyEvents.add(new Event() { entity = _update.group, severity = 0, summary = _changes.ToString() });
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", e.Message), EventLogEntryType.Error);
            }
            return _update;
        }
        public bool DestroyFailovergroup(Failovergroup _destroyfailovergroup)
        {
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            Failovergroup _remove = dbcontext.Failovergroups.Single(d => d.id == _destroyfailovergroup.id);
            CloudMoveyEvents.add(new Event() { entity = _remove.group, severity = 0, component = "Destroy Failover Group", summary = _remove.ToString() });
            dbcontext.Failovergroups.Remove(_remove);
            dbcontext.SaveChanges();
            return true;
        }
        #endregion
        #region workloads
        public List<Workload> ListWorkloads()
        {
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            var workloads = from Workload in dbcontext.Workloads select Workload;
            return workloads.ToList<Workload>();
        }
        public Workload UpdateWorkload(Workload _updateworkload)
        {
            Workload _update = null;
            try
            {
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _update = dbcontext.Workloads.FirstOrDefault(d => d.id == _updateworkload.id);
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateworkload, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updateworkload, _update);
                    _update.hash_value = _update.GetSHA1Hash();
                    CloudMoveyEvents.add(new Event() { entity = _update.hostname, severity = 0, component = "Updated Workload", summary = _update.ToString() });
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", e.Message), EventLogEntryType.Error);
            }
            return _update;
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
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            var platforms = from Platform in dbcontext.Platforms select Platform;
            return platforms.ToList<Platform>();
        }
        public Platform AddPlatform(Platform _addplatform)
        {
            Platform _addedplatform = null;
            try
            {
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _addplatform.id = Guid.NewGuid().ToString().Replace("-", "").GetSHA1Hash();
                _addplatform.hash_value = _addplatform.GetSHA1Hash();
                CloudMoveyEvents.add(new Event() { entity = _addplatform.description, severity = 0, component = "New Failover Group", summary = _addplatform.ToString() });
                _addedplatform = dbcontext.Platforms.Add(_addplatform);
                dbcontext.SaveChanges();
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
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _update = dbcontext.Platforms.FirstOrDefault(d => d.id == _updateplatform.id);
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateplatform, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updateplatform, _update);
                    _update.hash_value = _update.GetSHA1Hash();
                    CloudMoveyEvents.add(new Event() { entity = _update.description, severity = 0, component = "Updated Failover Group", summary = _update.ToString() });
                    dbcontext.SaveChanges();
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
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            Platform _remove = dbcontext.Platforms.Single(d => d.id == _destroyplatform.id);
            CloudMoveyEvents.add(new Event() { entity = _remove.description, severity = 0, component = "Destroyed Failover Group", summary = _remove.ToString() });
            dbcontext.Platforms.Remove(_remove);
            dbcontext.SaveChanges();
            return true;
        }
        public void RefreshPlatform(Platform _platform)
        {
            try {
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                Credential _credential = dbcontext.Credentials.FirstOrDefault(x => x.id == _platform.credential_id);
                DimensionData _caas = new DimensionData(_platform.url, _credential.username, _credential.password);
                List<CaaS.Models.Option> _dcoptions = new List<CaaS.Models.Option>();
                List<CaaS.Models.Option> _resourceoptions = new List<CaaS.Models.Option>();
                _resourceoptions.Add(new CaaS.Models.Option() { option = "datacenterId", value = _platform.datacenter });

                _dcoptions.Add(new CaaS.Models.Option() { option = "id", value = _platform.datacenter });
                DatacenterType _dc = ((DatacenterListType)_caas.datacenter().datacenters(_dcoptions)).datacenter[0];
                int workloads, networkdomains, vlans;
                string workloads_md5, networkdomains_md5, vlans_md5;
                workloads = networkdomains = vlans = 0;
                if (_dc.type == "MCP 2.0")
                {
                    List<ServerType> workload_list = _caas.mcp2workloads().listworkloads(_resourceoptions).server;
                    workloads = workload_list.Count();
                    workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(workload_list));
                    List<VlanType> vlan_list = _caas.mcp2vlans().listvlan(_resourceoptions).vlan;
                    vlans = vlan_list.Count();
                    vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(vlan_list));
                    List<NetworkDomainType> networkdomain_list = _caas.mcp2networkdomain().networkdomainlist(_resourceoptions).networkDomain;
                    networkdomains = networkdomain_list.Count();
                    networkdomains_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(networkdomain_list));
                }
                else
                {
                    List<CaaS.Models.Option> _dc1options = new List<CaaS.Models.Option>();
                    _dc1options.Add(new CaaS.Models.Option() { option = "location", value = _platform.datacenter });
                    networkdomains = 1;
                    networkdomains_md5 = "";
                    List<ServersWithBackupServer> workload_list = _caas.workload().platformworkloads(_dc1options).server.ToList();
                    workloads = workload_list.Count();
                    workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(workload_list));
                    List<NetworkWithLocationsNetwork> vlan_list = _caas.network().networklist(_platform.datacenter).network.ToList();
                    vlans = vlan_list.Count();
                    vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(vlan_list));
                }
                Platform __platform = dbcontext.Platforms.FirstOrDefault(x => x.id == _platform.id);
                __platform.vlan_count = vlans;
                __platform.workload_count = workloads;
                __platform.networkdomain_count = networkdomains;
                __platform.platform_version = _dc.type;

                __platform.lastupdated = DateTime.Now;
                __platform.human_vendor = (new Vendors()).VendorList.First(x => x.ID == _platform.vendor).Vendor;
                __platform.moid = _dc.id;
                dbcontext.SaveChanges();
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
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            var Credentials = from Credential in dbcontext.Credentials select Credential;
            return Credentials.ToList<Credential>();
        }
        public Credential AddCredential(Credential _addCredential)
        {
            Credential _addedCredential = null;
            try
            {
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _addCredential.id = Guid.NewGuid().ToString().Replace("-", "").GetSHA1Hash();
                _addedCredential = dbcontext.Credentials.Add(_addCredential);
                _addedCredential.human_type = (_addedCredential.credential_type == 0 ? "Platform" : "Workload");
                _addedCredential.hash_value = _addedCredential.GetSHA1Hash();
                CloudMoveyEvents.add(new Event() { entity = _addedCredential.description, severity = 0, component = "New Credential", summary = _addedCredential.ToString() });
                dbcontext.SaveChanges();
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
                CloudMoveyEntities dbcontext = new CloudMoveyEntities();
                _update = dbcontext.Credentials.FirstOrDefault(d => d.id == _updateCredential.id);
                _update.human_type = (_updateCredential.credential_type == 0 ? "Platform" : "Workload");
                IEnumerable<String> _changes = Extensions.EnumeratePropertyDifferences(_updateCredential, _update);
                if (_changes.Count() > 0)
                {
                    Copy(_updateCredential, _update);
                    _update.hash_value = _update.GetSHA1Hash();
                    CloudMoveyEvents.add(new Event() { entity = _update.description, severity = 0, component = "Update Credential", summary = _update.ToString() });
                    dbcontext.SaveChanges();
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
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            Credential _remove = dbcontext.Credentials.Single(d => d.id == _destroyCredential.id);
            dbcontext.Credentials.Remove(_remove);
            CloudMoveyEvents.add(new Event() { entity = _remove.description, severity = 0, component = "Destroy Credential", summary = _remove.ToString() });
            dbcontext.SaveChanges();
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
            CloudMoveyEntities dbcontext = new CloudMoveyEntities();
            var Events = from Event in dbcontext.Events select Event;
            return Events.ToList<Event>();
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
