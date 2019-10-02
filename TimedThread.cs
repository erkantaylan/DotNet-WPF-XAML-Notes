using System;
using System.Threading;
using System.Threading.Tasks;

public class TimedThread
{
    private readonly System.Action action;
    private readonly int timeoutMiliseconds;

    public TimedThread(Action action, int timeoutMiliseconds)
    {
        this.action = action;
        this.timeoutMiliseconds = timeoutMiliseconds;
    }

    public ThreadStatus Invoke()
    {
        Exception exception = null;
        var cancellationTokenSource = new CancellationTokenSource();
        Task task = Task.Run(() =>
        {
            try
            {
                using (cancellationTokenSource.Token.Register(Thread.CurrentThread.Abort))
                {
                    action();
                }
            }
            catch (Exception e)
            {
                if (!(e is ThreadAbortException))
                {
                    exception = e;
                }
            }
        }, cancellationTokenSource.Token);
        bool done = task.Wait(timeoutMiliseconds);
        if (exception != null)
        {
            throw exception;
        }

        if (done)
        {
            return ThreadStatus.DONE;
        }

        cancellationTokenSource.Cancel();
        return ThreadStatus.TIMEOUT;
    }

    public async Task<ThreadStatus> InvokeAsync()
    {
        return await Task.Factory.StartNew(Invoke);
    }
}

public enum ThreadStatus
{
    DONE,
    TIMEOUT
}

public class TimedThread<TResult>
{
    private readonly Func<TResult> action;
    private readonly int timeoutMiliseconds;

    public TimedThread(Func<TResult> action, int timeoutMiliseconds)
    {
        this.action = action;
        this.timeoutMiliseconds = timeoutMiliseconds;
    }

    public (ThreadStatus, TResult) Invoke()
    {
        Exception exception = null;
        var cancellationTokenSource = new CancellationTokenSource();

        var myTask = new Task<TResult>(() => action(), cancellationTokenSource.Token);
        myTask.Start();

        Task<TResult> task = Task.Run(() =>
        {
            TResult result = default;
            try
            {
                using (cancellationTokenSource.Token.Register(Thread.CurrentThread.Abort))
                {
                    result = action();
                }
            }
            catch (Exception e)
            {
                if (!(e is ThreadAbortException))
                {
                    exception = e;
                }
            }

            return result;
        }, cancellationTokenSource.Token);
        bool done = task.Wait(timeoutMiliseconds);
        if (exception != null)
        {
            throw exception;
        }

        if (done)
        {
            return (ThreadStatus.DONE, task.Result);
        }

        cancellationTokenSource.Cancel();
        return (ThreadStatus.TIMEOUT, task.Result);
    }

    public async Task<(ThreadStatus, TResult)> InvokeAsync()
    {
        return await Task.Factory.StartNew(Invoke);
    }
}
