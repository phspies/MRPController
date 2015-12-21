using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace CloudMoveyWorkerService.Database
{
    static class LocalData
    {

        static public type insert_record<type>(object _tablerow) where type : new()
        {
            try
            {
                object _returndata = new object();
                using (LocalDB db = new LocalDB())
                {
                    string _uniq_id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    _tablerow.GetType().GetProperty("id").SetValue(_tablerow, _uniq_id, null);

                    _returndata = db.Set(typeof(type)).Add((type)_tablerow);
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
        static public type update_record<type>(object _tablerow) where type : new()
        {
            try {
                using (LocalDB db = new LocalDB())
                {
                    if (db.Database.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        try { db.Database.Connection.Open(); } catch { }
                    }
                    string _key = (string)_tablerow.GetType().GetProperty("id").GetValue(_tablerow, null);

                    object _dbobject = db.Set(typeof(type)).Find(_key);
                    Utils.Objects.MapObjects(_tablerow, _dbobject);
                    db.SaveChanges();
                    return (type)_dbobject;
                }
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error adding record: {0}", ex.ToString()), EventLogEntryType.Error);
                return new type();
            }

        }
        static public type get_record<type>(string _primarykey_value) where type : new()
        {
            type _returndata = new type();
            try
            {
                using (LocalDB db = new LocalDB())
                {
                    if (db.Database.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        try { db.Database.Connection.Open(); } catch { }
                    }
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
        static public bool delete_record<type>(string _primarykey_value) where type : new()
        {
            try
            {
                using (LocalDB db = new LocalDB())
                {
                    if (db.Database.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        try { db.Database.Connection.Open(); } catch { }
                    }
                    object _dbobject = db.Set(typeof(type)).Find(_primarykey_value);
                    db.Set(typeof(type)).Remove(_dbobject);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error updating record: {0}", ex.ToString()), EventLogEntryType.Error);
                return false;
            }
        }

        static public List<T> get_as_list<T>()
        {
            List<T> _returndata = new List<T>();

            try
            {
                string _class= "";
                using (LocalDB db = new LocalDB())
                {
                    if (db.Database.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        try { db.Database.Connection.Open(); } catch { }
                    }
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
