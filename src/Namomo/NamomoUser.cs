using Ccs.Entities;
using SatelliteSite.IdentityModule.Entities;

namespace SatelliteSite
{
    public class NamomoUser : User, IUserWithRating
    {
        public int? Rating { get; set; }
    }
}
