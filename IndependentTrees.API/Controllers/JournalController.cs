using IndependentTrees.API.DataStorage;
using IndependentTrees.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Controllers
{
    /// <summary>
    /// Represents journal API
    /// </summary>
    [Area("api.user")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public JournalController(IDataStorage dataStorage) 
        {
            _dataStorage = dataStorage;
        }
        
        /// <remarks>
        /// Provides the pagination API. Skip means the number of items should be skipped by server. 
        /// Take means the maximum number items should be returned by server. 
        /// All fields of the filter are optional.        
        /// </remarks>
        [HttpPost("[area].[controller].[action]")]
        public async Task<ActionResult<Range<JournalInfo>>> GetRange(
            [Required] [Range(0, int.MaxValue)] int skip,
            [Required] [Range(1, 100)] int take,
            [Required] [FromBody] JournalFilter filter)
        {
            var journals = await _dataStorage.GetJournalsAsync(skip, take, filter);

            return Ok(journals);
        }

        /// <remarks>
        /// Returns the information about an particular event by ID.
        /// </remarks>
        [HttpPost("[area].[controller].[action]")]
        public async Task<ActionResult<Journal>> GetSingle([Required] int id)
        {
            var journal = await _dataStorage.GetJournalAsync(id);
            return journal == null ? NotFound() : Ok(journal);
        }
    }
}
