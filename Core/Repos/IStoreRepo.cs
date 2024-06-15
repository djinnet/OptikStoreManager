using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repos;
public interface IStoreRepo
{
    Task<RetailStore?> GetSingleAsync(Guid id);
    Task<IEnumerable<RetailStore>?> GetAsync();
    Task<(ECreateStoreResponse response, RetailStore? store)> AddAsync(RetailStore item);
    Task<(EUpdateStoreResponse response, RetailStore? Updatedstore)> UpdateAsync(Guid id, RetailStore item);
    Task<EDeleteStoreResponse> DeleteAsync(Guid id);

}
