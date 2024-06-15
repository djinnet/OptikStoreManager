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
/// This is the repository for the RetailChain entity. 
/// The Chain Requirement:
/// - The chain can have many stores.
/// - The chain's name cannot be duplicated in the database (means the name must be unique)
/// - The chain is not allowed to be deleted if the stores exists under the chain.
/// - The chain is allowed to be deleted if it do not have any stores.
/// </summary>
public class ChainRepo : IChainRepo
{
    private ApplicationDbContext _context { get; set; }
    public ChainRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(ECreateChainResponse response, RetailChain? chain)> AddAsync(RetailChain item)
    {
        try
        {
            if (item == null)
            {
                return (ECreateChainResponse.NotFound, null);
            }

            //Check if the number already exists in database with a number value that is provided by an application user
            if (_context.RetailStores.Any(i => string.Equals(i.Name, item.Name)))
            {
                //if the value matched, then it exists. No duplicated number allowed.
                return (ECreateChainResponse.ChainAlreadyExist, item);
            }

            //create an new object with current time
            RetailChain retailChain = new()
            {
                Name = item.Name,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow
            };

            await _context.AddAsync(retailChain);
            await _context.SaveChangesAsync();

            //everything went well.
            return (ECreateChainResponse.SuccessToCreate, retailChain);
        }
        catch (Exception)
        {
            return (ECreateChainResponse.FailedToCreate, null);
        }
    }

    public async Task<EDeleteChainResponse> DeleteAsync(Guid id)
    {
        try
        {
            //Get chain with its stores
            RetailChain? retailChain = await _context.RetailChains.Include(i => i.Stores).SingleOrDefaultAsync(i => i.Id == id);
            if (retailChain == null) 
            {
                return EDeleteChainResponse.NotFound;
            }

            //check if the chain has any existed stores, then returned the error
            if(retailChain.Stores.Count > 0)
            {
                //Not allowed to deleted because stores existed
                return EDeleteChainResponse.NotAllowedToDelete;
            }

            _context.RetailChains.Remove(retailChain);
            await _context.SaveChangesAsync();
            return EDeleteChainResponse.SuccessToDelete;
        }
        catch (Exception)
        {
            return EDeleteChainResponse.FailedToDelete;
        }
    }

    public async Task<RetailChain?> GetSingleAsync(Guid id)
    {
        return await _context.RetailChains.Include(i => i.Stores).SingleOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<RetailChain>> GetAsync()
    {
        try
        {
            return await _context.RetailChains.Include(i => i.Stores).AsNoTracking().ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<(EUpdateChainResponse response, RetailChain? Updatedchain)> UpdateAsync(Guid id, RetailChain item)
    {
        if (item == null)
        {
            return (EUpdateChainResponse.ItemIsNull, null);
        }

        //check for ids
        if (id != item.Id)
        {
            return (EUpdateChainResponse.IDNoMatch, item);
        }

        //ID matched with the item
        RetailChain? FoundretailChain = await _context.RetailChains.Include(i => i.Stores).SingleOrDefaultAsync(i => i.Id == id);
        if (FoundretailChain == null)
        {
            return (EUpdateChainResponse.NotFound, item);
        }

        //Check if the number already exists in database with a number value that is provided by an application user
        if (_context.RetailStores.Any(i => string.Equals(i.Name, item.Name)))
        {
            //if the value matched, then it exists. No duplicated number allowed.
            return (EUpdateChainResponse.ChainAlreadyExist, item);
        }

        FoundretailChain.Name = item.Name;
        FoundretailChain.ModifiedOn = DateTime.UtcNow; //updated to current time

        await _context.SaveChangesAsync();
        return (EUpdateChainResponse.SuccessToUpdate, FoundretailChain);
    }
}
