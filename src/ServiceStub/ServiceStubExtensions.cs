// <copyright file="ServiceStubExtensions.cs" company="Henrik Jensen">
// Copyright 2025 Henrik Jensen
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using Hj.ServiceStub.Abstraction;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hj.ServiceStub;

public static class ServiceStubExtensions
{
  /// <summary>
  /// Configures the services required for the Stub feature.
  /// </summary>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the Stub services will be added.</param>
  /// <param name="configuration">The <see cref="IConfiguration"/> containing the settings for the Stub feature.</param>
  /// <param name="configureOptions">Registers an action used to configure the service stub.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> with the Stub services configured.</returns>
  public static IServiceCollection ConfigureStub(this IServiceCollection services, IConfiguration configuration, Action<StubOptions>? configureOptions = null)
  {
    if (configureOptions is null)
    {
      services.Configure<StubOptions>(configuration.GetSection("Stub"));
    }
    else
    {
      services.Configure(configureOptions);
    }

    services.TryAddSingleton<IFileStore, FileSystemStore>();
    services
      .AddSingleton<StubApp>()
      .AddSingleton<StubMiddleware>();
    return services;
  }

  /// <summary>
  /// Adds the <see cref="StubMiddleware"/> to the application's request pipeline.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
  /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
  public static WebApplication UseStub(this WebApplication app)
  {
    var stubMiddleware = app.Services.GetRequiredService<StubMiddleware>();
    app.Use(stubMiddleware.InvokeAsync);
    return app;
  }

  /// <summary>
  /// Adds an API for managing the stub configuration at runtime.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
  /// <param name="routePrefix">The pattern that prefixes all routes in this group.</param>
  /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
  public static WebApplication UseStubApi(this WebApplication app, [StringSyntax("Route")] string? routePrefix = null)
  {
    StubApi.MapApi(app, routePrefix);
    return app;
  }

  /// <summary>
  /// Maps a stub endpoint to the specified path and HTTP method, executing the provided handler function when the
  /// endpoint is invoked.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
  /// <param name="path">The relative URL path where the stub endpoint will be accessible.</param>
  /// <param name="httpMethod">The HTTP method that the stub endpoint will respond to.</param>
  /// <param name="fn">A delegate that defines the logic to execute when the stub endpoint is invoked.  The delegate receives the current
  /// <see cref="HttpContext"/> and a <see cref="CancellationToken"/> for handling the request.</param>
  /// <returns>The <see cref="WebApplication"/> instance, allowing for method chaining.</returns>
  public static WebApplication MapStub(this WebApplication app, string path, HttpMethod httpMethod, Func<HttpContext, CancellationToken, Task> fn)
    => MapStub(app, path, httpMethod, StubConstants.DefaultCollection, fn);

  /// <summary>
  /// Maps a stub endpoint to the specified path and HTTP method, executing the provided handler function when the
  /// endpoint is invoked.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
  /// <param name="path">The relative URL path where the stub endpoint will be accessible.</param>
  /// <param name="httpMethod">The HTTP method that the stub endpoint will respond to.</param>
  /// <param name="collectionName">The stub collection that must be active before the mapping is used.</param>
  /// <param name="fn">A delegate that defines the logic to execute when the stub endpoint is invoked.  The delegate receives the current
  /// <see cref="HttpContext"/> and a <see cref="CancellationToken"/> for handling the request.</param>
  /// <returns>The <see cref="WebApplication"/> instance, allowing for method chaining.</returns>
  public static WebApplication MapStub(this WebApplication app, string path, HttpMethod httpMethod, string collectionName, Func<HttpContext, CancellationToken, Task> fn)
  {
    var route = StubApp.CreateRoute(collectionName, path, httpMethod);
    var stubApp = app.Services.GetRequiredService<StubApp>();
    stubApp.ApiRoutes.Add(route, fn);
    return app;
  }
}
