using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPJobImage : Core
    {
        public MRPJobImage(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRPJobImageListType list()
        {
            endpoint = "/jobimages/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPJobImageListType>(worker);
        }

        public ResultType get_id(string _image_id)
        {
            endpoint = "/jobimages/get_id.json";
            MRPJobImageIDGETType image = new MRPJobImageIDGETType()
            {
                image_id = _image_id
            };
            return post<ResultType>(image);
        }
        public ResultType get_moid(string _image_moid)
        {
            endpoint = "/jobimages/get_id.json";
            MRPJobImageMOIDGETType job = new MRPJobImageMOIDGETType()
            {
                 moid_id = _image_moid
            };
            return post<ResultType>(job);
        }

        public ResultType create(MRPJobImageType _jobimage)
        {
            MRPJobImagesCRUDType job = new MRPJobImagesCRUDType()
            {
                 jobimage = _jobimage
            };

            endpoint = "/jobimages/create.json";
            return post<ResultType>(job);
        }
        public ResultType update(MRPJobImageType _jobimage)
        {
            MRPJobImagesCRUDType job = new MRPJobImagesCRUDType()
            {
                 jobimage = _jobimage
            };

            endpoint = "/jobimages/update.json";
            return put<ResultType>(job);
        }

    }
}


