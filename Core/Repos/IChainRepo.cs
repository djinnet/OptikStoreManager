using Core.Enums;
using Core.Models;

namespace Core.Repos;

public interface IChainRepo
{
    Task<RetailChain> GetAsync(Guid id);
    Task<IEnumerable<RetailChain>> GetAsync();
    Task<(ECreateChainResponse response, RetailChain? chain)> AddAsync(RetailChain item);
    Task<(EUpdateChainResponse response, RetailChain? Updatedchain)> UpdateAsync(Guid id, RetailChain item);
    Task<EDeleteChainResponse> DeleteAsync(Guid id);
}