using Core.Models;

namespace WebAppStoreManager.Services;
public interface IEndpointsClient
{
    Task<Response<RetailChain>?> CreateChainAsync(RetailChain chain);
    Task<Response<RetailStore>?> CreateStoreAsync(RetailStore store);
    Task<Response<string>?> DeleteChainAsync(Guid id);
    Task<Response<string>?> DeleteStoreAsync(Guid id);
    Task<RetailChain?> GetChainAsync(Guid id);
    Task<List<RetailChain>?> GetChainsAsync();
    Task<RetailStore?> GetStoreAsync(Guid id);
    Task<List<RetailStore>?> GetStoresAsync();
    Task<Response<RetailChain>?> UpdateChainAsync(RetailChain chain);
    Task<Response<RetailStore>?> UpdateStoreAsync(RetailStore store);
}