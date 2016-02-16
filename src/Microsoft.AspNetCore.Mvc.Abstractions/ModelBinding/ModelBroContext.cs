using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class ModelBroContext
    {
        public ModelBroContext(ModelBindingContext context)
        {
            Inner = context;
        }

        protected ModelBindingContext Inner { get; }

        public string FieldName => Inner.FieldName;

        public HttpContext HttpContext => Inner.OperationBindingContext.HttpContext;

        public IList<IInputFormatter> InputFormatters => Inner.OperationBindingContext.InputFormatters;

        public ModelMetadata ModelMetadata => Inner.ModelMetadata;

        public string ModelName => Inner.ModelName;

        public ModelStateDictionary ModelState => Inner.OperationBindingContext.ActionContext.ModelState;

        public IServiceProvider RequestServices => Inner.OperationBindingContext.HttpContext.RequestServices;

        public object Result { get; set; }

        public IValueProvider ValueProvider => Inner.OperationBindingContext.ValueProvider;
    }
}
