using MRPService.API.Types.API;
using DoubleTake.Common.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Core.Contract.Connection;
using DoubleTake.Jobs.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MRPService.MRPService.Types.API;
using DoubleTake.Jobs.Contract1;
using MRPService.MRPService.Log;
using MRPService.Utilities;
using Newtonsoft.Json.Linq;

namespace MRPService.DoubleTake
{
    public class Verify
    {
        public static IEnumerable<VerificationStep> verify_job_options(IJobConfigurationVerifier VerifierFactory, String _job_type, RecommendedJobOptions jobInfo, JobCredentials jobCreds)
        {
            ActivityToken activityToken = VerifierFactory.VerifyJobOptions(
                _job_type.ToString(),
                jobInfo.JobOptions,
                jobCreds);

            List<VerificationStep> steps = new List<VerificationStep>();
            VerificationTaskStatus status = VerifierFactory.GetVerificationStatus(activityToken);
            while (
                status.Task.Status != ActivityCompletionStatus.Canceled &&
                status.Task.Status != ActivityCompletionStatus.Completed &&
                status.Task.Status != ActivityCompletionStatus.Faulted)
            {
                Thread.Sleep(1000);
                status = VerifierFactory.GetVerificationStatus(activityToken);
            }

            var failedSteps = status.Steps.Where(s => s.Status == VerificationStatus.Error);

            return failedSteps;

        }
    }
}
