// <copyright file="StubMiddleware.cs" company="Henrik Jensen">
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

namespace Hj.ServiceStub;

internal sealed class StubMiddleware(
  ILogger<StubMiddleware> logger,
  IFileStore fileStore,
  StubApp stubApp)
{
  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    if (context.Response.HasStarted
      || await HandleUsingApiStubAsync(context)
      || await HandleUsingStaticFileAsync(context))
    {
      return;
    }

    await next(context);
  }

  internal async Task<bool> HandleUsingApiStubAsync(HttpContext context)
  {
    var fn = stubApp.GetApiFunc(context);
    if (fn == null)
    {
      return false;
    }

    logger.LogDebug("Route: '{Url}', using stub API", context.Request.GetDisplayUrl());
    await fn(context, CancellationToken.None);
    return true;
  }

  internal async Task<bool> HandleUsingStaticFileAsync(HttpContext context)
  {
    var filePath = stubApp.GetFilePath(context);
    if (!fileStore.FileExists(filePath))
    {
      return false;
    }

    logger.LogDebug("Route: '{Url}', using stub JSON", context.Request.GetDisplayUrl());
    context.Response.StatusCode = 200;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(await fileStore.ReadAllTextAsync(filePath));
    return true;
  }
}
