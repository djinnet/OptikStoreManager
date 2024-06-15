using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repos;

/// <summary>
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
    public Task<(ECreateStoreResponse response, RetailStore? store)> AddAsync(RetailStore item)
    {
        throw new NotImplementedException();
    }

    public Task<EDeleteStoreResponse> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<RetailStore> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RetailStore>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<(EUpdateStoreResponse response, RetailStore? Updatedstore)> UpdateAsync(Guid id, RetailStore item)
    {
        throw new NotImplementedException();
    }
}
