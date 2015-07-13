using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.CaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS1
{

    class ServerImageObject : Core
    {
        public ServerImageObject(DimensionData _dimensiondata) : base(_dimensiondata) { }

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
        public ImagesWithDiskSpeed platformserverimages(List<Option> options=null)
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


        public ImagesWithDiskSpeed customerserverimages(List<Option> options = null)
        {
            orgendpoint("/imageWithDiskSpeed");
            urloptions = options;
            ImagesWithDiskSpeed softwarelabels = get<ImagesWithDiskSpeed>(null, true) as ImagesWithDiskSpeed;
            return softwarelabels;
        }

        /// <summary>
        /// Check if server image exists
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public ImagesWithDiskSpeed serverimageexist(List<Option> options = null)
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
        public ServerImageWithState serverimageget(String image_id)
        {
            orgendpoint(String.Format("/image/{0}", image_id));
            ServerImageWithState softwarelabels = get<ServerImageWithState>(null, true) as ServerImageWithState;
            return softwarelabels;
        }

        /// <summary>
        /// Delete server image
        /// </summary>
        /// <param name="image_id"></param>
        /// <returns></returns>
        public Status serverimagedelete(String image_id)
        {
            orgendpoint(String.Format("/image/{0}?delete", image_id));
            Status softwarelabels = get<Status>(null, true) as Status;
            return softwarelabels;
        }

        /// <summary>
        /// Modify server image
        /// </summary>
        /// <param name="image_id"></param>
        /// <param name="diskoptions"></param>
        /// <returns></returns>
        public Status serverimagemodify(String image_id, List<Option> diskoptions = null)
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
        /// Deploy new server from customer or platform image
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
        public Status serverimagedeploy(String name, String imageId, Boolean start=false, String administratorPassword=null, String description=null, String networkId=null, List<Option> diskoptions=null, String privateIp=null)
        {
            DeployServer deploy = new DeployServer();
            deploy.name = name;
            deploy.imageId = imageId;
            deploy.start = start;
            deploy.administratorPassword = administratorPassword;
            if (!Object.ReferenceEquals(null, description)) { deploy.description = description; }
            if (!Object.ReferenceEquals(null, privateIp)) { deploy.privateIp = privateIp; }
            if (!Object.ReferenceEquals(null, networkId)) { deploy.networkId = networkId; }

            List<DeployServerDisk> disks = new List<DeployServerDisk>();
            foreach (Option disk in diskoptions.FindAll(x => x.option.All(Char.IsNumber)))
            {
                disks.Add(new DeployServerDisk() { scsiId = (Byte)Convert.ToInt32(disk.option), speed = disk.value });
            }
            deploy.disk = disks;

            orgendpoint("/deployServer");
            Status status = post<Status>(deploy, false) as Status;
            return status;
        }

        /// <summary>
        /// Delete Server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status serverdelete(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?delete",server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Modify server with new configuration
        /// </summary>
        /// <param name="server_id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="cpuCount"></param>
        /// <param name="memory"></param>
        /// <param name="privateIp"></param>
        /// <returns></returns>
        public Status servermodify(String server_id, String name = null, String description = null, String cpuCount = null, String memory = null, String privateIp=null)
        {
            ModifyServer modify = new ModifyServer();
            if (!Object.ReferenceEquals(null, name)) { modify.name = name; }
            if (!Object.ReferenceEquals(null, description)) { modify.description = description; }
            if (!Object.ReferenceEquals(null, cpuCount)) { modify.cpuCount = cpuCount; }
            if (!Object.ReferenceEquals(null, memory)) { modify.memory = (Int32.Parse(memory) * 1024).ToString(); }
            if (!Object.ReferenceEquals(null, privateIp)) { modify.privateIp = privateIp; }
            orgendpoint(String.Format("/server/{0}", server_id));
            Status status = post<Status>(modify, true) as Status;
            return status;
        }

        /// <summary>
        /// Start server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status serverstart(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?start", server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Shutdown server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status servershutdown(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?shutdown", server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Poweroff server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status serverpoweroff(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?poweroff", server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Reboot server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status serverreboot(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?reboot", server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Reset server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status serverreset(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?reset", server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Update VMware Tools on server
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public Status serverupdatevmtools(String server_id)
        {
            orgendpoint(String.Format("/server/{0}?updateVMwareTools", server_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Add storage to a server
        /// </summary>
        /// <param name="server_id"></param>
        /// <param name="capacity"></param>
        /// <param name="storagetier"></param>
        /// <returns></returns>
        public Status serveraddstorage(String server_id, int capacity, String storagetier=null)
        {
            if (storagetier != null) { urloptions.Add(new Option { option = "speed", value = storagetier });}
            orgendpoint(String.Format("/server/{0}?addLocalStorage&amount={1}", server_id, capacity));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Remove storage from server
        /// </summary>
        /// <param name="server_id"></param>
        /// <param name="diskindex"></param>
        /// <returns></returns>
        public Status serverremovestorage(String server_id, int disk_id)
        {
            orgendpoint(String.Format("/server/{0}/disk/{1}?delete", server_id, disk_id));
            Status status = get<Status>(null, true) as Status;
            return status;
        }
        /// <summary>
        /// Change storage tier of virtual volume
        /// </summary>
        /// <param name="server_id"></param>
        /// <param name="disk_id"></param>
        /// <param name="storagetier"></param>
        /// <returns></returns>
        public Status serverstoragetierupdate(String server_id, int disk_id, String storagetier)
        {
            ChangeDiskSpeed speed = new ChangeDiskSpeed();
            speed.speed = storagetier;
            orgendpoint(String.Format("/server/{0}/disk/{1}/changeSpeed", server_id, disk_id));
            Status status = post<Status>(speed, false) as Status;
            return status;
        }
        /// <summary>
        /// Change virtual disk size of server
        /// Only bigger disk size is supported by the API
        /// </summary>
        /// <param name="server_id"></param>
        /// <param name="disk_id"></param>
        /// <param name="volumecapacity"></param>
        /// <returns></returns>
        public Status serverdiskexpand(String server_id, int disk_id, int volumecapacity)
        {
            ChangeDiskSize capacity = new ChangeDiskSize();
            capacity.newSizeGb = (byte)volumecapacity;
            orgendpoint(String.Format("/server/{0}/disk/{1}/changeSize", server_id, disk_id));
            Status status = post<Status>(capacity, false) as Status;
            return status;
        }
    }
}
