using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FaviconFetcher.Extensions
{
    public static class WebRequestExtension
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken, Action action, bool useSynchronizationContext = true)
        {
            using (cancellationToken.Register(action, useSynchronizationContext))
            {
                try
                {
                    return await task;
                }
                catch (Exception ex)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException

                        // NetStandard 2 form doesn't include cancellationToken as 3rd param
                        throw new TaskCanceledException(ex.Message, ex);
                        //throw new OperationCanceledException(ex.Message, ex, cancellationToken);
                    }

                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }
    }
}
