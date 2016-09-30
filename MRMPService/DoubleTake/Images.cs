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

        public async Task<IEnumerable<ImageInfoModel>> GetAllImages()
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse<IEnumerable<ImageInfoModel>> response = await api.GetImagesAsync();
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<IEnumerable<ImageInfoModel>> GetAllImagesFromSource()
        {
            var api = new ImagesApi(_source_connection);
            ApiResponse<IEnumerable<ImageInfoModel>> response = await api.GetImagesAsync();
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<IEnumerable<ImageInfoModel>> GetImagesSource(String _source_hostname)
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse<IEnumerable<ImageInfoModel>> response = await api.GetImagesAsync("sourceName eq " + _source_hostname);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<IEnumerable<ImageInfoModel>> GetImages(String _job_name)
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse<IEnumerable<ImageInfoModel>> response = await api.GetImagesAsync("protectionJobName eq " + _job_name);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<IEnumerable<ImageInfoModel>> GetImages()
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse<IEnumerable<ImageInfoModel>> response = await api.GetImagesAsync();
            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        public async Task<IEnumerable<SnapshotEntryModel>> GetSingleSnapshotEntry(Guid id)
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse<IEnumerable<SnapshotEntryModel>> response = await api.GetImageSnapshotsAsync(id);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }

        public async void DeleteSnapshotEntry(Guid id, Guid snapshot_id)
        {
            var api = new ImagesApi(_target_connection);
            ApiResponse response = await api.DeleteImageSnapshotAsync(id, snapshot_id);
            response.EnsureSuccessStatusCode();
            return;
        }
    }
}
