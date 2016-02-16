using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    public class BodyModelBroFactory : IModelBroFactory
    {
        private readonly Func<Stream, Encoding, TextReader> _readerFactory;

        public BodyModelBroFactory(IHttpRequestStreamReaderFactory readerFactory)
        {
            _readerFactory = readerFactory.CreateReader;
        }

        public IModelBro Create(ModelBroFactoryContext context)
        {
            if (context.BindingInfo.BindingSource == BindingSource.Body)
            {
                var key = context.IsTopLevelObject ? context.ModelMetadata.BinderModelName ?? string.Empty : null;
                return new Binder(_readerFactory, key);
            }

            return null;
        }

        private class Binder : IModelBro
        {
            private readonly Func<Stream, Encoding, TextReader> _readerFactory;
            private readonly string _modelBindingKey;

            public Binder(Func<Stream, Encoding, TextReader> readerFactory, string modelBindingKey)
            {
                _readerFactory = readerFactory;
                _modelBindingKey = modelBindingKey;
            }

            public async Task BindAsync(ModelBroContext bindingContext)
            {
                if (bindingContext == null)
                {
                    throw new ArgumentNullException(nameof(bindingContext));
                }
                
                var httpContext = bindingContext.HttpContext;
                var key = _modelBindingKey ?? bindingContext.ModelName;

                var formatterContext = new InputFormatterContext(
                    httpContext,
                    key,
                    bindingContext.ModelState,
                    bindingContext.ModelMetadata,
                    _readerFactory);

                var formatters = bindingContext.InputFormatters;
                var formatter = formatters.FirstOrDefault(f => f.CanRead(formatterContext));

                if (formatter == null)
                {
                    var message = Resources.FormatUnsupportedContentType(
                        bindingContext.HttpContext.Request.ContentType);

                    var exception = new UnsupportedContentTypeException(message);
                    bindingContext.ModelState.AddModelError(key, exception, bindingContext.ModelMetadata);

                    // This model binder is the only handler for the Body binding source and it cannot run twice. Always
                    // tell the model binding system to skip other model binders and never to fall back i.e. indicate a
                    // fatal error.
                    bindingContext.Result = ModelBindingResult.Failed(key);
                    return;
                }

                try
                {
                    var previousCount = bindingContext.ModelState.ErrorCount;
                    var result = await formatter.ReadAsync(formatterContext);
                    var model = result.Model;

                    if (result.HasError)
                    {
                        // Formatter encountered an error. Do not use the model it returned. As above, tell the model
                        // binding system to skip other model binders and never to fall back.
                        bindingContext.Result = ModelBindingResult.Failed(key);
                        return;
                    }

                    bindingContext.Result = ModelBindingResult.Success(key, model);
                    return;
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError(key, ex, bindingContext.ModelMetadata);

                    // This model binder is the only handler for the Body binding source and it cannot run twice. Always
                    // tell the model binding system to skip other model binders and never to fall back i.e. indicate a
                    // fatal error.
                    bindingContext.Result = ModelBindingResult.Failed(key);
                    return;
                }
            }
        }
    }
}
