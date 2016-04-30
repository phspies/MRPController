using MRMPService.API.Types.API;
using MRMPService.MRMPService.Types.API;

namespace MRMPService.API
{
    class MRPStacktree : Core
    {

        public MRPStacktree(MRP_ApiClient _MRP) : base(_MRP)
        {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

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


