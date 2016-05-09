﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DD.CBU.Compute.Api.Client.Interfaces;
using DD.CBU.Compute.Api.Client.Interfaces.Network20;
using DD.CBU.Compute.Api.Contracts.General;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Requests.Network20;

namespace DD.CBU.Compute.Api.Client.Network20
{
    /// <summary>
    /// The VIP Node Management type.
    /// </summary>
    public class VipNodeAccessor : IVipNodeAccessor
    {
        /// <summary>
        /// The Web Api.
        /// </summary>
        private readonly IWebApi _api;

        /// <summary>
        /// Initializes a new instance of <see cref="VipNodeAccessor"/>
        /// </summary>
        /// <param name="api">The Web Api</param>
        public VipNodeAccessor(IWebApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Creates a Node on a Network Domain in an MCP 2.0 data center location.
        /// </summary>
        /// <param name="node">The create node.</param>
        /// <returns>The async task of <see cref="ResponseType"/></returns>
        public async Task<ResponseType> CreateNode(CreateNodeType node)
        {
            return await _api.PostAsync<CreateNodeType, ResponseType>(ApiUris.AddVipNode(_api.OrganizationId), node);
        }

        /// <summary>
        /// Retrieves all of the Nodes on a particular Network Domain at an MCP 2.0 data center.
        /// </summary>
        /// <param name="options">The filter options</param>
        /// <returns>The async task of collection of <see cref="NodeType"/></returns>
        public async Task<IEnumerable<NodeType>> GetNodes(NodeListOptions options = null)
        {
            var response = await GetNodesPaginated(options, null);
            return response.items;
        }

        /// <summary>
        /// Retrieves all of the Nodes on a particular Network Domain at an MCP 2.0 data center.
        /// </summary>
        /// <param name="options">The filter options</param>
        /// <param name="pagingOptions"> The paging Options.</param>
        /// <returns>The async task of <see cref="PagedResponse{NodeType}"/></returns>
        public async Task<PagedResponse<NodeType>> GetNodesPaginated(NodeListOptions options = null, PageableRequest pagingOptions = null)
        {
            var response = await _api.GetAsync<nodes>(ApiUris.GetVipNodes(_api.OrganizationId), pagingOptions, options);
            return new PagedResponse<NodeType>
            {
                items = response.node,
                totalCount = response.totalCountSpecified ? response.totalCount : (int?)null,
                pageCount = response.pageCountSpecified ? response.pageCount : (int?)null,
                pageNumber = response.pageNumberSpecified ? response.pageNumber : (int?)null,
                pageSize = response.pageSizeSpecified ? response.pageSize : (int?)null
            };
        }

        /// <summary>
        /// Returns details of a single Node.
        /// </summary>
        /// <param name="nodeId">The Node id.</param>
        /// <returns>The async task of <see cref="NodeType"/></returns>
        public async Task<NodeType> GetNode(Guid nodeId)
        {
            return await _api.GetAsync<NodeType>(ApiUris.GetVipNode(_api.OrganizationId, nodeId));
        }

        /// <summary>
        /// Updates the mutable properties of a Node.
        /// </summary>
        /// <param name="node">The edit node.</param>
        /// <returns>The async task of <see cref="ResponseType"/></returns>
        public async Task<ResponseType> EditNode(EditNodeType node)
        {
            return await _api.PostAsync<EditNodeType, ResponseType>(ApiUris.EditVipNode(_api.OrganizationId), node);
        }

        /// <summary>
        /// Deletes a Node.
        /// </summary>
        /// <param name="nodeId">The Node id.</param>
        /// <returns>The async task of <see cref="ResponseType"/></returns>
        public async Task<ResponseType> DeleteNode(Guid nodeId)
        {
            return
                await
                    _api.PostAsync<DeleteNodeType, ResponseType>(ApiUris.DeleteVipNode(_api.OrganizationId),
                        new DeleteNodeType { id = nodeId.ToString() });
        }
    }
}