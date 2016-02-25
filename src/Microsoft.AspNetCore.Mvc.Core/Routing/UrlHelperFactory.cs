// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    /// <summary>
    /// A default implementation of <see cref="IUrlHelperFactory"/>.
    /// </summary>
    public class UrlHelperFactory : IUrlHelperFactory
    {
        /// <inheritdoc />
        /// Perf: Added a pool of StringBuilder that will be used by URL Helpers when generating URLs for requests
        private ObjectPool<StringBuilder> _stringBuilderPool;

        public ObjectPool<StringBuilder> StringBuilderPool
        {
            get
            {
                if (_stringBuilderPool == null)
                {
                    _stringBuilderPool = new DefaultObjectPoolProvider().Create<StringBuilder>();
                }

                return _stringBuilderPool;
            }
        }
        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            var httpContext = context.HttpContext;
            IUrlHelper urlHelper = null;
            if (httpContext != null && httpContext.Features != null)
            {
                urlHelper = httpContext.Features[typeof(IUrlHelper)] as IUrlHelper;
                if (urlHelper == null)
                {
                    urlHelper = new UrlHelper(context, StringBuilderPool);
                    httpContext.Features[typeof(IUrlHelper)] = urlHelper;
                }
            }
            else
            {
                urlHelper = new UrlHelper(context, StringBuilderPool);
            }

            return urlHelper;
        }
    }
}
