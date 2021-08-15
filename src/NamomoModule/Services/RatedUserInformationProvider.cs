#nullable enable
using Ccs.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SatelliteSite.NamomoModule.Services
{
    public class RatedUserInformationProvider : UserInformationProviderBase<int?>
    {
        private readonly IUserInformationCache<(int?, string)?> _cache;
        private readonly IUserManager _userManager;

        public RatedUserInformationProvider(
            IUrlHelperFactory urlHelperFactory,
            IUserInformationCache<(int?, string)?> cache,
            IUserManager userManager)
            : base(urlHelperFactory)
        {
            _cache = cache;
            _userManager = userManager;
        }

        protected override Task<(int?, string)?> GetUserAsync(int userId, string? userName)
        {
            return _cache.GetByUserIdAsync(
                userId,
                async uid =>
                {
                    var user = await _userManager.FindByIdAsync(uid);
                    if (user == null) return null;
                    return user == null ? default((int?, string)?) : (((IUserWithRating)user).Rating, user.UserName);
                });
        }

        protected override Task<(int?, string)?> GetUserAsync(string userName)
        {
            return _cache.GetByUserNameAsync(
                userName,
                async uname =>
                {
                    var user = await _userManager.FindByNameAsync(uname);
                    if (user == null) return null;
                    return user == null ? default((int?, string)?) : (((IUserWithRating)user).Rating, user.UserName);
                });
        }

        public static string RankClass(int? _rk)
        {
            if (_rk == null) return "rank-unrated";
            int rk = _rk.Value;

            return rk switch
            {
                _ when 3000 <= rk => "rank-legendary-grandmaster",
                _ when 2600 <= rk => "rank-international-grandmaster",
                _ when 2400 <= rk => "rank-grandmaster",
                _ when 2300 <= rk => "rank-international-master",
                _ when 2100 <= rk => "rank-master",
                _ when 1900 <= rk => "rank-candidate-master",
                _ when 1600 <= rk => "rank-expert",
                _ when 1400 <= rk => "rank-specialist",
                _ when 1200 <= rk => "rank-pupil",
                _ when -999 <= rk => "rank-newbie",
                _ => "rank-headquarters"
            };
        }

        public static string RankName(int? _rk)
        {
            if (_rk == null) return "unrated";
            int rk = _rk.Value;

            return rk switch
            {
                _ when 3000 <= rk => "legendary grandmaster",
                _ when 2600 <= rk => "international grandmaster",
                _ when 2400 <= rk => "grandmaster",
                _ when 2300 <= rk => "international master",
                _ when 2100 <= rk => "master",
                _ when 1900 <= rk => "candidate master",
                _ when 1600 <= rk => "expert",
                _ when 1400 <= rk => "specialist",
                _ when 1200 <= rk => "pupil",
                _ when -999 <= rk => "newbie",
                _ => "headquarters"
            };
        }

        protected override Task ProduceAsync(
            int? evermore,
            string username,
            IReadOnlyDictionary<string, string> attach,
            ViewContext actionContext,
            TagHelperContext context,
            TagHelperOutput output)
        {
            output.Attributes.AddClass("rank-show " + RankClass(evermore));
            return base.ProduceAsync(evermore, username, attach, actionContext, context, output);
        }
    }
}
