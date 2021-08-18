#nullable enable
using Ccs.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
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

        protected override ValueTask ProduceAsync(
            TagBuilder tag,
            int? evermore,
            string? username,
            IReadOnlyDictionary<string, string> attach,
            ViewContext actionContext)
        {
            tag.AddCssClass("rank-show " + RankClass(evermore));
            return base.ProduceAsync(tag, evermore, username, attach, actionContext);
        }

        private static bool ParseExplicitRating(
            IReadOnlyDictionary<string, string> attach,
            out int? rating)
        {
            if (!attach.TryGetValue("explicit-rating", out var ratingStr))
            {
                rating = default;
                return false;
            }
            else if (ratingStr == "null" || ratingStr == "none")
            {
                rating = default;
                return true;
            }
            else if (int.TryParse(ratingStr, out var ratingInt))
            {
                rating = ratingInt;
                return true;
            }
            else
            {
                rating = default;
                return false;
            }
        }

        protected override async ValueTask<(int?, string)?> GetUserAsync(
            int userId,
            string? userName,
            IReadOnlyDictionary<string, string> attach)
        {
            bool hasUserName = userName != null;
            bool hasRating = ParseExplicitRating(attach, out int? firstRating);
            (int?, string)? current = default;

            if (!hasUserName || !hasRating)
            {
                current = await _cache.GetByUserIdAsync(
                    userId,
                    async uid =>
                    {
                        var user = await _userManager.FindByIdAsync(uid);
                        return user == null
                            ? default((int?, string)?)
                            : (((IUserWithRating)user).Rating, user.UserName);
                    });

                if (!current.HasValue) return default;
            }

            string? finalUserName = hasUserName ? userName : current?.Item2;
            int? finalRating = hasRating ? firstRating : current?.Item1;
            return (finalRating, finalUserName!);
        }

        protected override ValueTask<(int?, string)?> GetUserAsync(string userName, IReadOnlyDictionary<string, string> attach)
        {
            if (userName == null)
            {
                return default;
            }
            else if (ParseExplicitRating(attach, out int? firstRating))
            {
                return new ValueTask<(int?, string)?>((firstRating, userName));
            }
            else
            {
                return _cache.GetByUserNameAsync(
                    userName,
                    async uname =>
                    {
                        var user = await _userManager.FindByNameAsync(uname);
                        return user == null
                            ? default((int?, string)?)
                            : (((IUserWithRating)user).Rating, user.UserName);
                    });
            }
        }
    }
}
