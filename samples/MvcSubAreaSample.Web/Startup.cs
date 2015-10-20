using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Framework.Configuration;

namespace MvcSubAreaSample.Web
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCaching();
            services.AddSession();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SubAreaViewLocationExpander());
            });

            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(PassThroughAttribute), order: 17);
                options.Filters.Add(new FormatFilterAttribute());
            })
            .AddXmlDataContractSerializerFormatters()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder);

            return services.BuildServiceProvider();
        }

        public IConfigurationRoot Configuration { get; set; }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseFileServer();

            app.UseRequestLocalization();

            app.UseSession();
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
    }
}
