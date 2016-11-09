﻿using DoubleTake.Web.Client;
using MRMPService.MRMPAPI.Contracts;
using System;

namespace MRMPService.MRMPDoubleTake
{
    class Doubletake : IDisposable
    {
        public MRPWorkloadType _source_workload, _target_workload;
        public Doubletake(MRPWorkloadType source_workload, MRPWorkloadType target_workload)
        {
            _source_workload = source_workload;
            _target_workload = target_workload;
        }

        public Job job()
        {
            return new Job(this);
        }
        public Workload workload()
        {
            return new Workload(this);
        }
        public Image image()
        {
            return new Image(this);
        }
        public Management management()
        {
            return new Management(this);
        }
        public Event events()
        {
            return new Event(this);
        }
        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
        }
    }
}
