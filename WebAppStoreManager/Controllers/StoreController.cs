using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Repos;
using Microsoft.AspNetCore.Authorization;
using Core.Models;
using Microsoft.AspNetCore.DataProtection;
using Core.Enums;
using Core.Exceptions;

namespace WebAppStoreManager.Controllers;

[Authorize(Roles = UserRoles.Admin)]
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

    [HttpGet, Route("[controller]/GetSingle")]
    [ProducesResponseType(typeof(RetailStore), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSingle([FromRoute] Guid id)
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
    [ProducesResponseType(typeof(RetailStore), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] RetailStore viewModel)
    {
        try
        {
            (ECreateStoreResponse response, RetailStore? store) = await _storeRepo.AddAsync(viewModel);

            return response switch
            {
                //If the chain is not found when we are added the store to the chain. Only happened if we are looking for the chain (if the store have an chain id), and it cant be found for some unknown reasons.
                ECreateStoreResponse.NotFound => NotFound($"Not found the chain's id. Store name: {store?.Name} | Store chain name: {store?.Chain?.Name} "),
                ECreateStoreResponse.FailedToCreate => BadRequest("Failed to create store"),
                ECreateStoreResponse.NumberAlreadyExist => BadRequest($"Store number {store?.Number} already exists. Please choose another number."),
                ECreateStoreResponse.ItemIsEmpty => BadRequest("Store data is empty"),
                _ => CreatedAtRoute("Get", new { store?.Id }, store),
            };
        }
        catch (CreateUnauthorizedException<RetailStore> ex)
        {
            _logger.LogCritical(ex, "Forbid to create store");
            return Forbid();
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] RetailStore model)
    {
        try
        {
            (EUpdateStoreResponse response, RetailStore? Updatedstore) = await _storeRepo.UpdateAsync(id, model);

            return response switch
            {
                //If the chain is not found when we are added the store to the chain. Only happened if we are looking for the chain (if the store have an chain id), and it cant be found for some unknown reasons.
                EUpdateStoreResponse.NotFound => NotFound($"Not found the chain's id. Store name: {Updatedstore?.Name} | Store chain name: {Updatedstore?.Chain?.Name} "),
                EUpdateStoreResponse.FailedToUpdate => BadRequest("Failed to update store"),
                EUpdateStoreResponse.NumberAlreadyExist => BadRequest($"Store number {Updatedstore?.Number} already exists. Please choose another number."),
                EUpdateStoreResponse.ItemIsNull => NotFound("Cannot find model"),
                EUpdateStoreResponse.IDNoMatch => BadRequest("Id doesnt match with body"),
                _ => Ok("The store has been updated.")
            };
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
            EDeleteStoreResponse response = await _storeRepo.DeleteAsync(id);
            return response switch
            {
                EDeleteStoreResponse.NotFound => NotFound("The store cannot be found."),
                EDeleteStoreResponse.FailedToDelete => BadRequest("The store cannot be delete."),
                _ => Ok("The store has been deleted."),
            };
        }
        catch (DeleteUnauthorizedException<RetailStore> ex)
        {
            _logger.LogCritical(ex, "Forbid to delete store");
            return Forbid();
        }
    }
}
