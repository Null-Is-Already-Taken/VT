using System;
using UnityEngine;

namespace VT.ReusableSystems.Timers.Utils
{
    /// <summary>
    /// Provides configurable logging for a Timer.
    /// </summary>
    public class TimerLogger
    {
        private readonly Timer timer;
        private string timerName = "Timer";
        private bool logProgress = false;
        private bool logTime = false;
        private float progressLogThreshold = 0.05f;
        private float timeLogThreshold = 1f;
        private float lastLoggedProgress = -1f;
        private float lastLoggedTime = -1f;
        private bool isSubscribed = false;
        private bool hasLoggedInfiniteProgress = false;

        private TimerLogger(Timer timer)
        {
            this.timer = timer ?? throw new ArgumentNullException(nameof(timer));
            Subscribe();
        }

        /// <summary>
        /// Starts a fluent configuration for logging a Timer.
        /// </summary>
        public static TimerLogger Attach(Timer timer)
        {
            return new TimerLogger(timer);
        }

        /// <summary>
        /// Enables progress logging. Default threshold is 5%.
        /// </summary>
        /// <param name="threshold">Minimum progress change (0-1) required to log. Default is 0.05 (5%).</param>
        public TimerLogger LogProgress(float threshold = 0.05f)
        {
            logProgress = true;
            progressLogThreshold = Mathf.Clamp01(threshold);
            return this;
        }

        /// <summary>
        /// Enables time logging. Default threshold is 1 second.
        /// </summary>
        /// <param name="threshold">Minimum time change in seconds required to log. Default is 1.0.</param>
        public TimerLogger LogTime(float threshold = 1f)
        {
            logTime = true;
            timeLogThreshold = Mathf.Max(0f, threshold);
            return this;
        }

        /// <summary>
        /// Sets a custom name for the timer in log messages.
        /// </summary>
        public TimerLogger WithName(string name)
        {
            timerName = string.IsNullOrEmpty(name) ? "Timer" : name;
            return this;
        }

        private void Subscribe()
        {
            if (!isSubscribed)
            {
                timer?.OnUpdate(OnTimerUpdate);
                timer?.OnStart(OnTimerStart);
                timer?.OnComplete(OnTimerComplete);
                isSubscribed = true;
            }
        }

        private void OnTimerStart()
        {
            lastLoggedProgress = -1f;
            lastLoggedTime = -1f;
            hasLoggedInfiniteProgress = false;
            Debug.Log($"[{timerName}] Timer started.");
        }

        private void OnTimerUpdate(float progress)
        {
            if (logProgress)
            {
                if (timer.Duration >= 0f)
                {
                    float change = Mathf.Abs(progress - lastLoggedProgress);
                    if (lastLoggedProgress < 0f || change >= progressLogThreshold)
                    {
                        Debug.Log($"[{timerName}] Progress: {progress:P0}");
                        lastLoggedProgress = progress;
                    }
                }
                else
                {
                    if (!hasLoggedInfiniteProgress)
                    {
                        Debug.Log($"[{timerName}] Progress: N/A");
                        hasLoggedInfiniteProgress = true;
                    }
                }
            }

            if (logTime)
            {
                float timeChange = Mathf.Abs(timer.Elapsed - lastLoggedTime);
                if (lastLoggedTime < 0f || timeChange >= timeLogThreshold)
                {
                    Debug.Log($"[{timerName}] Time Elapsed: {timer.Elapsed:F1}s");
                    lastLoggedTime = timer.Elapsed;
                }
            }
        }

        private void OnTimerComplete()
        {
            Debug.Log($"[{timerName}] Timer completed.");
        }
    }
}