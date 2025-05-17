using IndependentTrees.API.DataStorage;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Controllers
{
    /// <summary>
    /// Represents tree node API
    /// </summary>
    [Area("api.user.tree")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public NodeController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        /// <remarks>
        /// Create a new node in your tree. You must to specify a parent node ID that belongs to your tree. 
        /// A new node name must be unique across all siblings.
        /// </remarks>
        [HttpPost("[area].[controller].[action]")]
        public async Task<ActionResult> Create(
            [Required] string treeName,
            [Required] int parentNodeId,
            [Required] string nodeName)
        {
            await _dataStorage.CreateNodeAsync(treeName, parentNodeId, nodeName);
            return Ok();
        }

        /// <remarks>
        /// Delete an existing node in your tree. You must specify a node ID that belongs your tree.
        /// </remarks>
        [HttpPost("[area].[controller].[action]")]
        public async Task<ActionResult> Delete(
            [Required] string treeName,
            [Required] int nodeId)
        {
            await _dataStorage.DeleteNodeAsync(treeName, nodeId);
            return Ok();
        }

        /// <remarks>
        /// Rename an existing node in your tree. You must specify a node ID that belongs your tree. 
        /// A new name of the node must be unique across all siblings.
        /// </remarks>
        [HttpPost("[area].[controller].[action]")]
        public async Task<ActionResult> Rename(
            [Required] string treeName,
            [Required] int nodeId,
            [Required] string newNodeName)
        {
            await _dataStorage.RenameNodeAsync(treeName, nodeId, newNodeName);
            return Ok();
        }
    }
}
