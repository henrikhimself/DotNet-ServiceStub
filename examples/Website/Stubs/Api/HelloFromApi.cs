namespace Hj.Examples.Website.Stubs.Api;

internal static class HelloFromApi
{
  public static async Task GetJsonAsync(HttpContext context, CancellationToken cancellationToken)
  {
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync($$"""{ "Hello": "from API. I can return generated values such as the current time {{DateTime.Now}}" }""", cancellationToken);
  }

  public static async Task GetJsonFromMyOtherCollectionAsync(HttpContext context, CancellationToken cancellationToken)
  {
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync($$"""{ "Hello": "from API in my-other-collection. Here is a GUID {{Guid.NewGuid()}}" }""", cancellationToken);
  }

  public static async Task GetSvgAsync(HttpContext context, CancellationToken cancellationToken)
  {
    context.Response.ContentType = "image/svg+xml";
    await context.Response.WriteAsync(
    """
    <svg height="200" width="350" xmlns="http://www.w3.org/2000/svg">
      <path id="lineAC" d="M 30 180 q 150 -250 300 0" stroke="blue" stroke-width="2" fill="none"/>
      <text style="fill:red;font-size:25px;">
        <textPath href="#lineAC" textLength="100%" startOffset="20">Hello from SVG</textPath>
      </text>
      Sorry, your browser does not support inline SVG.
    </svg>
    """,
    cancellationToken);
  }
}
