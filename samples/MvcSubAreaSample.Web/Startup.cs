// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MvcSubAreaSample.Web
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SubAreaViewLocationExpander());
            });

            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(PassThroughAttribute), order: 17);
                options.Filters.Add(new FormatFilterAttribute());
            });

            return services.BuildServiceProvider();
        }

        public IConfigurationRoot Configuration { get; set; }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app)
        {
            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute("areaBase", "{area:exists}",
                    new
                    {
                        controller = "Home",
                        action = "Index"
                    });
                routes.MapRoute("subareaBase", "{area:exists}/{subarea:exists}",
                    new
                    {
                        controller = "Home",
                        action = "Index"
                    });
                routes.MapRoute("subarearoute", "{area:exists}/{subarea:exists}/{controller}/{action}");
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action}");
                routes.MapRoute(
                    "controllerActionRoute",
                    "{controller}/{action}",
                    new { controller = "Home", action = "Index" },
                    constraints: null,
                    dataTokens: new { NameSpace = "default" });

                routes.MapRoute(
                    "controllerRoute",
                    "{controller}",
                    new { controller = "Home" });
            });
        }

        public static void Main(string[] args)
        {
            var application = new WebApplicationBuilder()
                .UseConfiguration(WebApplicationConfiguration.GetDefault(args))
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
