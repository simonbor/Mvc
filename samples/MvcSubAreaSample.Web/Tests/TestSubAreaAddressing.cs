using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder.Internal;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MvcSubAreaSample.Web.Tests
{
    public class TestSubAreaAddressing
    {
        [Fact]
        public async Task ExecuteResultAsync_SubAreaGivesCorrectRoute()
        {
            var startup = new Startup();
            var serviceCollection = startup.ConfigureServices(new ServiceCollection());
            var builder = new ApplicationBuilder(serviceCollection);
            var requestDeligate = builder.Build();

            var actionDescriptor = CreateActionDescriptor("Menu", "Restaurant", "Home", "Index");
            
            throw new NotImplementedException();
        }

        private static ActionDescriptor CreateActionDescriptor(string subArea, string area, string controller, string action)
        {
            var actionDescriptor = new ActionDescriptor()
            {
                Name = $"SubArea: {subArea}, Area: {area}, Controller: {controller}, Action: {action}",
                RouteConstraints = new List<RouteDataActionConstraint>()
            };

            actionDescriptor.RouteConstraints.Add(
                area == null ?
                new RouteDataActionConstraint("area", null) :
                new RouteDataActionConstraint("area", area));

            actionDescriptor.RouteConstraints.Add(
                subArea == null ?
                new RouteDataActionConstraint("subarea", null) :
                new RouteDataActionConstraint("subarea", subArea));

            actionDescriptor.RouteConstraints.Add(
                controller == null ?
                new RouteDataActionConstraint("controller", null) :
                new RouteDataActionConstraint("controller", controller));

            actionDescriptor.RouteConstraints.Add(
                action == null ?
                new RouteDataActionConstraint("action", null) :
                new RouteDataActionConstraint("action", action));

            return actionDescriptor;
        }
    }
}
