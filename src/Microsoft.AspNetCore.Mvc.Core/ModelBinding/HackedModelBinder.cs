using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class HackedModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var services = bindingContext.OperationBindingContext.HttpContext.RequestServices;
            var manager = services.GetRequiredService<IModelBroManager>();

            var binder = manager.GetItBro(new ModelBroManagerContext()
            {
                ActionDescriptor = bindingContext.OperationBindingContext.ActionContext.ActionDescriptor,
            });

            await binder.BindAsync(new ModelBroContext(bindingContext));
        }
    }
}
