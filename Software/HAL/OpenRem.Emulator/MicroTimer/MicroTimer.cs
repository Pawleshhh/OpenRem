using System.Threading;

namespace OpenRem.Emulator;

/// <summary>
/// MicroTimer class
/// </summary>
public class MicroTimer
{
    public delegate void MicroTimerElapsedEventHandler(
        object sender,
        MicroTimerEventArgs e);

    public event MicroTimerElapsedEventHandler MicroTimerElapsed;

    System.Threading.Thread _threadTimer = null;
    CancellationToken cancellationToken;
    long _ignoreEventIfLateBy = long.MaxValue;
    long _timerIntervalInMicroSec = 0;
    bool _stopTimer = true;

    public MicroTimer()
    {
    }

    public MicroTimer(long timerIntervalInMicroseconds)
    {
        Interval = timerIntervalInMicroseconds;
    }

    public long Interval
    {
        get
        {
            return System.Threading.Interlocked.Read(
                ref this._timerIntervalInMicroSec);
        }
        set
        {
            System.Threading.Interlocked.Exchange(
                ref this._timerIntervalInMicroSec, value);
        }
    }

    public long IgnoreEventIfLateBy
    {
        get
        {
            return System.Threading.Interlocked.Read(
                ref this._ignoreEventIfLateBy);
        }
        set
        {
            System.Threading.Interlocked.Exchange(
                ref this._ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value);
        }
    }

    public bool Enabled
    {
        set
        {
            if (value)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }
        get { return (this._threadTimer != null && this._threadTimer.IsAlive); }
    }

    public void Start()
    {
        if (Enabled || Interval <= 0)
        {
            return;
        }

        this._stopTimer = false;

        System.Threading.ThreadStart threadStart = delegate ()
        {
            NotificationTimer(ref this._timerIntervalInMicroSec,
                ref this._ignoreEventIfLateBy,
                ref this._stopTimer);
        };

        this._threadTimer = new System.Threading.Thread(threadStart);
        this.cancellationToken = new CancellationToken();
        this._threadTimer.Priority = System.Threading.ThreadPriority.Highest;
        this._threadTimer.Start();
    }

    public void Stop()
    {
        this._stopTimer = true;
    }

    public void StopAndWait()
    {
        StopAndWait(System.Threading.Timeout.Infinite);
    }

    public bool StopAndWait(int timeoutInMilliSec)
    {
        this._stopTimer = true;

        if (!Enabled || this._threadTimer.ManagedThreadId ==
            System.Threading.Thread.CurrentThread.ManagedThreadId)
        {
            return true;
        }

        return this._threadTimer.Join(timeoutInMilliSec);
    }

    public void Abort()
    {
        this._stopTimer = true;

        if (Enabled)
        {
            this.cancellationToken.ThrowIfCancellationRequested();
        }
    }

    void NotificationTimer(ref long timerIntervalInMicroSec,
        ref long ignoreEventIfLateBy,
        ref bool stopTimer)
    {
        int timerCount = 0;
        long nextNotification = 0;

        MicroStopwatch microStopwatch = new MicroStopwatch();
        microStopwatch.Start();

        while (!stopTimer)
        {
            long callbackFunctionExecutionTime =
                microStopwatch.ElapsedMicroseconds - nextNotification;

            long timerIntervalInMicroSecCurrent =
                System.Threading.Interlocked.Read(ref timerIntervalInMicroSec);
            long ignoreEventIfLateByCurrent =
                System.Threading.Interlocked.Read(ref ignoreEventIfLateBy);

            nextNotification += timerIntervalInMicroSecCurrent;
            timerCount++;
            long elapsedMicroseconds = 0;

            while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds)
                   < nextNotification)
            {
                System.Threading.Thread.SpinWait(10);
            }

            long timerLateBy = elapsedMicroseconds - nextNotification;

            if (timerLateBy >= ignoreEventIfLateByCurrent)
            {
                continue;
            }

            MicroTimerEventArgs microTimerEventArgs =
                new MicroTimerEventArgs(timerCount,
                    elapsedMicroseconds,
                    timerLateBy,
                    callbackFunctionExecutionTime);
            MicroTimerElapsed?.Invoke(this, microTimerEventArgs);
        }

        microStopwatch.Stop();
    }
}