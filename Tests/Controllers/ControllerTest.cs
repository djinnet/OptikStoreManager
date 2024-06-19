using Core.Models;
using Core.Repos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Mocks.Database;
using Xunit;

namespace Tests.Controllers;
public class ControllerTest
{
    private readonly Mock<IChainRepo> mockRepository;

    public ControllerTest()
    {
        mockRepository = new Mock<IChainRepo>();
    }

    [Fact]
    public async Task GetData()
    {
        // Arrange
        mockRepository.Setup(u => u.GetAsync()).ReturnsAsync(DBMemory.Chains());

        // Act
        var result = await mockRepository.Object.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<RetailChain>>(result);
        Assert.Equal((DBMemory.Chains()).Count, result.Count());
    }
}
