using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;

namespace MRMPService.MRMPAPI
{
    class MRPStacktree : Core
    {

        public MRPStacktree(MRMP_ApiClient _MRP) : base(_MRP)
        {
        }
        public MRMP_ApiClient MRP = new MRMP_ApiClient();

        public ResultType update(MRPStacktreeType _stacktree)
        {
            MRPStacktreeCRUDType stacktree = new MRPStacktreeCRUDType()
            {
                stacktree = _stacktree
            };

            endpoint = "/stacktrees/update.json";
            return put<ResultType>(stacktree);
        }
    }
}


