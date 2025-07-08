using Microsoft.AspNetCore.Mvc;

namespace Hj.Examples.Website.Controllers;

public sealed class HomeController : Controller
{
  public IActionResult Index() => View();
}
