using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using DoubleTakeRestProxy;

namespace DoubleTakeManagementServiceRestProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDoubleTakeJobManager" in both code and config file together.
    [ServiceContract]
    public interface IDoubleTakeManagementService
    {
        //[OperationContract]
        //[System.ServiceModel.Web.WebInvoke(Method = "GET", ResponseFormat = System.ServiceModel.Web.WebMessageFormat.Xml, BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Wrapped, UriTemplate = "xml/{id}")]
        //string XMLData(string id);

        //[OperationContract]
        //[System.ServiceModel.Web.WebInvoke(Method = "GET", ResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json, BodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Wrapped, UriTemplate = "json/{id}")]
        //string JSONData(string id);

        //:add, :create, :delete, :edit, :get_qualification_results, :update_recommended_job_options, :verify_job_options, 
        //:fix_job_options, :get_job, :get_jobs, :start, :stop, :pause, :failover, :get_recommended_failover_options, :verify_failover_options, 
        //:failback, :get_recommended_failback_options, :verify_failback_options, :restore, :get_recommended_restore_options, :verify_restore_options, //
        //:reverse, :undo_failover, :get_verification_status, :get_fix_job_options_status, :update_credentials, :persist_shares, :get_action_status, 
        //:get_action_statuses, :get_log_messages]


        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "/GetProductInfo")]
        String GetProductInfo(JobManagerRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "/GetImages")]
        String GetImages(JobManagerRequest request);
    }
}


 