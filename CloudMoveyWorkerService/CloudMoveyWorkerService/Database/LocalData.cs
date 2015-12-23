using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace CloudMoveyWorkerService.Database
{
    class LocalData
    {

        public type insert_record<type>(object _tablerow) where type : new()
        {
            try
            {
                object _returndata = new object();
                using (LocalDB db = new LocalDB())
                {
                    string _current_id = _tablerow.GetType().GetProperty("id").GetValue(_tablerow) as string;

                    if (String.IsNullOrEmpty(_current_id))
                    {
                        string _uniq_id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                        _tablerow.GetType().GetProperty("id").SetValue(_tablerow, _uniq_id, null);
                    }
                    string _class = "";
                    switch (_class = typeof(type).Name.ToLower())
                    {
                        case "workload":
                            _returndata = db.Workloads.Add((Workload)_tablerow);
                            break;
                        case "credential":
                            _returndata = db.Credentials.Add((Credential)_tablerow);
                            break;
                        case "networkflow":
                            _returndata = db.NetworkFlows.Add((NetworkFlow)_tablerow);
                            break;
                        case "performance":
                            _returndata = db.Performance.Add((Performance)_tablerow);
                            break;
                        case "platform":
                            _returndata = db.Platforms.Add((Platform)_tablerow);
                            break;
                        case "event":
                            _returndata = db.Events.Add((Event)_tablerow);
                            break;
                        default:
                            Global.event_log.WriteEntry(String.Format("Error processing table type: {0}", _class), EventLogEntryType.Error);
                            break;
                    }
                    db.SaveChanges();
                }
                return (type)_returndata;
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0}", ex.ToString()), EventLogEntryType.Error);
                return new type();
            }
        }
        public type update_record<type>(object _tablerow) where type : new()
        {
            try {
                using (LocalDB db = new LocalDB())
                {

                    string _key = (string)_tablerow.GetType().GetProperty("id").GetValue(_tablerow, null);
                    object _dbobject = new object();
                    string _class = "";

                    switch (_class = typeof(type).Name.ToLower())
                    {
                        case "workload":
                            _dbobject = db.Workloads.Find(_key);
                            Utils.Objects.MapObjects(_tablerow, _dbobject);
                            db.SaveChanges();
                            break;
                        case "credential":
                            _dbobject = db.Credentials.Find(_key);
                            Utils.Objects.MapObjects(_tablerow, _dbobject);
                            db.SaveChanges();
                            break;
                        case "networkflow":
                            _dbobject = db.NetworkFlows.Find(_key);
                            Utils.Objects.MapObjects(_tablerow, _dbobject);
                            db.SaveChanges();
                            break;
                        case "performance":
                            _dbobject = db.Performance.Find(_key);
                            Utils.Objects.MapObjects(_tablerow, _dbobject);
                            db.SaveChanges();
                            break;
                        case "platform":
                            _dbobject = db.Platforms.Find(_key);
                            Utils.Objects.MapObjects(_tablerow, _dbobject);
                            db.SaveChanges();
                            break;
                        case "event":
                            _dbobject = db.Events.Find(_key);
                            Utils.Objects.MapObjects(_tablerow, _dbobject);
                            db.SaveChanges();
                            break;
                        default:
                            Global.event_log.WriteEntry(String.Format("Error processing table type: {0}", _class), EventLogEntryType.Error);
                            break;
                    }
                    return (type)_dbobject;
                }
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0}", ex.ToString()), EventLogEntryType.Error);
                return new type();
            }

        }
        public type get_record<type>(string _primarykey_value) where type : new()
        {
            type _returndata = new type();
            try
            {
                using (LocalDB db = new LocalDB())
                {
                    object _dbobject = db.Set(typeof(type)).Find(_primarykey_value);
                    return (type)_dbobject;
                }
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", ex.ToString()), EventLogEntryType.Error);
                return _returndata;
            }
        }
        public bool delete_record<type>(string _primarykey_value) where type : new()
        {
            try
            {
                using (LocalDB db = new LocalDB())
                {
                    string _class = "";
                    switch (_class = typeof(type).Name.ToLower())
                    {
                        case "workload":
                            db.Workloads.Remove(db.Workloads.Find(_primarykey_value));
                            db.SaveChanges();
                            break;
                        case "credential":
                            db.Credentials.Remove(db.Credentials.Find(_primarykey_value));
                            db.SaveChanges();
                            break;
                        case "networkflow":
                            db.NetworkFlows.Remove(db.NetworkFlows.Find(_primarykey_value));
                            db.SaveChanges();
                            break;
                        case "performance":
                            db.Performance.Remove(db.Performance.Find(_primarykey_value));
                            db.SaveChanges();
                            break;
                        case "platform":
                            db.Platforms.Remove(db.Platforms.Find(_primarykey_value));
                            db.SaveChanges();
                            break;
                        case "event":
                            db.Events.Remove(db.Events.Find(_primarykey_value));
                            db.SaveChanges();
                            break;
                        default:
                            Global.event_log.WriteEntry(String.Format("Error processing table type: {0}", _class), EventLogEntryType.Error);
                            break;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", ex.ToString()), EventLogEntryType.Error);
                return false;
            }
        }

        public List<T> get_as_list<T>()
        {
            List<T> _returndata = new List<T>();

            try
            {
                string _class= "";
                using (LocalDB db = new LocalDB())
                {

                    switch(_class = typeof(T).Name.ToLower())
                    {
                        case "workload":
                            _returndata = db.Workloads.Cast<T>().ToList<T>();
                            break;
                        case "credential":
                            _returndata = db.Credentials.Cast<T>().ToList<T>();
                            break;
                        case "networkflow":
                            _returndata = db.NetworkFlows.Cast<T>().ToList<T>();
                            break;
                        case "performance":
                            _returndata = db.Performance.Cast<T>().ToList<T>();
                            break;
                        case "platform":
                            _returndata = db.Platforms.Cast<T>().ToList<T>();
                            break;
                        case "event":
                            _returndata = db.Events.Cast<T>().ToList<T>();
                            break;
                        default:
                            Global.event_log.WriteEntry(String.Format("Error processing table type: {0}", _class), EventLogEntryType.Error);
                            return new List<T>();
                    }
                }

            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error getting all records: {0}", ex.ToString()), EventLogEntryType.Error);
                return new List<T>();
            }
            return _returndata;
        }

    }

}
