using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTExamples
{
    //class DRRecoveryBase: JobBase
    //{
    //    public class DRImageRecoveryOptions : DefaultOptions
    //    {
    //        [Option("sourceServerName", Required = true, HelpText = "The source server name whose image is created using DataOnlyImageProtection job on the repository server.")]
    //        public string SourceServerName { get; set; }

    //    }

    //    private DRImageRecoveryOptions drOptions = new DRImageRecoveryOptions();
    //    private string drJobType;

    //    public DRRecoveryBase(string jobType) :
    //        base(jobType)
    //    {
    //        Options = drOptions;
    //        this.drJobType = jobType;
    //    }

    //    protected override async Task<WorkloadModel> CreateWorkload()
    //    {
    //        SimpleLog.Log("Creating a DR Recovery workload.");

    //        if (String.IsNullOrEmpty(drOptions.SourceServerName))
    //        {
    //            throw new Exception("The Source server name was not provided. Exiting...");
    //        }

    //        var connection = await ManagementService.GetConnectionAsync(Options.Source);
    //        ImagesApi imagesApi = new ImagesApi(connection);
    //        ApiResponse<IEnumerable<ImageInfoModel>> images = await imagesApi.GetImagesAsync();
    //        ImageInfoModel image = images.Content.Where(i => i.ImageName == drOptions.SourceServerName && 
    //                                i.ImageType == ((drJobType == "DataOnlyImageRecovery") ? ImageType.DataOnly : ImageType.FullServer)).First();

    //        if (image == null || image.Id == Guid.Empty) throw new Exception("Source image not found on the respository server");

    //        workloadApi = new WorkloadsApi(connection);

    //        SimpleLog.Log("Creating workload...");

    //        var workloadResult = await workloadApi.CreateWorkloadAsync(JOB_TYPE, image.Id, Guid.Empty);
    //        workloadResult.EnsureSuccessStatusCode();

    //        SimpleLog.Log("Workload created.");

    //        WorkloadModel workload = workloadResult.Content;

    //        SimpleLog.Log("DR Recovery workload created and sent to server.");
    //        return await GetWorkload(workload.Id);
    //    }
    //}
}
