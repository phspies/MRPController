using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPManagementobject : Core
    {
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
        public MRPManagementobjectType getmanagementobject_dt_id(string _moid)
        {
            endpoint = "/managementobjects/get_id.json";
            MRPManagementobjectDTIDGETType Managementobject = new MRPManagementobjectDTIDGETType()
            {
                moid = _moid

            };
            return post<MRPManagementobjectType>(Managementobject);
        }

        public ResultType createmanagementobject(MRPManagementobjectType _Managementobject)
        {
            MRPManagementobjectsCRUDType Managementobject = new MRPManagementobjectsCRUDType()
            {
                managementobject = _Managementobject
            };

            endpoint = "/managementobjects/create.json";
            return post<ResultType>(Managementobject);
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


