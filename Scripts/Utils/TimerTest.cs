using Sirenix.OdinInspector;
using UnityEngine;
using VT.Utils;

namespace VT.Utils.Tests
{
    public class TimerTest : MonoBehaviour
    {
        [SerializeField] private float duration;
        private Timer currentTimer;

        private Timer BuildBasicTimer()
        {
            Debug.Log("--- Basic Timer Example ---");
            
            var basicTimer = new Timer(this);
            basicTimer.New(
                duration: duration,
                onStart: () => Debug.Log("Basic timer started!"),
                onUpdate: progress => Debug.Log($"Basic timer progress: {progress:P0}"),
                onComplete: () => Debug.Log("Basic timer completed!")
            );
            return basicTimer;
        }

        private Timer BuildFluentTimer()
        {
            Debug.Log("--- Fluent API Timer Example ---");

            Timer fluentTimer = new Timer(this)
                .SetDuration(duration)
                .OnStart(() => Debug.Log("Fluent timer started!"))
                .LogProgress()
                .OnComplete(() => Debug.Log("Fluent timer completed!"));
            
            return fluentTimer;
        }

        private Timer BuildInfiniteTimer()
        {
            Debug.Log("--- Infinite Timer Example ---");

            Timer infiniteTimer = new Timer(this)
                .Infinite() // Infinite duration
                .OnStart(() => Debug.Log("Infinite timer started!"))
                .LogTimeElapsed();

            return infiniteTimer;
        }

        private Timer BuildOneShotTimer()
        {
            Debug.Log("--- One-Shot Timer Example ---");

            Timer oneShotTimer = new Timer(this, oneShot: true)
                .SetDuration(2f)
                .OnStart(() => Debug.Log("One-shot timer started!"))
                .OnComplete(() => Debug.Log("One-shot timer completed and auto-disposed!"));

            return oneShotTimer;
        }

        private Timer BuildPauseAndResumeTimer(Timer targetTimer, float resumeAfterSeconds)
        {
            Timer pauseAndResumeTimer = new Timer(this)
                .SetDuration(resumeAfterSeconds)
                .OnStart(() => targetTimer.Pause())
                .OnComplete(() => targetTimer.Resume())
                .AutoDispose();

            return pauseAndResumeTimer;
        }

        [Button]
        public void TestBasicTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }
            currentTimer = BuildBasicTimer();
            currentTimer.Start();
        }

        [Button]
        public void TestFluentTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }
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
            currentTimer = BuildInfiniteTimer();
            currentTimer.Start();
        }

        public void TestOneShotTimer()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
                currentTimer.Stop();
            }
            currentTimer = BuildOneShotTimer();
        }

        [Button]
        public void TestRestart()
        {
            if (currentTimer != null && currentTimer.IsRunning)
            {
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