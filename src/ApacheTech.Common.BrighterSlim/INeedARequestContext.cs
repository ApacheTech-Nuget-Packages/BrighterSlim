﻿#region Licence
/* The MIT License (MIT)
Copyright Â© 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the â€œSoftwareâ€), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED â€œAS ISâ€, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

namespace ApacheTech.Common.BrighterSlim
{
    /// <summary>
    /// Interface INeedARequestContext
    /// </summary>
    public interface INeedARequestContext
    {
        /// <summary>
        /// Requests the context factory.
        /// </summary>
        /// <param name="requestContextFactory">The request context factory.</param>
        /// <returns>IAmACommandProcessorBuilder.</returns>
        IAmACommandProcessorBuilder RequestContextFactory(IAmARequestContextFactory requestContextFactory);
    }
}
