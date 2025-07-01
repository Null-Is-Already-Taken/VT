using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VT.Utils
{
    public class Timer : IDisposable
    {
        #region Events
        private event Action OnStarted = () => { };
        private event Action<float> OnUpdated = _ => { };
        private event Action OnCompleted = () => { };
        #endregion

        #region Public Properties
        public float Duration { get; private set; } = -1f;
        public float Elapsed { get; private set; } = 0f;
        public float Progress => Duration > 0f ? Mathf.Clamp01(Elapsed / Duration) : 0f;
        public bool IsRunning { get; private set; } = false;
        #endregion

        #region Internal State
        private bool autoDispose = false;
        private bool isDisposed = false;
        private bool useUnscaledTime = false;

        private MonoBehaviour runner;
        private GameObject runnerGameObject;
        private Coroutine tickRoutine;
        #endregion

        #region Constructors & Factory
        private Timer() { }

        public static Timer Create(float seconds, MonoBehaviour runner = null)
        {
            return new Timer()
                .SetDuration(seconds)
                .WithRunner(runner);
        }
        #endregion

        #region Configuration Methods
        public Timer SetDuration(float seconds)
        {
            EnsureConfigurable();
            if (seconds < 0f && seconds != -1f)
            {
                Debug.LogWarning("Timer duration cannot be negative. Using -1 for infinite duration.");
                Duration = -1;
                return this;
            }
            Duration = seconds;
            return this;
        }

        public Timer RunIndefinitely()
        {
            EnsureConfigurable();
            return SetDuration(-1f);
        }

        public Timer OnStart(Action onStart)
        {
            EnsureConfigurable();
            OnStarted += onStart;
            return this;
        }

        public Timer OnUpdate(Action<float> onUpdate)
        {
            EnsureConfigurable();
            OnUpdated += onUpdate;
            return this;
        }

        public Timer OnComplete(Action onComplete)
        {
            EnsureConfigurable();
            OnCompleted += onComplete;
            return this;
        }

        public Timer WithRunner(MonoBehaviour runner)
        {
            EnsureConfigurable();
            if (runner == null)
                return this;

            this.runner = runner;
            if (runnerGameObject != null)
            {
                UnityEngine.Object.Destroy(runnerGameObject);
                runnerGameObject = null;
            }

            return this;
        }

        public Timer UseUnscaledTime(bool useUnscaledTime)
        {
            EnsureConfigurable();

            this.useUnscaledTime = useUnscaledTime;
            return this;
        }

        public Timer LogProgress()
        {
            EnsureConfigurable();

            OnUpdated += progress => Debug.Log($"Progress: {progress:P0}");
            return this;
        }

        public Timer LogTimeElapsed()
        {
            EnsureConfigurable();

            OnUpdated += _ => Debug.Log($"Time Elapsed: {Elapsed}");
            return this;
        }

        public Timer AutoDispose()
        {
            EnsureConfigurable();

            autoDispose = true;
            return this;
        }
        #endregion

        #region Control Methods
        public void Start()
        {
            EnsureAvailable();
            if (Duration < 0f && Duration != -1f)
            {
                Debug.LogWarning("Timer duration cannot be negative. Using -1 for infinite duration.");
                return;
            }

            Stop();
            EnsureRunner();
            Elapsed = 0f;
            IsRunning = true;
            OnStarted.Invoke();

            if (Duration == 0f) // if duration is 0, complete immediately
            {
                OnUpdated.Invoke(1f);
                OnCompleted.Invoke();
                if (autoDispose)
                    Dispose();
                return;
            }

            tickRoutine = runner.StartCoroutine(Tick());
        }

        public void Pause()
        {
            EnsureAvailable();
            IsRunning = false;
        }

        public void Resume()
        {
            EnsureAvailable();
            IsRunning = true;
        }

        public void Reset()
        {
            EnsureAvailable();
            IsRunning = false;
            Elapsed = 0f;
        }

        public void Restart()
        {
            EnsureAvailable();
            if (tickRoutine != null && IsRunning)
                Stop();

            Start();
        }

        public void Stop()
        {
            EnsureAvailable();
            if (tickRoutine != null)
            {
                runner.StopCoroutine(tickRoutine);
                tickRoutine = null;
            }
            IsRunning = false;
        }
        #endregion

        #region Disposal
        public void Dispose()
        {
            EnsureAvailable();
            Stop();

            OnStarted = null;
            OnUpdated = null;
            OnCompleted = null;

            if (runnerGameObject != null)
            {
                UnityEngine.Object.Destroy(runnerGameObject);
                runnerGameObject = null;
            }

            runner = null;
            IsRunning = false;
            isDisposed = true;
        }
        #endregion

        #region Private Helpers
        private void EnsureAvailable()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(Timer));
        }

        private void EnsureConfigurable([CallerMemberName] string method = null)
        {
            EnsureAvailable();

            if (IsRunning)
                throw new InvalidOperationException($"Cannot call '{method}' while the timer is running. Stop first.");
        }

        private void EnsureRunner()
        {
            if (runner == null)
            {
                runnerGameObject = new GameObject("Timer Runner");
                runnerGameObject.hideFlags = HideFlags.HideAndDontSave;
                runner = runnerGameObject.AddComponent<TimerRunnerMonoBehaviour>();
            }
        }
        #endregion

        #region Public Helpers
        public override string ToString()
        {
            return ToTimerString(Elapsed);
        }

        public static string ToTimerString(float seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
        #endregion

        #region Coroutine
        /// <summary>
        /// Coroutine that handles the timer countdown and triggers events.
        /// </summary>
        private IEnumerator Tick()
        {
            Elapsed = 0f;

            // If timerDuration is -1, run indefinitely; otherwise, run until timeElapsedInSeconds >= timerDuration.
            while (Duration == -1f || Elapsed < Duration)
            {
                if (IsRunning)
                {
                    Elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                    // Avoid division by zero when timerDuration is 0 or less
                    OnUpdated.Invoke(Progress); // Pass normalized progress (0 to 1)
                }

                yield return null;
            }

            // Invoke completion callback once the timer ends
            OnCompleted.Invoke();
            tickRoutine = null;
            IsRunning = false;

            if (autoDispose)
            {
                Dispose(); // Clean up if one-shot timer
            }
        }
        #endregion

        private class TimerRunnerMonoBehaviour : MonoBehaviour { }
    }
}
