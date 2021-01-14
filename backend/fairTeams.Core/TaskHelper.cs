using System;
using System.Threading;
using System.Threading.Tasks;

namespace fairTeams.Core
{
    public static class TaskHelper
    {
        public static TaskCompletionSource CreateResultlessTaskCompletionSourceWithTimeout(int timeoutInMilliseconds)
        {
            return CreateResultlessTaskCompletionSourceWithTimeout(timeoutInMilliseconds, null);
        }

        public static TaskCompletionSource CreateResultlessTaskCompletionSourceWithTimeout(int timeoutInMilliseconds, string timeoutExceptionMessage)
        {
            var taskCompletionSource = new TaskCompletionSource();
            var ct = new CancellationTokenSource(timeoutInMilliseconds);

            if (string.IsNullOrEmpty(timeoutExceptionMessage))
            {
                ct.Token.Register(() => taskCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            }
            else
            {
                ct.Token.Register(() => taskCompletionSource.TrySetException(new TimeoutException(timeoutExceptionMessage)), useSynchronizationContext: false);
            }


            return taskCompletionSource;
        }

        public static TaskCompletionSource<T> CreateTaskCompletionSourceWithTimeout<T>(int timeoutInMilliseconds)
        {
            return CreateTaskCompletionSourceWithTimeout<T>(timeoutInMilliseconds, null);
        }

        public static TaskCompletionSource<T> CreateTaskCompletionSourceWithTimeout<T>(int timeoutInMilliseconds, string timeoutExceptionMessage)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            var ct = new CancellationTokenSource(timeoutInMilliseconds);

            if (string.IsNullOrEmpty(timeoutExceptionMessage))
            {
                ct.Token.Register(() => taskCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            }
            else
            {
                ct.Token.Register(() => taskCompletionSource.TrySetException(new TimeoutException(timeoutExceptionMessage)), useSynchronizationContext: false);
            }

            return taskCompletionSource;
        }
    }
}
