using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.DoubleTake
{
    enum dt_server_type { source, target }

    public static class DT_JobTypes
    {
        public static string HA_FilesFolders = "FilesAndFolders";
        public static string HA_Cluster_FilesFolders = "ClusterAwareFilesAndFolders";
        public static string HA_Full_Failover = "FullServerFailover";
        public static string HA_Linux_FullFailover = "LinuxFullServerFailover";
        public static string HA_SQL_Failover = "Sql";
        public static string HA_SQL_Cluster_Failover = "SqlClustered";
        public static string HA_Exchange_Failover = "Exchange";
        public static string HA_Exchange_Cluster_Failover = "ExchangeClustered";
        public static string DR_Data_Protection = "DataOnlyImageProtection";
        public static string DR_Data_Recovery = "DataOnlyImageRecovery";
        public static string DR_Full_Protection = "FullServerImageProtection";
        public static string DR_Full_Recovery = "FullServerImageRecovery";
        public static string Move_Server_Migration = "MoveServerMigration";
        public static string Move_DataOnly_Migration = "MoveDataOnlyMigration";
        public static string Move_VRA_Migration = "VraMove";
    }
    public enum DT_WorkloadType { Source, Target};
}
