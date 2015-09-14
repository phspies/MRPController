using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS
{

    class WorkloadImageObject : Core
    {
        public WorkloadImageObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

        /// <summary>
        /// List platform images
        /// Available options:
        ///     Paging/Ordering Optional Parameters:
        ///         &pageSize=
        ///         &pageNumber=
        ///         &orderBy=
        ///     Filter Optional Parameters:
        ///         &id=
        ///         &location=
        ///         &name=
        ///         &created=
        ///         &state=
        ///         &operatingSystemId=
        ///         &operatingSystemFamily=
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public ImagesWithDiskSpeed platformworkloadimages(List<Option> options=null)
        {
            simpleendpoint("/base/imageWithDiskSpeed");
            urloptions = options;
            ImagesWithDiskSpeed softwarelabels = get<ImagesWithDiskSpeed>(null, true) as ImagesWithDiskSpeed;
            return softwarelabels;
        }

        /// <summary>
        /// List customer images
        /// Available options:
        ///     Paging/Ordering Optional Parameters:
        ///         &pageSize=
        ///         &pageNumber=
        ///         &orderBy=
        ///     Filter Optional Parameters:
        ///         &id=
        ///         &location=
        ///         &name=
        ///         &created=
        ///         &state=
        ///         &operatingSystemId=
        ///         &operatingSystemFamily=
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        /// 


        public CustomerImagesWithDiskSpeed customerworkloadimages(List<Option> options = null)
        {
            orgendpoint("/imageWithDiskSpeed");
            urloptions = options;
            CustomerImagesWithDiskSpeed customerworkloadimages = get<CustomerImagesWithDiskSpeed>(null, true) as CustomerImagesWithDiskSpeed;
            return customerworkloadimages;
        }

        /// <summary>
        /// Check if workload image exists
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public ImagesWithDiskSpeed workloadimageexist(List<Option> options = null)
        {
            orgendpoint("/image/nameExists");
            urloptions = options;
            ImagesWithDiskSpeed softwarelabels = get<ImagesWithDiskSpeed>(null, true) as ImagesWithDiskSpeed;
            return softwarelabels;
        }

        /// <summary>
        /// Retrieve image information
        /// </summary>
        /// <param name="image_id"></param>
        /// <returns></returns>
        public ServerImageWithState workloadimageget(String image_id)
        {
            orgendpoint(String.Format("/image/{0}", image_id));
            ServerImageWithState softwarelabels = get<ServerImageWithState>(null, true) as ServerImageWithState;
            return softwarelabels;
        }

        /// <summary>
        /// Delete workload image
        /// </summary>
        /// <param name="image_id"></param>
        /// <returns></returns>
        public Status workloadimagedelete(String image_id)
        {
            orgendpoint(String.Format("/image/{0}?delete", image_id));
            Status softwarelabels = get<Status>(null, true) as Status;
            return softwarelabels;
        }

        /// <summary>
        /// Modify workload image
        /// </summary>
        /// <param name="image_id"></param>
        /// <param name="diskoptions"></param>
        /// <returns></returns>
        public Status workloadimagemodify(String image_id, List<Option> diskoptions = null)
        {
            ModifyServerImage modify = new ModifyServerImage();
            modify.description = diskoptions.Find(x => x.option == "description").value;
            List<ModifyServerImageDisk> disks = new List<ModifyServerImageDisk>();
            foreach (Option diskindex in diskoptions.FindAll(x => x.option.All(Char.IsNumber)))
            {
                disks.Add(new ModifyServerImageDisk() { scsiId = (Byte)Convert.ToInt32(diskindex.option), speed = diskindex.value });
            }
            modify.disk = disks;
            orgendpoint(String.Format("/image/{0}/modify", image_id));
            Status status = post<Status>(modify, false) as Status;
            return status;
        }

        /// <summary>
        /// Deploy new workload from customer or platform image
        /// Additional options for disk tier can be passed with diskindex and tier values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageId"></param>
        /// <param name="start"></param>
        /// <param name="administratorPassword"></param>
        /// <param name="description"></param>
        /// <param name="networkId"></param>
        /// <param name="diskoptions"></param>
        /// <param name="privateIp"></param>
        /// <returns></returns>
        public Status workloadimagedeploy(DeployServer _workload)
        {
            orgendpoint("/deployWorkload");
            Status status = post<Status>(_workload, false) as Status;
            return status;
        }

        /// <summary>
        /// Delete Workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloaddelete(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?delete",workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Modify workload with new configuration
        /// </summary>
        /// <param name="workload_id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="cpuCount"></param>
        /// <param name="memory"></param>
        /// <param name="privateIp"></param>
        /// <returns></returns>
        public Status workloadmodify(String workload_id, String name = null, String description = null, int cpuCount=0, int memory=0, String privateIp=null)
        {
            ModifyServer modify = new ModifyServer();
            if (!Object.ReferenceEquals(null, name)) { modify.name = name; }
            if (!Object.ReferenceEquals(null, description)) { modify.description = description; }
            if (cpuCount!=0) { modify.cpuCount = cpuCount; }
            if (memory!=0) { modify.memory = (memory * 1024); }
            if (!Object.ReferenceEquals(null, privateIp)) { modify.privateIp = privateIp; }
            orgendpoint(String.Format("/workload/{0}", workload_id));
            Status status = post<Status>(modify, true) as Status;
            return status;
        }

        /// <summary>
        /// Start workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloadstart(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?start", workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Shutdown workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloadshutdown(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?shutdown", workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Poweroff workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloadpoweroff(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?poweroff", workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Reboot workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloadreboot(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?reboot", workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Reset workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloadreset(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?reset", workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Update VMware Tools on workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <returns></returns>
        public Status workloadupdatevmtools(String workload_id)
        {
            orgendpoint(String.Format("/workload/{0}?updateVMwareTools", workload_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Add storage to a workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <param name="capacity"></param>
        /// <param name="storagetier"></param>
        /// <returns></returns>
        public Status workloadaddstorage(String workload_id, int capacity, String storagetier=null)
        {
            if (storagetier != null) { urloptions.Add(new Option { option = "speed", value = storagetier });}
            orgendpoint(String.Format("/workload/{0}?addLocalStorage&amount={1}", workload_id, capacity));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Remove storage from workload
        /// </summary>
        /// <param name="workload_id"></param>
        /// <param name="diskindex"></param>
        /// <returns></returns>
        public Status workloadremovestorage(String workload_id, int disk_id)
        {
            orgendpoint(String.Format("/workload/{0}/disk/{1}?delete", workload_id, disk_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Change storage tier of virtual volume
        /// </summary>
        /// <param name="workload_id"></param>
        /// <param name="disk_id"></param>
        /// <param name="storagetier"></param>
        /// <returns></returns>
        public Status workloadstoragetierupdate(String workload_id, int disk_id, String storagetier)
        {
            ChangeDiskSpeed speed = new ChangeDiskSpeed();
            speed.speed = storagetier;
            orgendpoint(String.Format("/workload/{0}/disk/{1}/changeSpeed", workload_id, disk_id));
            Status status = post<Status>(speed, false) as Status;
            return status;
        }
        /// <summary>
        /// Change virtual disk size of workload
        /// Only bigger disk size is supported by the API
        /// </summary>
        /// <param name="workload_id"></param>
        /// <param name="disk_id"></param>
        /// <param name="volumecapacity"></param>
        /// <returns></returns>
        public Status workloaddiskexpand(String workload_id, int disk_id, int volumecapacity)
        {
            ChangeDiskSize capacity = new ChangeDiskSize();
            capacity.newSizeGb = (byte)volumecapacity;
            orgendpoint(String.Format("/workload/{0}/disk/{1}/changeSize", workload_id, disk_id));
            Status status = post<Status>(capacity, false) as Status;
            return status;
        }
    }
}
