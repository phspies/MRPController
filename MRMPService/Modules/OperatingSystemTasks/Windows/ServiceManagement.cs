using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.OperatingSystemTasks.Windows
{
    public class ServiceManagement
    {
        public enum ServiceManagementServiceType : uint
        {
            KernelDriver = 0x1,
            FileSystemDriver = 0x2,
            Adapter = 0x4,
            RecognizerDriver = 0x8,
            OwnProcess = 0x10,
            ShareProcess = 0x20,
            Interactive = 0x100
        }

        public enum ServiceManagementReturnValue
        {
            Success = 0,
            NotSupported = 1,
            AccessDenied = 2,
            DependentServicesRunning = 3,
            InvalidServiceControl = 4,
            ServiceCannotAcceptControl = 5,
            ServiceNotActive = 6,
            ServiceRequestTimeout = 7,
            UnknownFailure = 8,
            PathNotFound = 9,
            ServiceAlreadyRunning = 10,
            ServiceDatabaseLocked = 11,
            ServiceDependencyDeleted = 12,
            ServiceDependencyFailure = 13,
            ServiceDisabled = 14,
            ServiceLogonFailure = 15,
            ServiceMarkedForDeletion = 16,
            ServiceNoThread = 17,
            StatusCircularDependency = 18,
            StatusDuplicateName = 19,
            StatusInvalidName = 20,
            StatusInvalidParameter = 21,
            StatusInvalidServiceAccount = 22,
            StatusServiceExists = 23,
            ServiceAlreadyPaused = 24,
            ServiceNotFound = 25
        }

        public enum ServiceManagementOnError
        {
            UserIsNotNotified = 0,
            UserIsNotified = 1,
            SystemRestartedLastGoodConfiguraion = 2,
            SystemAttemptStartWithGoodConfiguration = 3
        }

        public enum ServiceManagementStartMode
        {
            Boot = 0,
            System = 1,
            Automatic = 2,
            Manual = 3,
            Disabled = 4
        }

        public enum ServiceManagementServiceState
        {
            Running,
            Stopped,
            Paused,
            StartPending,
            StopPending,
            PausePending,
            ContinuePending
        }
    }
}
