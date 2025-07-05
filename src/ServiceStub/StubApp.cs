// <copyright file="StubApp.cs" company="Henrik Jensen">
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

namespace Hj.ServiceStub;

internal sealed class StubApp(IOptions<StubOptions> appOptions)
{
  /// <summary>
  /// Gets map of routes to API methods.
  /// Key format is "{collection name}/{path segments}/{http method}".
  /// </summary>
  public Dictionary<string, Func<HttpContext, CancellationToken, Task>> ApiRoutes { get; } = new(StringComparer.OrdinalIgnoreCase);

  public string CurrentCollection { get; set; } = StubConstants.DefaultCollection;

  public static string CreateRoute(string collectionName, ICollection<string> pathSegments, HttpMethod httpMethod)
    => CreateRoute(collectionName, string.Join('/', pathSegments), httpMethod);

  public static string CreateRoute(string collectionName, string path, HttpMethod httpMethod)
  {
    path = path.TrimStart('/').TrimEnd('/');
    var route = string.Join('/', collectionName, path, httpMethod.Method).ToLowerInvariant();
    return route;
  }

  public ICollection<string> GetRouteSegments(HttpContext context)
  {
    var fullPath = context.Request.PathBase + context.Request.Path;
    var segments = fullPath.HasValue
      ? fullPath.Value.Split('/', StringSplitOptions.RemoveEmptyEntries)
      : [];
    return [CurrentCollection, .. segments, context.Request.Method.ToLowerInvariant()];
  }

  public string GetRoute(HttpContext context)
    => string.Join('/', [.. GetRouteSegments(context)]);

  public string GetFilePath(HttpContext context)
  {
    var jsonDirectory = Path.GetDirectoryName(appOptions.Value.JsonPath) ?? AppContext.BaseDirectory;
    return Path.Combine([jsonDirectory, .. GetRouteSegments(context)]) + ".json";
  }

  public Func<HttpContext, CancellationToken, Task>? GetApiFunc(HttpContext context)
  {
    var route = GetRoute(context);
    ApiRoutes.TryGetValue(route, out var fn);
    return fn;
  }
}
