using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repos;
public class ChainRepo : IChainRepo
{
    public Task<(ECreateChainResponse response, RetailChain? chain)> AddAsync(RetailChain item)
    {
        throw new NotImplementedException();
    }

    public Task<EDeleteChainResponse> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<RetailChain> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RetailChain>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<(EUpdateChainResponse response, RetailChain? Updatedchain)> UpdateAsync(Guid id, RetailChain item)
    {
        throw new NotImplementedException();
    }
}
