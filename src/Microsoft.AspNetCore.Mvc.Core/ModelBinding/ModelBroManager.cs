using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class ModelBroManager : IModelBroManager
    {
        private readonly IModelBroFactory[] _factories;

        public ModelBroManager(IOptions<MvcOptions> options)
        {
            _factories = options.Value.ModelBroFactory.ToArray();
        }

        public IModelBro GetItBro(ModelBroManagerContext context)
        {
            var factoryContext = new ModelBroFactoryContext();

            for (var i = 0; i < _factories.Length; i++)
            {
                var factory = _factories[i];
                var result = factory.Create(factoryContext);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
