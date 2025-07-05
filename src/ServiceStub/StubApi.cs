// <copyright file="StubApi.cs" company="Henrik Jensen">
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

using Hj.ServiceStub.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Hj.ServiceStub;

internal static class StubApi
{
  public static IEndpointRouteBuilder MapApi(this IEndpointRouteBuilder app, string? routePrefix)
  {
    var api = app;
    if (!string.IsNullOrWhiteSpace(routePrefix))
    {
      api = app.MapGroup(routePrefix);
    }

    api.MapGet("/collection", GetCollection)
      .WithName("GetCollection")
      .WithDescription("Get active collection name.");
    api.MapPost("/collection", PostCollection)
      .WithName("SetCollection")
      .WithDescription("Set active collection.");

    return app;
  }

  public static JsonHttpResult<CollectionDto> GetCollection([FromServices] StubApp stubApp)
  {
    return TypedResults.Json<CollectionDto>(new()
    {
      Name = stubApp.CurrentCollection,
    });
  }

  public static IResult PostCollection([FromServices] StubApp stubApp, [FromBody] CollectionDto collection)
  {
    if (string.IsNullOrWhiteSpace(collection.Name))
    {
      return Results.BadRequest();
    }

    stubApp.CurrentCollection = collection.Name;
    return Results.Ok();
  }
}
