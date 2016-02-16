using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class ServicesModelBroFactory : IModelBroFactory
    {
        public IModelBro Create(ModelBroFactoryContext context)
        {
            if (context.BindingInfo.BindingSource == BindingSource.Services)
            {
                return new Binder(context.ModelMetadata.ModelType);
            }

            return null;
        }

        private class Binder : IModelBro
        {
            private readonly Type _type;

            public Binder(Type type)
            {
                _type = type;
            }

            public Task BindAsync(ModelBroContext context)
            {
                context.Result = context.RequestServices.GetRequiredService
            }
        }
    }
}
