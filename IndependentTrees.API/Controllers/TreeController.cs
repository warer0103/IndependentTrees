using IndependentTrees.API.DataStorage;
using IndependentTrees.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Controllers
{
    /// <summary>
    /// Represents entire tree API
    /// </summary>
    [Area("api.user")]
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public TreeController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        /// <remarks>
        /// Returns your entire tree. If your tree doesn't exist it will be created automatically.
        /// </remarks>
        [HttpPost("[area].[controller].get")]
        public async Task<ActionResult<Node>> GetOrCreate([Required] string treeName)
        {
            var node = await _dataStorage.GetOrCreateTreeAsync(treeName);
            return Ok(node);
        }
    }
}
