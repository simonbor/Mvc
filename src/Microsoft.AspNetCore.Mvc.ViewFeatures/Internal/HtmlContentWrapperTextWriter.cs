// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    /// <summary>
    /// A <see cref="TextWriter"/> implementation which writes to an <see cref="IHtmlContentBuilder"/> instance.
    /// </summary>
    public class HtmlContentWrapperTextWriter : TextWriter
    {
        private const int MaxCharToStringLength = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContentWrapperTextWriter"/> class.
        /// </summary>
        /// <param name="contentBuilder">The <see cref="IHtmlContentBuilder"/> to write to.</param>
        /// <param name="encoding">The <see cref="System.Text.Encoding"/> in which output is written.</param>
        public HtmlContentWrapperTextWriter(IHtmlContentBuilder contentBuilder, Encoding encoding)
        {
            if (contentBuilder == null)
            {
                throw new ArgumentNullException(nameof(contentBuilder));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            ContentBuilder = contentBuilder;
            Encoding = encoding;
        }

        /// <summary>
        /// The <see cref="IHtmlContentBuilder"/> this <see cref="HtmlContentWrapperTextWriter"/> writes to.
        /// </summary>
        public IHtmlContentBuilder ContentBuilder { get; }

        /// <inheritdoc />
        public override Encoding Encoding { get; }

        /// <inheritdoc />
        public override void Write(char value)
        {
            Write(value.ToString());
        }

        /// <inheritdoc />
        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0 || (index + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            while (count > 0)
            {
                // Split large char arrays into 1KB strings.
                var currentCount = count;
                if (MaxCharToStringLength < currentCount)
                {
                    currentCount = MaxCharToStringLength;
                }

                Write(new string(buffer, index, currentCount));
                index += currentCount;
                count -= currentCount;
            }
        }

        /// <inheritdoc />
        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            ContentBuilder.AppendHtml(value);
        }

        /// <inheritdoc />
        public override void Write(object value)
        {
            IHtmlContentContainer container;
            IHtmlContent content;
            if ((container = value as IHtmlContentContainer) != null)
            {
                Write(container);
            }
            else if ((content = value as IHtmlContent) != null)
            {
                Write(content);
            }
            else
            {
                base.Write(value);
            }
        }

        /// <summary>
        /// Writes an <see cref="IHtmlContent"/> value.
        /// </summary>
        /// <param name="value">The <see cref="IHtmlContent"/> value.</param>
        public void Write(IHtmlContent value)
        {
            ContentBuilder.AppendHtml(value);
        }

        /// <summary>
        /// Writes an <see cref="IHtmlContentBuilder"/> value.
        /// </summary>
        /// <param name="value">The <see cref="IHtmlContentBuilder"/> value.</param>
        public void Write(IHtmlContentBuilder value)
        {
            value.MoveTo(ContentBuilder);
        }

        /// <inheritdoc />
        public override Task WriteAsync(char value)
        {
            Write(value.ToString());
            return TaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Write(buffer, index, count);
            return TaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public override Task WriteAsync(string value)
        {
            Write(value);
            return TaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public override void WriteLine()
        {
            Write(Environment.NewLine);
        }

        /// <inheritdoc />
        public override void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        /// <inheritdoc />
        public override Task WriteLineAsync(char value)
        {
            WriteLine(value);
            return TaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public override Task WriteLineAsync(char[] value, int start, int offset)
        {
            WriteLine(value, start, offset);
            return TaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public override Task WriteLineAsync(string value)
        {
            WriteLine(value);
            return TaskCache.CompletedTask;
        }

        /// <inheritdoc />
        public override Task WriteLineAsync()
        {
            WriteLine();
            return TaskCache.CompletedTask;
        }
    }
}