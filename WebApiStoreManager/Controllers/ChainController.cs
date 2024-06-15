using Azure;
using Core.Enums;
using Core.Exceptions;
using Core.Models;
using Core.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiStoreManager.Controllers;

/// <summary>
/// This is the API controller for chains that managed the stores.
/// The controller has basic CRUD functions for API endpoints
/// The resource is named RetailChain. 
/// </summary>
[Authorize(Roles = UserRoles.Admin)]
[Route("api/[controller]")]
[ApiController]
public class ChainController : ControllerBase
{
    public ChainController(ILogger<ChainController> logger, IChainRepo repo)
    {
        _logger = logger;
        _repo = repo;
    }

    private readonly ILogger<ChainController> _logger;
    private readonly IChainRepo _repo;

    /// <summary>
    /// Get all chains
    /// </summary>
    /// <returns>Return a list of all chains</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RetailChain>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RetailChain>>> Get()
    {
        try
        {
            IEnumerable<RetailChain> chains = await _repo.GetAsync();
            if(chains == null)
            {
                return NotFound("Chains is not found!");
            }
            return Ok(chains);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Crashed Get Chains");
            return BadRequest("Web api is down!");
        }
        
    }

    /// <summary>
    /// Get a chain by Guid
    /// </summary>
    /// <param name="id">The id of the chain</param>
    /// <returns>Return a chain with the given id</returns>
    [Route("{id:guid}", Name = "Get")]
    [ProducesResponseType(typeof(RetailStore), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            RetailChain? chain = await _repo.GetSingleAsync(id);
            if (chain == null)
            {
                return NotFound("Chain is not found!");
            }
            return Ok(chain);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Crashed Get Chain");
            return BadRequest("Web api is down!");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(RetailChain), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] RetailChain chain)
    {
        try
        {
            (ECreateChainResponse response, RetailChain? chainValue) = await _repo.AddAsync(chain);
            return response switch
            {
                ECreateChainResponse.NotFound => NotFound($"Not found chain"),
                ECreateChainResponse.FailedToCreate => BadRequest("Failed to create chain."),
                ECreateChainResponse.ChainAlreadyExist => BadRequest($"Chain Name {chainValue?.Name} already exists. Please choose another name."),
                _ => CreatedAtRoute("Get", new { chainValue?.Id }, chainValue),
            };
        }
        catch (CreateUnauthorizedException<RetailChain> ex)
        {
            _logger.LogCritical(ex, "Crashed Post Chain");
            return BadRequest("Web api is down!");
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(Guid id, [FromBody] RetailChain chain)
    {
        try
        {
            (EUpdateChainResponse response, RetailChain? Updatedchain) = await _repo.UpdateAsync(id, chain);
            return response switch
            {
                EUpdateChainResponse.NotFound => NotFound($"Not found the chain from id"),
                EUpdateChainResponse.FailedToUpdate => BadRequest("Failed to update chain"),
                EUpdateChainResponse.ChainAlreadyExist => BadRequest($"Chain name {Updatedchain?.Name} already exists. Please choose another name."),
                EUpdateChainResponse.ItemIsNull => NotFound("Cannot find model"),
                EUpdateChainResponse.IDNoMatch => BadRequest("Id doesnt match with body"),
                _ => Ok("The chain has been updated.")
            };
        }
        catch (EditUnauthorizedException<RetailChain> ex)
        {
            _logger.LogCritical(ex, "Crashed Put Chain");
            return BadRequest("Web api is down!");
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            EDeleteChainResponse response = await _repo.DeleteAsync(id);
            return response switch
            {
                EDeleteChainResponse.NotFound => NotFound("The chain cannot be found."),
                EDeleteChainResponse.FailedToDelete => BadRequest("The chain cannot be delete."),
                EDeleteChainResponse.NotAllowedToDelete => BadRequest("The chain is not allowed to be deleted becauase there exist stores in the chain."),
                _ => Ok("The chain is deleted."),
            };
        }
        catch (DeleteUnauthorizedException<RetailChain> ex)
        {
            _logger.LogCritical(ex, "Crashed Delete Chain");
            return BadRequest("Web api is down!");
        }
    }
}
   
