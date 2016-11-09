using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class PortalPlatform : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PortalPlatform()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public PortalPlatform(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformListType list()
        {
            endpoint = "/platforms/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPlatformListType)post<MRPPlatformListType>(worker);
        }
        public MRPPlatformListType list_paged_filtered(MRPPlatformFilterPagedType _paged_filter_settings)
        {
            endpoint = "/platforms/list_paged_filtered.json";
            return post<MRPPlatformListType>(_paged_filter_settings);
        }

        public MRPPlatformType get_by_id(string _platform_id)
        {
            endpoint = "/platforms/get.json";
            MRPPlatformGETType worker = new MRPPlatformGETType()
            {
                platform_id = _platform_id
            };
            return post<MRPPlatformType>(worker);
        }

        public MRPPlatformType create(MRPPlatformType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/platforms/create.json";
            return (MRPPlatformType)post<MRPPlatformType>(platform);
        }
        public MRPPlatformType update(MRPPlatformType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/platforms/update.json";
            return (MRPPlatformType)put<MRPPlatformType>(platform);
        }
    }
}


