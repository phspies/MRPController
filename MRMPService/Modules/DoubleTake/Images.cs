using DoubleTake.Web.Client;
using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MRMPService.MRMPDoubleTake
{
    class Image : Core
    {
        public Image(Doubletake doubletake) : base(doubletake) { }
        public IEnumerable<ImageInfoModel> GetAllImages()
        {
            var api = new ImagesApi(_target_connection);
            var response = api.GetImagesAsync().Result;
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public IEnumerable<ImageInfoModel> GetAllImagesFromSource()
        {
            var api = new ImagesApi(_source_connection);
            var response = api.GetImagesAsync().Result;
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public IEnumerable<ImageInfoModel> GetImagesSource(String _source_hostname)
        {
            var api = new ImagesApi(_target_connection);
            var response = api.GetImagesAsync("sourceName eq " + _source_hostname).Result;
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public IEnumerable<ImageInfoModel> GetImages(String _job_name)
        {
            var api = new ImagesApi(_target_connection);
            var response = api.GetImagesAsync("protectionJobName eq " + _job_name).Result;
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public IEnumerable<ImageInfoModel> GetImages()
        {
            var api = new ImagesApi(_target_connection);
            var response = api.GetImagesAsync().Result;
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public IEnumerable<SnapshotEntryModel> GetSingleSnapshotEntry(Guid id)
        {
            var api = new ImagesApi(_target_connection);
            var response = api.GetImageSnapshotsAsync(id).Result;
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public void DeleteSnapshotEntry(Guid id, Guid snapshot_id)
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse response = api.DeleteImageSnapshotAsync(id, snapshot_id).Result;
            response.EnsureSuccessStatusCode();
            return;
        }
    }
}
