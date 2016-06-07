
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{

    public class Transactions : Core
    {

        public Transactions(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public TransactionStatus getTransactionStatus_Method(long transactionId)
        {
            endpoint = "/transactions/{transactionId}/status";
            endpoint.Replace("{transactionId}", transactionId.ToString());
            mediatype = "application/json";
            return get<TransactionStatus>();
        }


        public TransactionResult getTransactionResult_Method(long transactionId)
        {
            endpoint = "/transactions/{transactionId}/result";
            endpoint.Replace("{transactionId}", transactionId.ToString());
            mediatype = "application/json";
            return get<TransactionResult>();
        }


        public void abortTransaction_Method(long transactionId)
        {
            endpoint = "/transactions/{transactionId}";
            endpoint.Replace("{transactionId}", transactionId.ToString());
            mediatype = "*/*";
            delete();
        }
    }
}
