using MRMPService.MRMPAPI.Contracts;
using System;

namespace MRMPService.MRMPAPI
{
    class MRPWorkload : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPWorkload()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public MRPWorkload(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRMP_ApiClient MRP = new MRMP_ApiClient();

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
        public MRPWorkloadType get_by_id(string _workload_id)
        {
            endpoint = "/workloads/get_by_id.json";
            MRPWorkloadGetIDType _workload_get = new MRPWorkloadGetIDType()
            {
                workload_id = _workload_id
            };
            return post<MRPWorkloadType>(_workload_get);
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
            _workload.credential = null;
            _workload.platform = null;
            if (_workload.workloadinterfaces != null)
            {
                if (_workload.workloadinterfaces.Count == 0)
                {
                    _workload.workloadinterfaces = null;
                }
            }
            if (_workload.workloadprocesses != null)
            {
                if (_workload.workloadprocesses.Count == 0)
                {
                    _workload.workloadprocesses = null;
                }
            }
            if (_workload.workloadsoftwares != null)
            {
                if (_workload.workloadsoftwares.Count == 0)
                {
                    _workload.workloadsoftwares = null;
                }
            }
            if (_workload.workloadvolumes != null)
            {
                if (_workload.workloadvolumes.Count == 0)
                {
                    _workload.workloadvolumes = null;
                }
            }

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


