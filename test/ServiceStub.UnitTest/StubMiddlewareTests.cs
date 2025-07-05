using Hj.ServiceStub.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Hj.ServiceStub.UnitTest;

public class StubMiddlewareTests
{
  [Fact]
  public async Task HandleUsingStubApiAsync_GivenMissingApiStub_ReturnsFalseAsync()
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    inputBuilder.Instance<StubApp>().ApiRoutes.Clear();
    var httpContext = inputBuilder.Instance<DefaultHttpContext>();

    var sut = sutBuilder.CreateSut<StubMiddleware>();

    // act
    var result = await sut.HandleUsingApiStubAsync(httpContext);

    // assert
    Assert.False(result);
  }

  [Fact]
  public async Task HandleUsingStubApiAsync_GivenApiStub_ReturnsTrueAsync()
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    var httpContext = inputBuilder.Instance<DefaultHttpContext>();

    var sut = sutBuilder.CreateSut<StubMiddleware>();

    // act
    var result = await sut.HandleUsingApiStubAsync(httpContext);

    // assert
    Assert.True(result);
  }

  [Fact]
  public async Task HandleUsingStaticFileAsync_GivenMissingFile_ReturnsFalseAsync()
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    inputBuilder.Instance<IFileStore>().FileExists(Arg.Any<string>()).Returns(false);
    var httpContext = inputBuilder.Instance<DefaultHttpContext>();

    var sut = sutBuilder.CreateSut<StubMiddleware>();

    // act
    var result = await sut.HandleUsingStaticFileAsync(httpContext);

    // assert
    Assert.False(result);
  }

  [Fact]
  public async Task HandleUsingStaticFileAsync_GivenFile_ReturnsTrueAsync()
  {
    // arrange
    SutBuilder sutBuilder = new();
    var inputBuilder = SetHappyPath(sutBuilder.InputBuilder);

    var httpContext = inputBuilder.Instance<DefaultHttpContext>();

    var sut = sutBuilder.CreateSut<StubMiddleware>();

    // act
    var result = await sut.HandleUsingStaticFileAsync(httpContext);

    // assert
    Assert.True(result);
  }

  private static InputBuilder SetHappyPath(InputBuilder inputBuilder)
  {
    var httpContext = inputBuilder.Instance<DefaultHttpContext>();
    var httpRequest = httpContext.Request;
    httpRequest.Method = HttpMethod.Get.ToString();
    httpRequest.PathBase = "/y";
    httpRequest.Path = "/a/b";

    var stubOptions = inputBuilder.Instance<StubOptions>();
    inputBuilder.Instance<IOptions<StubOptions>>().Value.Returns(stubOptions);

    var stubApp = inputBuilder.Instance<StubApp>();
    stubApp.ApiRoutes.Add("x/y/a/b/get", (ctx, ct) => Task.CompletedTask);
    stubApp.CurrentCollection = "x";

    inputBuilder.Instance<IFileStore>().FileExists(Arg.Any<string>()).Returns(true);

    return inputBuilder;
  }
}
