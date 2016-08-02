using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPManagementobjectSnapshots : Core
    {
        public MRPManagementobjectSnapshots(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRPManagementobjectSnapshotListType list()
        {
            endpoint = "/managementobjectsnapshots/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPManagementobjectSnapshotListType>(worker);
        }

        public ResultType get_id(string _image_id)
        {
            endpoint = "/managementobjectsnapshots/get_id.json";
            MRPManagementobjectSnapshotIDGETType image = new MRPManagementobjectSnapshotIDGETType()
            {
                image_id = _image_id
            };
            return post<ResultType>(image);
        }
        public ResultType get_moid(string _image_moid)
        {
            endpoint = "/managementobjectsnapshots/get_id.json";
            MRPManagementobjectSnapshotMOIDGETType job = new MRPManagementobjectSnapshotMOIDGETType()
            {
                 moid_id = _image_moid
            };
            return post<ResultType>(job);
        }

        public ResultType create(MRPManagementobjectSnapshotType _ManagementobjectSnapshot)
        {
            MRPManagementobjectSnapshotsCRUDType job = new MRPManagementobjectSnapshotsCRUDType()
            {
                 ManagementobjectSnapshot = _ManagementobjectSnapshot
            };

            endpoint = "/managementobjectsnapshots/create.json";
            return post<ResultType>(job);
        }
        public ResultType update(MRPManagementobjectSnapshotType _ManagementobjectSnapshot)
        {
            MRPManagementobjectSnapshotsCRUDType job = new MRPManagementobjectSnapshotsCRUDType()
            {
                 ManagementobjectSnapshot = _ManagementobjectSnapshot
            };

            endpoint = "/managementobjectsnapshots/update.json";
            return put<ResultType>(job);
        }

    }
}


