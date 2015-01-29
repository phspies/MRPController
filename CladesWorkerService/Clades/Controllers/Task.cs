using CladesWorkerService.Clades.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.API
{
    class TasksObject : Core
    {
        public TasksObject(Clades _dimensiondata) : base(_dimensiondata) { }
        public TaskListObject tasks()
        {
            endpoint = "/clades/worker/tasklist";
            TaskListObject account = get<TaskListObject>(null, true) as TaskListObject;
            return account;
        }
    }
}


