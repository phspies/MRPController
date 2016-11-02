using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPManagementobject : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPManagementobject()
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
        public MRPManagementobject(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRPManagementobjectListType listmanagementobjects()
        {
            endpoint = "/managementobjects/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPManagementobjectListType)post<MRPManagementobjectListType>(worker);
        }

        public MRPManagementobjectType getmanagementobject_id(string _managementobject_id)
        {
            endpoint = "/managementobjects/get_id.json";
            MRPManagementobjectIDGETType Managementobject = new MRPManagementobjectIDGETType()
            {
                managementobject_id = _managementobject_id

            };
            return post<MRPManagementobjectType>(Managementobject);
        }
        public ResultType updatemanagementobject(MRPManagementobjectType _Managementobject)
        {
            MRPManagementobjectsCRUDType Managementobject = new MRPManagementobjectsCRUDType()
            {
                managementobject = _Managementobject
            };

            endpoint = "/managementobjects/update.json";
            return put<ResultType>(Managementobject);
        }

    }
}


