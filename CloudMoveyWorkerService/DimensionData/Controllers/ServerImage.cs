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

    class WorkloadObject : Core
    {
        public WorkloadObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        /// <summary>
        /// Check if workload image exists
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public ServersType workloadimageexist(List<Option> options = null)
        {
            orgendpoint("/image/nameExists");
            urloptions = options;
            ServersType softwarelabels = get<ServersType>(null, true) as ServersType;
            return softwarelabels;
        }

 
        public Status workloadmodify(String image_id, List<Option> diskoptions = null)
        {
            ReconfigureServerType modify = new ReconfigureServerType();
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
        public Status workloaddeploy(DeployServerType _workload)
        {
            orgendpoint("/server/deployServer");
            Status status = post<Status>(_workload, false) as Status;
            return status;
        }
        public Status workloaddelete(DeleteServerType _deleteworkload)
        {
            orgendpoint("/server/deleteServer");
            Status status = post<Status>(_deleteworkload, false) as Status;
            return status;
        }
        public Status workloadmodify(String workload_id, String name = null, String description = null, int cpuCount=0, int memory=0, String privateIp=null)
        {
            ReconfigureServerType modify = new ReconfigureServerType();
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
