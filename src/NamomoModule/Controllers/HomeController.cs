using Blogging.Services;
using Ccs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SatelliteSite.NamomoModule.Controllers
{
    [Area("Xylab")]
    [SupportStatusCodePage]
    [Route("/[action]")]
    public class HomeController : ViewControllerBase
    {
        public static string ProgramVersion { get; }
            = typeof(HomeController).Assembly
                .GetCustomAttribute<GitVersionAttribute>()?
                .Version?.Substring(0, 7) ?? "unknown";


        [HttpGet]
        [HttpGet("/")]
        public async Task<IActionResult> Index(
            [FromServices] IContestRepository2 cts,
            [FromServices] IBloggingFacade blogs,
            [FromServices] IMemoryCache memoryCache)
        {
            ViewBag.ActiveAction = "HomePage";
            ViewBag.CompileVersion = ProgramVersion;
            //ViewBag.Nearest = await cts.FindNearestAsync();
            var model = await blogs.Blogs.ListAsync(null, 3, 0);

            if (User.GetUserId() != null)
            {
                var uuid = int.Parse(User.GetUserId());
                ViewBag.PostsVote = await blogs.Blogs.StatisticsAsync(model.Select(a => a.Id), uuid);
            }

            ViewBag.Ratings = await memoryCache.GetOrCreateAsync("TopRated", async entry =>
            {
                var updater = HttpContext.RequestServices.GetRequiredService<IRatingUpdater>();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await updater.GetRatedUsersAsync(1, 10);
            });

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Ratings([FromServices] IRatingUpdater usr, int page = 1)
        {
            if (page <= 0) return NotFound();
            ViewBag.Page = page;
            return View(await usr.GetRatedUsersAsync(page, 100));
        }


        [HttpGet]
        public IActionResult About()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Contests([FromServices] IContestRepository2 store, int page = 1)
        {
            ViewData["ActiveAction"] = "ListContest";
            return View(await store.ListAsync(User, Ccs.CcsDefaults.KindDom, page));
        }


        [HttpGet]
        public async Task<IActionResult> Gyms([FromServices] IContestRepository2 store, int page = 1)
        {
            ViewData["ActiveAction"] = "ListGym";

            return View(
                await store.ListAsync(User, Ccs.CcsDefaults.KindGym, page));
        }
    }
}
