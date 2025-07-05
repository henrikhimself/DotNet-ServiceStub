using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Hj.ServiceStub.UnitTest;

public class StubAppTests
{
  [Fact]
  public void CreateRoute_GivenPathSegments_Returns()
  {
    // act
    var result = StubApp.CreateRoute("a", ["b", "c"], HttpMethod.Get);

    // assert
    Assert.Equal("a/b/c/get", result);
  }

  [Fact]
  public void CreateRoute_GivenPath_Returns()
  {
    // act
    var result = StubApp.CreateRoute("a", "/b/c/", HttpMethod.Get);

    // assert
    Assert.Equal("a/b/c/get", result);
  }

  [Fact]
  public void GetRoute_GivenHttpContext_Returns()
  {
    // arrange
    DefaultHttpContext? httpContext = null;

    var sut = SystemUnderTest.For<StubApp>(arrange =>
    {
      SetHappyPath(arrange);
      httpContext = arrange.Instance<DefaultHttpContext>();
    });

    sut.CurrentCollection = "x";

    // act
    var result = sut.GetRoute(httpContext!);

    // assert
    Assert.Equal("x/y/a/b/get", result);
  }

  [Fact]
  public void GetFilePath_GivenHttpContext_Returns()
  {
    // arrange
    DefaultHttpContext? httpContext = null;

    var sut = SystemUnderTest.For<StubApp>(arrange =>
    {
      SetHappyPath(arrange);
      httpContext = arrange.Instance<DefaultHttpContext>();
      arrange.Instance<StubOptions>().JsonPath = "/home/bourne/json/";
    });

    sut.CurrentCollection = "x";

    // act
    var result = sut.GetFilePath(httpContext!);

    // assert
    Assert.Equal("\\home\\bourne\\json\\x\\y\\a\\b\\get.json", result);
  }

  [Fact]
  public void GetApiFunc_GivenHttpContext_Returns()
  {
    // arrange
    DefaultHttpContext? httpContext = null;

    var sut = SystemUnderTest.For<StubApp>(arrange =>
    {
      SetHappyPath(arrange);
      httpContext = arrange.Instance<DefaultHttpContext>();
    });

    sut.ApiRoutes.Add("x/y/a/b/get", (ctx, ct) => Task.CompletedTask);
    sut.CurrentCollection = "x";

    // act
    var result = sut.GetApiFunc(httpContext!);

    // assert
    Assert.NotNull(result);
  }

  private static void SetHappyPath(InputBuilder inputBuilder)
  {
    var httpContext = inputBuilder.Instance<DefaultHttpContext>();
    var httpRequest = httpContext.Request;
    httpRequest.Method = HttpMethod.Get.ToString();
    httpRequest.PathBase = "/y";
    httpRequest.Path = "/a/b";

    var stubOptions = inputBuilder.Instance<StubOptions>();
    inputBuilder.Instance<IOptions<StubOptions>>().Value.Returns(stubOptions);
  }
}
