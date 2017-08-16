using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPWorkload : Core
    {

        public MRPWorkload(MRMPApiClient _MRP) : base(_MRP) { }

        public MRPWorkloadListType list_paged_filtered(MRPWorkloadFilterPagedType _paged_filter_settings)
        {
            endpoint = "/workloads/list_paged_filtered.json";
            return post<MRPWorkloadListType>(_paged_filter_settings);
        }
        public MRPWorkloadListType list_paged_filtered_brief(MRPWorkloadFilterPagedType filter_settings)
        {
            endpoint = "/workloads/list_paged_filtered_brief.json";
            return post<MRPWorkloadListType>(filter_settings);
        }
        public MRMPWorkloadBaseType get_by_id(string _workload_id)
        {
            endpoint = "/workloads/get_by_id.json";
            MRPWorkloadGetIDType _workload_get = new MRPWorkloadGetIDType()
            {
                workload_id = _workload_id
            };
            return post<MRMPWorkloadBaseType>(_workload_get);
        }
        public ResultType createworkload(MRMPWorkloadBaseType _workload)
        {
            _workload.credential = null;
            _workload.platform = null;
            if (_workload.workloadinterfaces != null)
            {
                _workload.workloadinterfaces.ForEach(x => x.platformnetwork = null);
            }
            MRPWorkloadsCRUDType platform = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/create.json";
            return post<ResultType>(platform);
        }
        public ResultType updateworkload(MRMPWorkloadBaseType _workload)
        {
            _workload.credential = null;
            _workload.platform = null;
            if (_workload.workloadinterfaces != null)
            {
                _workload.workloadinterfaces.ForEach(x => x.platformnetwork = null);
            }

            MRPWorkloadsCRUDType workload = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/update.json";
            return put<ResultType>(workload);
        }
        public void InventoryUpdateStatus(MRMPWorkloadBaseType workload, string message, bool status)
        {
            MRMPWorkloadBaseType _update_workload = new MRMPWorkloadBaseType();
            if (status)
            {
                _update_workload.os_collection_status = true;
                _update_workload.os_contact_error_count = 0;
            }
            else
            {
                _update_workload.os_collection_status = false;
                _update_workload.os_contact_error_count++;
            }
            _update_workload.os_collection_message = message;
            _update_workload.os_last_contact = DateTime.UtcNow;
            _update_workload.id = workload.id;
            try
            {
                updateworkload(_update_workload);
            }
            catch( Exception ex)
            {
                Logger.log(String.Format("Error Updating workload information: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }

        public void PeformanceUpdateStatus(MRMPWorkloadBaseType workload, string message, bool status)
        {
            MRMPWorkloadBaseType _update_workload = new MRMPWorkloadBaseType();
            if (status)
            {
                _update_workload.perf_collection_status = true;
                _update_workload.perf_contact_error_count = 0;
            }
            else
            {
                _update_workload.perf_collection_status = false;
                _update_workload.perf_contact_error_count++;
            }
            _update_workload.perf_collection_message = message;
            _update_workload.perf_last_contact = DateTime.UtcNow;
            _update_workload.id = workload.id;
            try
            {
                updateworkload(_update_workload);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error Updating workload information: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }
        public void DoubleTakeUpdateStatus(MRMPWorkloadBaseType workload, string message, bool status)
        {
            MRMPWorkloadBaseType _update_workload = new MRMPWorkloadBaseType();
            if (status)
            {
                _update_workload.dt_collection_status = true;
                _update_workload.dt_contact_error_count = 0;
            }
            else
            {
                _update_workload.dt_collection_status = false;
                _update_workload.dt_contact_error_count++;
            }
            _update_workload.dt_collection_message = message;
            _update_workload.dt_last_contact = DateTime.UtcNow;
            _update_workload.id = workload.id;
            try
            {
                updateworkload(_update_workload);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error Updating workload information: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }

    }
}


