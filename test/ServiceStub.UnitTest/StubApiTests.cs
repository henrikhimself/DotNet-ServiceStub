using Hj.ServiceStub.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Hj.ServiceStub.UnitTest;

public class StubApiTests
{
  [Fact]
  public void GetCollection__Returns()
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    var stubApp = inputBuilder.Instance<StubApp>();

    // act
    var result = StubApi.GetCollection(stubApp);

    // assert
    Assert.Equal(stubApp.CurrentCollection, result?.Value?.Name);
  }

  [Fact]
  public void PostCollection_GivenCollection_UpdatesCollection()
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    var stubApp = inputBuilder.Instance<StubApp>();
    CollectionDto collectionDto = new() { Name = Guid.NewGuid().ToString() };

    // act
    var result = StubApi.PostCollection(stubApp, collectionDto);

    // assert
    Assert.IsType<Ok>(result);
    Assert.Equal(collectionDto.Name, stubApp.CurrentCollection);
  }

#nullable disable
  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void PostCollection_GivenBadCollection_ReturnBadRequest(string badName)
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    var stubApp = inputBuilder.Instance<StubApp>();
    CollectionDto collectionDto = new() { Name = badName };

    // act
    var result = StubApi.PostCollection(stubApp, collectionDto);

    // assert
    Assert.IsType<BadRequest>(result);
  }
#nullable restore

  private static InputBuilder SetHappyPath(InputBuilder inputBuilder)
  {
    inputBuilder.Instance<IOptions<StubOptions>>().Value.Returns(new StubOptions());
    inputBuilder.Instance<StubApp>().CurrentCollection = Guid.NewGuid().ToString();
    return inputBuilder;
  }
}
