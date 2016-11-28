using MRMPService.MRMPAPI.Contracts;
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
        public MRPManagementobjectListType list_filtered(MRManagementobjectFilterType filter_settings)
        {
            endpoint = "/managementobjects/list.json";
            return post<MRPManagementobjectListType>(filter_settings);
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


