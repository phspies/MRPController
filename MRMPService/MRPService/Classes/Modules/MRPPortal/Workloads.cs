using MRMPService.MRMPAPI.Types.API;
using System;

namespace MRMPService.MRMPAPI
{
    class MRPWorkload : Core
    {
        public MRPWorkload(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRMP_ApiClient MRP = new MRMP_ApiClient();

        public MRPWorkloadListType listworkloads()
        {
            endpoint = "/workloads/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPWorkloadListType>(worker);
        }

        public MRPWorkloadListType list_dt_installed()
        {
            endpoint = "/workloads/list_dt_installed.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPWorkloadListType>(worker);
        }

        public MRPWorkloadListType list_by_platform(MRPPlatformType _platform)
        {
            endpoint = "/workloads/list_by_platform.json";
            MRPPlatformGETType _get_workloads_platform = new MRPPlatformGETType()
            {
                platform_id = _platform.id
            };
            return post<MRPWorkloadListType>(_get_workloads_platform);
        }
        public MRPWorkloadListType list_by_platform_all(MRPPlatformType _platform)
        {
            endpoint = "/workloads/list_by_platform_all.json";
            MRPPlatformGETType _get_workloads_platform = new MRPPlatformGETType()
            {
                platform_id = _platform.id
            };
            return post<MRPWorkloadListType>(_get_workloads_platform);
        }
        public ResultType createworkload(MRPWorkloadType _workload)
        {
            MRPWorkloadsCRUDType platform = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/create.json";
            return post<ResultType>(platform);
        }
        public ResultType updateworkload(MRPWorkloadType _workload)
        {
            MRPWorkloadsCRUDType workload = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/update.json";
            return put<ResultType>(workload);
        }
        public void InventoryUpdateStatus(MRPWorkloadType workload, string message, bool status)
        {
            MRPWorkloadType _update_workload = new MRPWorkloadType();
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

            updateworkload(_update_workload);
        }

        public void PeformanceUpdateStatus(MRPWorkloadType workload, string message, bool status)
        {
            MRPWorkloadType _update_workload = new MRPWorkloadType();
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
            updateworkload(_update_workload);

        }
        public void DoubleTakeUpdateStatus(MRPWorkloadType workload, string message, bool status)
        {
            MRPWorkloadType _update_workload = new MRPWorkloadType();
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
            updateworkload(_update_workload);

        }

    }
}


