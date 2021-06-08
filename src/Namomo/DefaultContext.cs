using Ccs.Entities;
using Ccs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Polygon.Entities;
using Polygon.Models;
using Polygon.Storages;
using SatelliteSite.IdentityModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenant.Entities;

namespace SatelliteSite
{
    public class DefaultContext :
        IdentityDbContext<NamomoUser, Role, int>,
        IPolygonDbContext,
        IContestDbContext
    {
        public DefaultContext(DbContextOptions<DefaultContext> options)
            : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Executable> Executables { get; set; }
        public DbSet<InternalError> InternalErrors { get; set; }
        public DbSet<Judgehost> Judgehosts { get; set; }
        public DbSet<Judging> Judgings { get; set; }
        public DbSet<JudgingRun> JudgingRuns { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Rejudging> Rejudgings { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<SubmissionStatistics> SubmissionStatistics { get; set; }
        public DbSet<Testcase> Testcases { get; set; }
        public DbSet<ProblemAuthor> ProblemAuthors { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestProblem> ContestProblems { get; set; }
        public DbSet<Jury> ContestJuries { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> TeamMembers { get; set; }
        public DbSet<Clarification> Clarifications { get; set; }
        public DbSet<Balloon> Balloons { get; set; }
        public DbSet<Event> ContestEvents { get; set; }
        public DbSet<Printing> Printings { get; set; }
        public DbSet<RankCache> RankCache { get; set; }
        public DbSet<ScoreCache> ScoreCache { get; set; }
        public DbSet<Visibility> ContestTenants { get; set; }

        IQueryable<Affiliation> IContestDbContext.Affiliations => Set<Affiliation>();
        IQueryable<Category> IContestDbContext.Categories => Set<Category>();
        IQueryable<Submission> IContestDbContext.Submissions => Submissions;
        IQueryable<SubmissionStatistics> IContestDbContext.SubmissionStatistics => SubmissionStatistics;
        IQueryable<Problem> IContestDbContext.Problems => Problems;
        IQueryable<ProblemAuthor> IContestDbContext.ProblemAuthors => ProblemAuthors;
        IQueryable<Judging> IContestDbContext.Judgings => Judgings;
        IQueryable<JudgingRun> IContestDbContext.JudgingRuns => JudgingRuns;
        IQueryable<Testcase> IContestDbContext.Testcases => Testcases;
        IQueryable<IUser> IContestDbContext.Users => Users;
    }

    public class QueryCache : QueryCacheBase<DefaultContext>
    {
        public override async Task<IEnumerable<(int UserId, string UserName, AuthorLevel Level)>> FetchPermittedUserAsync(DefaultContext context, int probid)
        {
            var query =
                from s in context.ProblemAuthors
                where s.ProblemId == probid
                join u in context.Users on s.UserId equals u.Id
                select new { u.Id, u.UserName, s.Level };
            return (await query.ToListAsync()).Select(a => (a.Id, a.UserName, a.Level));
        }

        public override Task<List<SolutionAuthor>> FetchSolutionAuthorAsync(DefaultContext context, Expression<Func<Submission, bool>> predicate)
        {
            var query =
                from s in context.Submissions.WhereIf(predicate != null, predicate)
                join u in context.Users on new { s.ContestId, s.TeamId } equals new { ContestId = 0, TeamId = u.Id }
                into uu from u in uu.DefaultIfEmpty()
                join t in context.Teams on new { s.ContestId, s.TeamId } equals new { t.ContestId, t.TeamId }
                into tt from t in tt.DefaultIfEmpty()
                select new SolutionAuthor(s.Id, s.ContestId, s.TeamId, u.UserName, t.TeamName);
            return query.ToListAsync();
        }

        protected override Expression<Func<DateTimeOffset, DateTimeOffset, double>> CalculateDuration { get; }
            = (start, end) => (end - start).TotalSeconds;
    }
}
