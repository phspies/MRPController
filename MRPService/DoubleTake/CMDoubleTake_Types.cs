using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.CMDoubleTake.Types
{
    public static class DTJobTypes
    {
        public static string HA_FilesFolders = "FilesAndFolders";
        public static string HA_Cluster_FilesFolders = "ClusterAwareFilesAndFolders";
        public static string HA_Full_Failover = "FullWorkloadFailover";
        public static string HA_Linux_FullFailover = "LinuxFullWorkloadFailover";
        public static string HA_SQL_Failover = "Sql";
        public static string HA_SQL_Cluster_Failover = "SqlClustered";
        public static string HA_Exchange_Failover = "Exchange";
        public static string HA_Exchange_Cluster_Failover = "ExchangeClustered";
        public static string DR_Data_Protection = "DataOnlyImageProtection";
        public static string DR_Data_Recovery = "DataOnlyImageRecovery";
        public static string DR_Full_Protection = "FullWorkloadImageProtection";
        public static string DR_Full_Recovery = "FullWorkloadImageRecovery";
    }
    enum CMWorkloadType { Source, Target};
}
