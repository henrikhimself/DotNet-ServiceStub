# Service Stub
A Service Stub intercepts HTTP requests and serves local service stubs for testing purposes, eliminating the need for external dependencies.

It supports mapping static JSON files and “Minimal API-like” methods to specific endpoint paths and HTTP methods.

Named collections of mappings are supported, making it easy to switch between sets of responses and enabling flexible testing scenarios.

## Static JSON
A top-level folder is used to store static JSON files. The path to this folder can be configured in appsettings.json or programmatically.

Within this folder, you can create any number of collection folders, each with an arbitrary name. Inside each collection folder, the folder structure should mirror the path segments of your API endpoints - each subfolder represents a path segment. For every endpoint, place static JSON files named after the corresponding HTTP method (such as get.json, post.json, etc.) inside the subfolder representing the last path segment.

## Minimal API-like Methods
Endpoint paths can be mapped to static methods in a Minimal API-like fashion. This allows responses to return dynamic values.

A collection name can optionally be specified with each app.MapStub call in Program.cs.

## API
The Service Stub API can be used to change the active collection at runtime.

To enable this, add app.UseStubApi(); in Program.cs, and refer to the included Stub.http file (see the example project) for instructions on how to call the API.

## Collections
The default collection is named _default.

# Example
See the included website project for a working example.