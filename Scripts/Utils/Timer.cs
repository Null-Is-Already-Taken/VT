using System;
using System.Collections;
using UnityEngine;

namespace VT.Utils
{
    public class Timer : IDisposable
    {
        private MonoBehaviour monoBehaviour;
        private Coroutine timerCoroutine;
        private float timerDuration;
        private Action onTimerComplete;
        private Action<float> onTimerUpdate;
        private float timeElapsedInSeconds;
        private bool pause;

        private GameObject ownedObject;
        private bool oneShot;
        private bool isDisposed;

        public float TimeElapsedInSeconds => timeElapsedInSeconds;
        public float TimeLeft => timerDuration - timeElapsedInSeconds;

        public Timer(MonoBehaviour monoBehaviour = null, bool oneShot = false)
        {
            if (monoBehaviour == null)
            {
                ownedObject = new GameObject("TimerObject");
                this.monoBehaviour = ownedObject.AddComponent<DummyMonoBehaviour>();
            }
            else
            {
                this.monoBehaviour = monoBehaviour;
            }

            this.oneShot = oneShot;
            isDisposed = false;
        }

        public float Progress => timerDuration > 0f ? Mathf.Clamp01(timeElapsedInSeconds / timerDuration) : -1f;

        public void Pause()
        {
            pause = true;
        }

        public void Resume()
        {
            pause = false;
        }

        /// <summary>
        /// Starts a timer with a specified duration and completion callback.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="onComplete">Callback when the timer completes.</param>
        /// <param name="onUpdate">Callback invoked each frame with the elapsed time as parameter (optional).</param>
        public void StartTimer(float duration, Action onComplete, Action<float> onUpdate = null)
        {
            if (isDisposed)
            {
                Debug.LogWarning("Attempted to start a disposed timer.");
                return;
            }

            if (duration < 0f && duration != -1f)
            {
                Debug.LogWarning("Timer duration cannot be negative. Using -1 for infinite duration.");
                return;
            }

            StopTimer();
            timerDuration = duration;
            onTimerComplete = onComplete;
            onTimerUpdate = onUpdate;

            if (duration == 0f) // if duration is 0, complete immediately
            {
                onTimerUpdate?.Invoke(1f);
                onTimerComplete?.Invoke();
                if (oneShot)
                    Dispose();
                return;
            }

            timerCoroutine = monoBehaviour.StartCoroutine(TimerCoroutine());
        }

        public void RestartTimer()
        {
            if (isDisposed)
            {
                Debug.LogWarning("Attempted to restart a disposed timer.");
                return;
            }

            if (timerCoroutine != null)
            {
                StopTimer();
            }

            StartTimer(timerDuration, onTimerComplete, onTimerUpdate);
        }

        /// <summary>
        /// Stops the timer if it's currently running.
        /// </summary>
        public void StopTimer()
        {
            if (isDisposed)
            {
                Debug.LogWarning("Attempted to stop a disposed timer.");
                return;
            }

            if (timerCoroutine != null)
            {
                monoBehaviour.StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }

        public override string ToString()
        {
            return ToDisplayString(timeElapsedInSeconds);
        }

        public static string ToDisplayString(float seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        /// <summary>
        /// Coroutine that handles the timer countdown and triggers events.
        /// </summary>
        private IEnumerator TimerCoroutine()
        {
            timeElapsedInSeconds = 0f;

            // If timerDuration is -1, run indefinitely; otherwise, run until timeElapsedInSeconds >= timerDuration.
            while (timerDuration == -1f || timeElapsedInSeconds < timerDuration)
            {
                if (!pause)
                {
                    timeElapsedInSeconds += Time.deltaTime;

                    // Avoid division by zero when timerDuration is 0 or less
                    onTimerUpdate?.Invoke(Progress); // Pass normalized progress (0 to 1)
                }

                yield return null;
            }

            // Invoke completion callback once the timer ends
            onTimerComplete?.Invoke();
            timerCoroutine = null;
            if (oneShot)
            {
                Dispose(); // Clean up if one-shot timer
            }
        }

        public void Dispose()
        {
            if (isDisposed) return;

            StopTimer();
            onTimerComplete = null;
            onTimerUpdate = null;

            if (ownedObject != null)
            {
                UnityEngine.Object.Destroy(ownedObject);
                ownedObject = null;
            }

            monoBehaviour = null;

            isDisposed = true;
        }

        private class DummyMonoBehaviour : MonoBehaviour { }
    }
}
