using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class BinderTypeModelBroFactory : IModelBroFactory
    {
        public IModelBro Create(ModelBroFactoryContext context)
        {
            if (context.BindingInfo.BinderType != null)
            {
                // TODO: Type Activate
                return (IModelBro)Activator.CreateInstance(context.BindingInfo.BinderType);
            }

            return null;
        }
    }
}
