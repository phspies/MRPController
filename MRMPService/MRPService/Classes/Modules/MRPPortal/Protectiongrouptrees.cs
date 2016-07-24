using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;

namespace MRMPService.MRMPAPI
{
    class MRPProtectiongrouptree : Core
    {

        public MRPProtectiongrouptree(MRMP_ApiClient _MRP) : base(_MRP)
        {
        }
        public MRMP_ApiClient MRP = new MRMP_ApiClient();

        public ResultType update(MRPProtectiongrouptreeType _protectiongrouptree)
        {
            MRPProtectiongrouptreeCRUDType stacktree = new MRPProtectiongrouptreeCRUDType()
            {
                protectiongrouptree = _protectiongrouptree
            };

            endpoint = "/protectiongrouptrees/update.json";
            return put<ResultType>(stacktree);
        }
    }
}


