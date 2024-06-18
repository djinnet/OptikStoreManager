using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Repos;
using Microsoft.AspNetCore.Authorization;
using Core.Models;
using Microsoft.AspNetCore.DataProtection;
using Core.Enums;
using Core.Exceptions;

namespace WebAppStoreManager.Controllers;

//[Authorize(Roles = UserRoles.Admin)]
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly ILogger<StoreController> _logger;
    private readonly IStoreRepo _storeRepo;

    public StoreController(ILogger<StoreController> logger, IStoreRepo repo)
    {
        _logger = logger;
        _storeRepo = repo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RetailStore>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get()
    {
        try
        {
            IEnumerable<RetailStore>? list = await _storeRepo.GetAsync();
            if (list == null)
            {
                return NotFound("List is not found!");
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Crashed Get Stores");
            return BadRequest("Web api is down!");
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RetailStore), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            RetailStore? store = await _storeRepo.GetSingleAsync(id);
            if (store == null)
            {
                return NotFound("The store could not be found.");
            }

            return Ok(store);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Crashed Get Store");
            return BadRequest("Web api is down!");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RetailStore viewModel)
    {
        try
        {
            var apiresponse = new ApiResponse<RetailStore?, ECreateStoreResponse>() { HttpStatus = StatusCodes.Status200OK };
            (ECreateStoreResponse response, RetailStore? store) = await _storeRepo.AddAsync(viewModel);
            apiresponse.Result = store;
            apiresponse.Status = response;

            switch (response)
            {
                case ECreateStoreResponse.ItemIsEmpty:
                    apiresponse.AddModelErrors("Store data is empty");
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    break;
                case ECreateStoreResponse.NotFound:
                    apiresponse.HttpStatus = StatusCodes.Status404NotFound;
                    apiresponse.AddModelErrors($"Not found the chain's id. Store name: {store?.StoreName} | Store chain name: {store?.Chain?.ChainName} ");
                    break;
                case ECreateStoreResponse.NumberAlreadyExist:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors($"Store number {store?.Number} already exists. Please choose another number.");
                    break;
                case ECreateStoreResponse.FailedToCreate:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors("Failed to create chain.");
                    break;
            }

            return Ok(apiresponse);
        }
        catch (CreateUnauthorizedException<RetailStore> ex)
        {
            _logger.LogCritical(ex, "Forbid to create store");
            return Forbid();
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RetailStore model)
    {
        try
        {
            var apiresponse = new ApiResponse<RetailStore?, EUpdateStoreResponse>() { HttpStatus = StatusCodes.Status200OK };
            (EUpdateStoreResponse response, RetailStore? Updatedstore) = await _storeRepo.UpdateAsync(id, model);
            apiresponse.Result = Updatedstore;
            apiresponse.Status = response;

            switch (response)
            {
                case EUpdateStoreResponse.ItemIsNull:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors("Cannot find model");
                    break;
                case EUpdateStoreResponse.NotFound:
                    apiresponse.HttpStatus = StatusCodes.Status404NotFound;
                    apiresponse.AddModelErrors($"Not found the chain's id. Store name: {Updatedstore?.StoreName} | Store chain name: {Updatedstore?.Chain?.ChainName} ");
                    break;
                case EUpdateStoreResponse.NumberAlreadyExist:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors($"Store number {model?.Number} already exists. Please choose another number.");
                    break;
                case EUpdateStoreResponse.IDNoMatch:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors("Id doesnt match with body");
                    break;
                case EUpdateStoreResponse.FailedToUpdate:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors("Failed to update store");
                    break;
            }

            return Ok(apiresponse);
        }
        catch (EditUnauthorizedException<RetailStore> ex)
        {
            _logger.LogCritical(ex, "Forbid to edit store");
            return Forbid();
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            var apiresponse = new ApiResponse<string?, EDeleteStoreResponse>() { HttpStatus = StatusCodes.Status200OK };
            EDeleteStoreResponse response = await _storeRepo.DeleteAsync(id);
            apiresponse.Result = string.Empty;
            apiresponse.Status = response;

            switch (response)
            {
                case EDeleteStoreResponse.NotFound:
                    apiresponse.HttpStatus = StatusCodes.Status404NotFound;
                    apiresponse.AddModelErrors($"The chain cannot be found.");
                    break;
                case EDeleteStoreResponse.FailedToDelete:
                    apiresponse.HttpStatus = StatusCodes.Status400BadRequest;
                    apiresponse.AddModelErrors($"The chain cannot be delete.");
                    break;
            }

            return Ok(apiresponse);
        }
        catch (DeleteUnauthorizedException<RetailStore> ex)
        {
            _logger.LogCritical(ex, "Forbid to delete store");
            return Forbid();
        }
    }
}
