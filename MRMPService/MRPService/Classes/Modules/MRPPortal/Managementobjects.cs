using MRMPService.MRMPAPI.Contracts;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI
{
    public class MRPManagementobject : Core
    {
        public MRPManagementobject(MRMP_ApiClient _MRP) : base(_MRP) { }

        public async Task<MRPManagementobjectListType> listmanagementobjects()
        {
            endpoint = "/managementobjects/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return await post<MRPManagementobjectListType>(worker);
        }
        public async Task<MRPManagementobjectListType> list_filtered(MRManagementobjectFilterType filter_settings)
        {
            endpoint = "/managementobjects/list.json";
            return await post<MRPManagementobjectListType>(filter_settings);
        }
        public async Task<MRPManagementobjectType> getmanagementobject_id(string _managementobject_id)
        {
            endpoint = "/managementobjects/get_id.json";
            MRPManagementobjectIDGETType Managementobject = new MRPManagementobjectIDGETType()
            {
                managementobject_id = _managementobject_id

            };
            return await post<MRPManagementobjectType>(Managementobject);
        }
        public async Task<ResultType> updatemanagementobject(MRPManagementobjectType _Managementobject)
        {
            MRPManagementobjectsCRUDType Managementobject = new MRPManagementobjectsCRUDType()
            {
                managementobject = _Managementobject
            };

            endpoint = "/managementobjects/update.json";
            return await put<ResultType>(Managementobject);
        }

    }
}


