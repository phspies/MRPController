using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPJob : Core
    {
        public MRPJob(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRPJobListType listjobs()
        {
            endpoint = "/jobs/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPJobListType)post<MRPJobListType>(worker);
        }

        public ResultType getjob_id(string _job_id)
        {
            endpoint = "/jobs/get_id.json";
            MRPJobIDGETType job = new MRPJobIDGETType()
            {
                job_id = _job_id

            };
            return post<ResultType>(job);
        }
        public ResultType getjob_dt_id(string _dt_job_id)
        {
            endpoint = "/jobs/get_id.json";
            MRPJobDTIDGETType job = new MRPJobDTIDGETType()
            {
                dt_job_id = _dt_job_id

            };
            return post<ResultType>(job);
        }

        public ResultType createjob(MRPJobType _job)
        {
            MRPJobsCRUDType job = new MRPJobsCRUDType()
            {
                job = _job
            };

            endpoint = "/jobs/create.json";
            return post<ResultType>(job);
        }
        public ResultType updatejob(MRPJobType _job)
        {
            MRPJobsCRUDType job = new MRPJobsCRUDType()
            {
                job = _job
            };

            endpoint = "/jobs/update.json";
            return put<ResultType>(job);
        }

    }
}


