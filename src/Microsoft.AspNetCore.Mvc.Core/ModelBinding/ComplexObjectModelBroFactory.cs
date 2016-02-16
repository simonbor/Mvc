using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class ComplexObjectModelBroFactory : IModelBroFactory
    {
        private Stack<KeyValuePair<ModelMetadata, IModelBro>> _stack = new Stack<KeyValuePair<ModelMetadata, IModelBro>>();

        public IModelBro Create(ModelBroFactoryContext context)
        {
            if (!context.ModelMetadata.IsComplexType)
            {
                return null;
            }
            
            foreach (var item in _stack)
            {
                if (item.Key.Equals(context.ModelMetadata))
                {
                    return item.Value;
                }
            }

            var binder = new Binder();
            _stack.Push(new KeyValuePair<ModelMetadata, IModelBro>(context.ModelMetadata, binder));

            foreach (var property in context.ModelMetadata.Properties)
            {
                binder.PropertyBinders.Add(property, context.CreateBro(property));
            }

            _stack.Pop();

            return binder;
        }

        private class Binder : IModelBro
        {
            public IDictionary<ModelMetadata, IModelBro> PropertyBinders { get; } = new Dictionary<ModelMetadata, IModelBro>();

            public Task BindAsync(ModelBroContext bindingContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
