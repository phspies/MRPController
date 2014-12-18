using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cloudManage.Models
{
    class ServerDiskDataModel
    {
        public String id {get; set;}
        public int scsiId {get; set;}
        public int sizeGb {get; set;}
        public String speed {get; set;}
        public String state {get; set;}

    }
    class ServerMachineStatus
    {
        public String name {get; set;}
        public String value {get; set;}
    }
    class ServerDataModel
    {
        public String id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String operatingSystem { get; set; }
        public int cpuCount { get; set; }
        public int memoryMb { get; set; }
        public String resourcePath { get; set; }
        public List<ServerDiskDataModel> disks {get; set;}
        public String softwareLabel { get; set; }
        public String sourceImageId { get; set; }
        public String networkId { get; set; }
        public String machineName { get; set; }
        public String privateIp { get; set; }
        public String publicIp { get; set; }
        public DateTime created { get; set; }
        public Boolean isStarted { get; set; }
        public Boolean isDeployed { get; set; }
        public String state { get; set; }
        public List<ServerMachineStatus> machineStatus { get; set; }

    }
}
