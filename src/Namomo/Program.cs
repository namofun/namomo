using Markdig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SatelliteSite.IdentityModule.Entities;
using System.IO;

namespace SatelliteSite
{
    public class Program
    {
        public static IHost Current { get; private set; }

        public static void Main(string[] args)
        {
            Current = CreateHostBuilder(args).Build();
            Current.AutoMigrate<DefaultContext>();
            Current.MigratePolygonV1();
            Current.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .MarkDomain<Program>()
                .AddModule<TelemetryModule.TelemetryModule>()
                .AddModule<IdentityModule.IdentityModule<NamomoUser, Role, DefaultContext>>()
                .AddModule<NamomoModule.NamomoModule>()
                .AddModule<GroupModule.GroupModule<DefaultContext>>()
                .AddModule<BloggingModule.BloggingModule<NamomoUser, DefaultContext>>()
                .AddModule<PolygonModule.PolygonModule<Polygon.DefaultRole<DefaultContext, QueryCache>>>()
                .AddModule<ContestModule.ContestModule<Ccs.RelationalRole<NamomoUser, Role, DefaultContext>>>()
                .AddModule<PlagModule.PlagModule<Plag.Backend.RestfulBackendRole>>()
                .AddDatabase<DefaultContext>((c, b) => b.UseNpgsql(c.GetConnectionString("DbConnection"), b => b.UseBulk()))
                .ConfigureSubstrateDefaults<DefaultContext>()
                .ConfigureServices((context, services) =>
                {
                    services.AddMarkdown();

                    services.AddDbModelSupplier<DefaultContext, Polygon.Storages.PolygonIdentityEntityConfiguration<NamomoUser, DefaultContext>>();

                    services.ConfigurePolygonStorage(options =>
                    {
                        options.JudgingDirectory = Path.Combine(context.HostingEnvironment.ContentRootPath, "Runs");
                        options.ProblemDirectory = Path.Combine(context.HostingEnvironment.ContentRootPath, "Problems");
                    });

                    services.Configure<ContestModule.Routing.MinimalSiteOptions>(options =>
                    {
                        options.Keyword = context.Configuration.GetConnectionString("ContestKeyword");
                    });
                });
    }
}
