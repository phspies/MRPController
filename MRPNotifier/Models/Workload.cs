using CloudMRPNotifier.CloudMRPWCF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CloudMRPNotifier.Models
{
    public class Workload_ListDataModel
    {
        CloudMRPServiceClient channel = new CloudMRPServiceClient();

        private List<Workload_ObjectDataModel> _list = new List<Workload_ObjectDataModel>();
        public List<Workload_ObjectDataModel> list
        {
            get
            {
                _list = new List<Workload_ObjectDataModel>();
                foreach (Workload workload in channel.ListWorkloads())
                {
                    Workload_ObjectDataModel _newworkload = new Workload_ObjectDataModel();
                    Objects.MapObjects(workload, _newworkload);
                    _list.Add(_newworkload);
                }
                return _list;
            }
            set { }
        }
        public void addworkload(Workload_ObjectDataModel _workload)
        {
            Workload _wl = new Workload();
            Objects.Copy(_workload, _wl);
            Workload _newwl = channel.AddWorkload(_wl);
            Workload_ObjectDataModel _listwl = new Workload_ObjectDataModel();
            Objects.Copy(_newwl, _listwl);
            _list.Add(_listwl);
        }
        public void updateworkload(Workload_ObjectDataModel _workload)
        {
            Workload _wl = new Workload();
            Objects.Copy(_workload, _wl);
            Workload _newwl = channel.UpdateWorkload(_wl);
            Workload_ObjectDataModel _listwl = _list.FirstOrDefault(x => x.id == _workload.id);
            Objects.Copy(_workload, _listwl);
        }


    }
    public class Workload_ObjectDataModel : INotifyPropertyChanged
    {
        public string _id;
        public string _hostname;
        public string _platform_id;
        public string _credential_id;
        public string _hash_value;
        public string _failovergroup_id;
        public string _moid;
        public string _platform;
        public string _iplist;
        public string _human_os;
        public string _application;
        public string _ostype;
        public string _osedition;
        public bool _enabled;
        public bool _credential_ok;
        public int _vcpu;
        public int _vcore;
        public int _vmemory;
        public long _storage_count;

        public string id
        {
            get { return _id; }
            set
            {
                if (value != this._id)
                {
                    this._id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string hostname
        {
            get { return _hostname; }
            set
            {
                if (value != this._hostname)
                {
                    this._hostname = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string platform_id
        {
            get { return _platform_id; }
            set
            {
                if (value != this._platform_id)
                {
                    this._platform_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string credential_id
        {
            get { return _credential_id; }
            set
            {
                if (value != this._credential_id)
                {
                    this._credential_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string osshort
        {
            get
            {
                string os = "";
                if (osedition.Contains("WIN"))
                {
                    os = "windows";
                }
                else if (osedition.Contains("CENTOS"))
                {
                    os = "centos";
                }
                else if (osedition.Contains("REDHAT"))
                {
                    os = "redhat";
                }
                else if (osedition.Contains("UBUNTU"))
                {
                    os = "ubuntu";
                }
                else if (osedition.Contains("SUSE"))
                {
                    os = "suse";
                }
                return os;
            }
        }
        public string platform_description
        {
            get
            {
                CloudMRPServiceClient channel = new CloudMRPServiceClient();
                return channel.ListPlatforms().FirstOrDefault(x => x.id == platform_id).description;
            }
        }
        public string platform
        {
            get
            {
                CloudMRPServiceClient channel = new CloudMRPServiceClient();
                return channel.ListPlatforms().FirstOrDefault(x => x.id == platform_id).human_vendor;
            }
        }
        public bool credential_ok
        {
            get { return _credential_ok; }
            set
            {
                if (value != this._credential_ok)
                {
                    this._credential_ok = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public int vcore
        {
            get { return _vcore; }
            set
            {
                if (value != this._vcore)
                {
                    this._vcore = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public string iplist
        {
            get { return _iplist; }
            set
            {
                if (value != this._iplist)
                {
                    this._iplist = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public string osedition
        {
            get { return _osedition; }
            set
            {
                if (value != this._osedition)
                {
                    this._osedition = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public string ostype
        {
            get { return _ostype; }
            set
            {
                if (value != this._ostype)
                {
                    this._ostype = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public string application
        {
            get { return _application; }
            set
            {
                if (value != this._application)
                {
                    this._application = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public string human_os
        {
            get
            {
                string major, minor, edition, edition_size;
                major = minor = edition = edition_size = "";
                string cpu = osedition.Split('/')[1];
                if (osedition.Contains("WIN"))
                {
                    major = "Windows";
                    if (osedition.Contains("2008"))
                    {
                        minor = "2008";
                    }
                    else if (osedition.Contains("2012"))
                    {
                        minor = "2012";
                    }
                    if (osedition.Contains("R2"))
                    {
                        edition = "R2";
                    }
                    if (osedition.Contains("DC"))
                    {
                        edition_size = "Datacenter";
                    }
                    else if (osedition.Contains("E"))
                    {
                        edition_size = "Enterprise";
                    }
                    else if (osedition.Contains("S"))
                    {
                        edition_size = "Standard";
                    }
                }
                else if (osedition.Contains("CENTOS"))
                {
                    major = "Centos";
                    minor = osedition.Split('/')[0].Replace("CENTOS", "");
                }
                else if (osedition.Contains("REDHAT"))
                {
                    major = "Redhat";
                    minor = osedition.Split('/')[0].Replace("REDHAT", "");
                }
                else if (osedition.Contains("UBUNTU"))
                {
                    major = "Ubuntu";
                    minor = osedition.Split('/')[0].Replace("UBUNTU", "");
                }
                else if (osedition.Contains("SUSE"))
                {
                    major = "Suse";
                    minor = osedition.Split('/')[0].Replace("SUSE", "");
                }
                string returnval = "";
                if (ostype.ToLower() == "windows")
                {
                    returnval = String.Format("{0} {1} {2} {3} {4}bit Edition", major, minor, edition_size, edition, cpu);
                }
                else if (ostype.ToLower() == "unix")
                {
                    returnval = String.Format("{0} {1} {2}bit Edition", major, minor, cpu);
                }
                return returnval;
            }
            set
            {
                if (value != this._human_os)
                {
                    this._human_os = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public string hash_value
        {
            get { return _hash_value; }
            set
            {
                if (value != this._hash_value)
                {
                    this._hash_value = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public string failovergroup_id
        {
            get { return _failovergroup_id; }
            set
            {
                if (value != this._failovergroup_id)
                {
                    this._failovergroup_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string moid
        {
            get { return _moid; }
            set
            {
                if (value != this._moid)
                {
                    this._moid = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool enabled
        {
            get { return _enabled; }
            set
            {
                if (value != this._enabled)
                {
                    this._enabled = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int vcpu
        {
            get { return _vcpu; }
            set { _vcpu = value; }
        }
        public int vmemory
        {
            get { return _vmemory; }
            set
            {
                if (value != this._vmemory)
                {
                    this._vmemory = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public long storage_count
        {
            get { return _storage_count; }
            set
            {
                if (value != this._storage_count)
                {
                    this._storage_count = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        CloudMRPServiceClient channel = new CloudMRPServiceClient();

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null && id != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                Workload _updateworkload = new Workload();

                Objects.MapObjects(this, _updateworkload);
                channel.UpdateWorkloadAsync(_updateworkload);
            }
        }
    }
}

namespace Utils
{
    class Objects
    {
        public static void Copy(object src, object dest)
        {
            if (src != null)
            {
                foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(dest))
                {
                    item.SetValue(dest, item.GetValue(src));
                }
            }
        }

        public static object MapObjects(object source, object target)
        {
            foreach (PropertyInfo sourceProp in source.GetType().GetProperties())
            {
                PropertyInfo targetProp = target.GetType().GetProperties().Where(p => p.Name == sourceProp.Name).FirstOrDefault();
                if (targetProp != null && targetProp.GetType().Name == sourceProp.GetType().Name)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
        }

    }
}