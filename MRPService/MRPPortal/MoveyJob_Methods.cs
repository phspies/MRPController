﻿using MRPService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.Portal
{
    class MRPJob : Core
    {
        public MRPJob(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
        public CloudMRPPortal CloudMRP = new CloudMRPPortal();

        public MRPJobListType listjobs()
        {
            endpoint = "/api/v1/jobs/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPJobListType)post<MRPJobListType>(worker);
        }

        public MRPJobType getjob_id(string _job_id)
        {
            endpoint = "/api/v1/jobs/get_id.json";
            MRPJobIDGETType job = new MRPJobIDGETType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                job_id = _job_id
                
            };
            return post<MRPJobType>(job);
        }
        public MRPJobType getjob_dt_id(string _dt_job_id)
        {
            endpoint = "/api/v1/jobs/get_id.json";
            MRPJobDTIDGETType job = new MRPJobDTIDGETType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                dt_job_id = _dt_job_id

            };
            return post<MRPJobType>(job);
        }

        public MRPJobType createjob(MRPJobType _job)
        {
            MRPJobsCRUDType job = new MRPJobsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                job = _job
            };

            endpoint = "/api/v1/jobs/create.json";
            return post<MRPJobType>(job);
        }
        public MRPJobType updatejob(MRPJobType _job)
        {
            MRPJobsCRUDType job = new MRPJobsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                job = _job
            };

            endpoint = "/api/v1/jobs/update.json";
            return (MRPJobType)put<MRPJobType>(job);
        }

    }
}


