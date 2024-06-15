using Core.Database;
using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repos;

/// <summary>
/// This is the repository for the RetailStore entity. 
/// The store requirement:
/// - The store number has to be checked if it already exists. No Duplicated number is allowed on create and update function.
/// - The store can be connected to no chain or 1 chain. (In order to get it working on database level, it is an optional one-to-many relationship)
/// - - if the store chainid is null, it means it is not connected to an chain. That means the dropdown for "pick a chain" on frontend is blank.
/// - - if the store chainid is not null, it is connected to an chain. That means the dropdown for "pick a chain" on frontend is displayed the chain's name.
/// - - The store cannot connect to multiple chains, but the chain can connected to multiple stores.
/// The function for connect store to chain is implemented on create and update functions.
/// </summary>
public class StoreRepo : IStoreRepo
{
    private ApplicationDbContext _context {  get; set; }
    public StoreRepo(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<(ECreateStoreResponse response, RetailStore? store)> AddAsync(RetailStore item)
    {
        try
        {
            if(item == null)
            {
                return (ECreateStoreResponse.ItemIsEmpty, null);
            }

            //Check if the number already exists in database with a number value that is provided by an application user
            if(_context.RetailStores.Any(i => i.Number == item.Number))
            {
                //if the value matched, then it exists. No duplicated number allowed.
                return (ECreateStoreResponse.NumberAlreadyExist, item);
            }

            //create an new object with current time
            RetailStore retailStore = new()
            {
                Name = item.Name,
                Number = item.Number,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,             
                //Add chain id if it exists. If item chain id is blank, it is a standalone store. If not, it is connected to an chain
                ChainId = item.ChainId
            };

            await _context.AddAsync(retailStore);
            await _context.SaveChangesAsync();

            //everything went well.
            return (ECreateStoreResponse.SuccessToCreate, retailStore);
        }
        catch (Exception)
        {
            return (ECreateStoreResponse.FailedToCreate, null);
        }
    }

    public async Task<EDeleteStoreResponse> DeleteAsync(Guid id)
    {
        try
        {
            RetailStore? store = await _context.RetailStores.FindAsync(id);
            if (store == null)
            {
                return EDeleteStoreResponse.NotFound;
            }

            _context.RetailStores.Remove(store);
            await _context.SaveChangesAsync();
            return EDeleteStoreResponse.SuccessToDelete;
        }
        catch (Exception)
        {
            return EDeleteStoreResponse.FailedToDelete;
        }
    }

    public async Task<RetailStore?> GetSingleAsync(Guid id)
    {
        return await _context.RetailStores.FindAsync(id);
    }

    public async Task<IEnumerable<RetailStore>?> GetAsync()
    {
        try
        {
            return await _context.RetailStores.AsNoTracking().ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<(EUpdateStoreResponse response, RetailStore? Updatedstore)> UpdateAsync(Guid id, RetailStore item)
    {
        if(item == null)
        {
            return (EUpdateStoreResponse.ItemIsNull, null);
        }

        //check for ids
        if(id != item.Id)
        {
            return (EUpdateStoreResponse.IDNoMatch, item);
        }

        //ID matched with the item
        RetailStore? FoundStore = await _context.RetailStores.FindAsync(id);
        if(FoundStore == null)
        {
            return (EUpdateStoreResponse.NotFound, item);
        }

        //Check if the number already exists in database with a number value that is provided by an application user
        if (_context.RetailStores.Any(i => i.Number == item.Number))
        {
            //if the value matched, then it exists. No duplicated number allowed.
            return (EUpdateStoreResponse.NumberAlreadyExist, FoundStore);
        }

        FoundStore.Name = item.Name;
        FoundStore.Number = item.Number;
        FoundStore.ChainId = item.ChainId;
        FoundStore.ModifiedOn = DateTime.UtcNow; //updated to current time

        await _context.SaveChangesAsync();
        return (EUpdateStoreResponse.SuccessToUpdate,  FoundStore);
    }
}
