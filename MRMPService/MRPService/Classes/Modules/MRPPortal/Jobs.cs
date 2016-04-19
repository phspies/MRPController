using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class MRPJob : Core
    {
        public MRPJob(MRP_ApiClient _MRP) : base(_MRP) {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

        public MRPJobListType listjobs()
        {
            endpoint = "/jobs/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPJobListType)post<MRPJobListType>(worker);
        }

        public MRPJobType getjob_id(string _job_id)
        {
            endpoint = "/jobs/get_id.json";
            MRPJobIDGETType job = new MRPJobIDGETType()
            {
                job_id = _job_id
                
            };
            return post<MRPJobType>(job);
        }
        public MRPJobType getjob_dt_id(string _dt_job_id)
        {
            endpoint = "/jobs/get_id.json";
            MRPJobDTIDGETType job = new MRPJobDTIDGETType()
            {
                dt_job_id = _dt_job_id

            };
            return post<MRPJobType>(job);
        }

        public MRPJobType createjob(MRPJobType _job)
        {
            MRPJobsCRUDType job = new MRPJobsCRUDType()
            {
                job = _job
            };

            endpoint = "/jobs/create.json";
            return post<MRPJobType>(job);
        }
        public MRPJobType updatejob(MRPJobType _job)
        {
            MRPJobsCRUDType job = new MRPJobsCRUDType()
            {
                job = _job
            };

            endpoint = "/jobs/update.json";
            return (MRPJobType)put<MRPJobType>(job);
        }

    }
}


