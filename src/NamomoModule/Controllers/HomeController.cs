using Blogging.Services;
using Ccs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            [FromServices] IBlogStore blogs)
        {
            ViewBag.ActiveAction = "HomePage";
            ViewBag.CompileVersion = ProgramVersion;
            //ViewBag.Nearest = await cts.FindNearestAsync();
            //ViewBag.Ratings = await usr.ListUserRatingsAsync(10);
            var model = await blogs.ListAsync(null, 5, 0);

            if (User.GetUserId() != null)
            {
                var uuid = int.Parse(User.GetUserId());
                ViewBag.PostsVote = await blogs.StatisticsAsync(model.Select(a => a.Id), uuid);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult About()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Problemsets([FromServices] IContestRepository2 store, int page = 1)
        {
            ViewData["ActiveAction"] = "Problemset";

            return View(
                await store.ListAsync(User, Ccs.CcsDefaults.KindProblemset, page));
        }


        [HttpGet]
        public async Task<IActionResult> Contests([FromServices] IContestRepository2 store, int page = 1)
        {
            ViewData["ActiveAction"] = "ListContest";

            return View(
                await store.ListAsync(User, Ccs.CcsDefaults.KindDom, page));
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
