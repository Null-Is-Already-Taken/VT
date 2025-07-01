using Sirenix.OdinInspector;
using UnityEngine;

namespace VT.Utils.Tests
{
    public class TimerTest : MonoBehaviour
    {
        [SerializeField] private float duration;
        private Timer currentTimer;

        private Timer BuildExternalRunnerTimer()
        {
            Timer basicTimer = Timer.Create()
                .SetDuration(duration)
                .WithRunner(this);

            TimerLogger.Attach(basicTimer)
                .WithName("External Runner")
                .LogProgress();
            
            return basicTimer;
        }

        private Timer BuildFluentTimer()
        {
            Timer fluentTimer = Timer.Create()
                .SetDuration(duration);

            TimerLogger.Attach(fluentTimer)
                .WithName("Fluent")
                .LogProgress();

            return fluentTimer;
        }

        private Timer BuildInfiniteTimer()
        {
            Timer infiniteTimer = Timer.Create()
                .RunIndefinitely(); // Infinite duration

            TimerLogger.Attach(infiniteTimer)
                .WithName("Infinite")
                .LogProgress()
                .LogTime();

            return infiniteTimer;
        }

        private Timer BuildOneShotTimer()
        {
            Timer oneShotTimer = Timer.Create()
                .SetDuration(duration)
                .AutoDispose();

            TimerLogger.Attach(oneShotTimer)
                .WithName("Infinite")
                .LogProgress();

            return oneShotTimer;
        }

        private Timer BuildPauseAndResumeTimer(Timer targetTimer, float resumeAfterSeconds)
        {
            Timer pauseAndResumeTimer = Timer.Create()
                .SetDuration(resumeAfterSeconds)
                .OnStart(() => targetTimer.Pause())
                .OnComplete(() => targetTimer.Resume())
                .AutoDispose();

            TimerLogger.Attach(pauseAndResumeTimer)
                .WithName("Pause & Resume")
                .LogTime();

            return pauseAndResumeTimer;
        }

        [Button]
        public void ExternalRunnerTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }

            Debug.Log("--- External Runner Timer Example ---");
            currentTimer = BuildExternalRunnerTimer();
            currentTimer.Start();
        }

        [Button]
        public void TestFluentTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }

            Debug.Log("--- Fluent API Timer Example ---");
            currentTimer = BuildFluentTimer();
            currentTimer.Start();
        }

        [Button]
        public void TestInfiniteTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }

            Debug.Log("--- Infinite Timer Example ---");
            currentTimer = BuildInfiniteTimer();
            currentTimer.Start();
        }

        public void TestOneShotTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }

            Debug.Log("--- One Shot Timer Example ---");
            currentTimer = BuildOneShotTimer();
            currentTimer.Start();
        }

        [Button]
        public void TestRestart()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                Debug.Log("--- Restart Timer Example ---");
                currentTimer.Restart();
            }
            else
            {
                Debug.Log("Current timer is not running");
            }
        }

        [Button]
        public void TestPauseResume()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                float pauseDuration = 5f;
                Debug.Log($"Pausing {currentTimer} for {pauseDuration}s...");
                Timer pauseAndResumeTimer = BuildPauseAndResumeTimer(currentTimer, pauseDuration);
                pauseAndResumeTimer.Start();
            }
            else
            {
                Debug.Log("Current timer is not running");
            }
        }

        [Button]
        public void StopAllTimers()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                Debug.Log($"Stopping {currentTimer} timer");
                currentTimer?.Stop();
            }
            else
            {
                Debug.Log("Timer is not running");
            }
        }

        [Button]
        public void DisposeAllTimers()
        {
            Debug.Log("Disposing all timers...");
            
            currentTimer?.Dispose();
            currentTimer = null;
        }

        private void OnDestroy()
        {
            DisposeAllTimers();
        }
    }
} 