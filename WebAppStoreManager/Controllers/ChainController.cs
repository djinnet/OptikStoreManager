using Azure;
using Core.Enums;
using Core.Exceptions;
using Core.Models;
using Core.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace WebAppStoreManager.Controllers;

/// <summary>
/// This is the API controller for chains that managed the stores.
/// The controller has basic CRUD functions for API endpoints
/// The resource is named RetailChain. 
/// </summary>
//[Authorize(Roles = UserRoles.Admin)]
[AllowAnonymous]
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
    [HttpGet("{id:guid}")]
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
    public async Task<IActionResult> Post([FromBody] RetailChain chain)
    {
        try
        {
            var apiresponse = new ApiResponse<RetailChain?, ECreateChainResponse>() { HttpStatus = StatusCodes.Status200OK };
            //Uncomment for now. Will be enabled in next version
            //if (!ModelState.IsValid)
            //{
            //    apiresponse.AddModelErrors(ModelState);
            //    return BadRequest(apiresponse);
            //}
            (ECreateChainResponse response, RetailChain? chainValue) = await _repo.AddAsync(chain);
            apiresponse.Result = chainValue;
            apiresponse.Status = response;
            switch (response)
            {
                case ECreateChainResponse.NameIsEmpty:
                    apiresponse.AddModelErrors("chain name is empty");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
                case ECreateChainResponse.NotFound:
                    apiresponse.AddModelErrors("Not found chain");
                    apiresponse.HttpStatus = StatusCodes.Status404NotFound;                    
                    break;
                case ECreateChainResponse.ChainAlreadyExist:
                    apiresponse.AddModelErrors($"Chain Name {chainValue?.ChainName} already exists. Please choose another name.");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
                case ECreateChainResponse.FailedToCreate:
                    apiresponse.AddModelErrors("Failed to create chain.");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
            }

            return Ok(apiresponse);
        }
        catch (CreateUnauthorizedException<RetailChain> ex)
        {
            _logger.LogCritical(ex, "Crashed Post Chain");
            return BadRequest("Web api is down!");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] RetailChain chain)
    {
        try
        {
            var apiresponse = new ApiResponse<RetailChain?, EUpdateChainResponse>() { HttpStatus = StatusCodes.Status200OK };
            //Uncomment for now. Will be enabled in next version
            //if (!ModelState.IsValid)
            //{
            //    apiresponse.AddModelErrors(ModelState);
            //    return BadRequest(apiresponse);
            //}
            (EUpdateChainResponse response, RetailChain? Updatedchain) = await _repo.UpdateAsync(id, chain);
            apiresponse.Result = Updatedchain;
            apiresponse.Status = response;

            switch (response)
            {
                case EUpdateChainResponse.NotFound:
                    apiresponse.AddModelErrors($"Not found the chain from id");
                    apiresponse.HttpStatus = StatusCodes.Status404NotFound;                    
                    break;
                case EUpdateChainResponse.ChainAlreadyExist:
                    apiresponse.AddModelErrors($"Chain name {Updatedchain?.ChainName} already exists. Please choose another name.");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
                case EUpdateChainResponse.FailedToUpdate:
                    apiresponse.AddModelErrors("Failed to update chain");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;                    
                    break;
                case EUpdateChainResponse.ItemIsNull:
                    apiresponse.AddModelErrors("Cannot find model");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
                case EUpdateChainResponse.IDNoMatch:
                    apiresponse.AddModelErrors("Id doesnt match with body");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
            }

            return Ok(apiresponse);
        }
        catch (EditUnauthorizedException<RetailChain> ex)
        {
            _logger.LogCritical(ex, "Crashed Put Chain");
            return BadRequest("Web api is down!");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var apiresponse = new ApiResponse<string?, EDeleteChainResponse>() { HttpStatus = StatusCodes.Status200OK };
            EDeleteChainResponse response = await _repo.DeleteAsync(id);
            apiresponse.Result = string.Empty;
            apiresponse.Status = response;

            switch (response)
            {
                case EDeleteChainResponse.NotFound:
                    apiresponse.AddModelErrors($"The chain cannot be found.");
                    apiresponse.HttpStatus = StatusCodes.Status404NotFound;
                    break;
                case EDeleteChainResponse.FailedToDelete:
                    apiresponse.AddModelErrors($"The chain cannot be delete.");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
                case EDeleteChainResponse.NotAllowedToDelete:
                    apiresponse.AddModelErrors("The chain is not allowed to be deleted becauase there exist stores in the chain.");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
            }

            //If no errors found, set to success msg
            if(apiresponse.Errors.Count < 0) {
                apiresponse.Result = "The chain is deleted.";
            }

            return Ok(apiresponse);
        }
        catch (DeleteUnauthorizedException<RetailChain> ex)
        {
            _logger.LogCritical(ex, "Crashed Delete Chain");
            return BadRequest("Web api is down!");
        }
    }
}
   
