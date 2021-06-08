using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace SatelliteSite
{
    public static class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .AutoMigrate<PdsContext>()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .MarkDomain<PdsContext>()
                .AddModule<PlagModule.PlagModule<Plag.Backend.StorageBackendRole<PdsContext>>>()
                .AddDatabase<PdsContext>((c, b) => b.UseSqlServer(c.GetConnectionString("PlagDbConnection"), b => b.UseBulk()))
                .ConfigureServices(services => services.ConfigureApplicationBuilder(options => options.PointBeforeEndpoint.Add(app => app.Use(FakeAuthorization))))
                .ConfigureSubstrateDefaultsCore();

        static Task FakeAuthorization(HttpContext httpContext, Func<Task> next)
        {
            httpContext.Items["__AuthorizationMiddlewareWithEndpointInvoked"] = true;
            return next();
        }
    }
}
