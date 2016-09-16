using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPDoubleTake
{
    enum dt_server_type { source, target }

    public class DT_JobTypes
    {
        public const string HA_FilesFolders = "FilesAndFolders";
        public const string HA_Cluster_FilesFolders = "ClusterAwareFilesAndFolders";
        public const string HA_Full_Failover = "FullServerFailover";
        public const string HA_Linux_FullFailover = "LinuxFullServerFailover";
        public const string HA_SQL_Failover = "Sql";
        public const string HA_SQL_Cluster_Failover = "SqlClustered";
        public const string HA_Exchange_Failover = "Exchange";
        public const string HA_Exchange_Cluster_Failover = "ExchangeClustered";
        public const string DR_Data_Protection = "DataOnlyImageProtection";
        public const string DR_Data_Recovery = "DataOnlyImageRecovery";
        public const string DR_Full_Protection = "FullServerImageProtection";
        public const string DR_Full_Recovery = "FullServerImageRecovery";
        public const string Move_Server_Migration = "MoveServerMigration";
        public const string Move_DataOnly_Migration = "MoveDataOnlyMigration";
        public const string Move_VRA_Migration = "VraMove";
    }
    public enum DT_WorkloadType { Source, Target};
}
