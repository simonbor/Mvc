using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class HeaderModelBroFactory : IModelBroFactory
    {
        public IModelBro Create(ModelBroFactoryContext context)
        {
            if (context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Header))
            {
                return new Binder();
            }

            return null;
        }

        private class Binder : IModelBro
        {
            public Task BindAsync(ModelBroContext bindingContext)
            {
                if (bindingContext == null)
                {
                    throw new ArgumentNullException(nameof(bindingContext));
                }

                var request = bindingContext.HttpContext.Request;
                var modelMetadata = bindingContext.ModelMetadata;

                // Property name can be null if the model metadata represents a type (rather than a property or parameter).
                var headerName = bindingContext.FieldName;
                object model = null;
                if (bindingContext.ModelMetadata.ModelType == typeof(string))
                {
                    string value = request.Headers[headerName];
                    if (value != null)
                    {
                        model = value;
                    }
                }
                else if (typeof(IEnumerable<string>).IsAssignableFrom(bindingContext.ModelMetadata.ModelType))
                {
                    var values = request.Headers.GetCommaSeparatedValues(headerName);
                    if (values.Length > 0)
                    {
                        model = ModelBindingHelper.ConvertValuesToCollectionType(
                            bindingContext.ModelMetadata.ModelType,
                            values);
                    }
                }

                if (model == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed(bindingContext.ModelName);
                    return TaskCache.CompletedTask;
                }
                else
                {
                    bindingContext.ModelState.SetModelValue(
                        bindingContext.ModelName,
                        request.Headers.GetCommaSeparatedValues(headerName),
                        request.Headers[headerName]);

                    bindingContext.Result = ModelBindingResult.Success(bindingContext.ModelName, model);
                    return TaskCache.CompletedTask;
                }
            }
        }
    }
}
