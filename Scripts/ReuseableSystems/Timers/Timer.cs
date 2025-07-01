using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VT.Utils
{
    /// <summary>
    /// A flexible timer utility that provides countdown functionality with event callbacks.
    /// Supports both finite and infinite durations, pausing/resuming, and automatic disposal.
    /// </summary>
    /// <remarks>
    /// The Timer class implements IDisposable for proper resource management. It can be used
    /// with or without a MonoBehaviour runner, and supports both scaled and unscaled time.
    /// 
    /// Example usage:
    /// <code>
    /// Timer.Create(5f)
    ///     .OnStart(() => Debug.Log("Timer started"))
    ///     .OnUpdate(progress => Debug.Log($"Progress: {progress:P0}"))
    ///     .OnComplete(() => Debug.Log("Timer completed"))
    ///     .AutoDispose()
    ///     .Start();
    /// </code>
    /// </remarks>
    public class Timer : IDisposable
    {
        #region Events
        private event Action OnStarted = () => { };
        private event Action<float> OnUpdated = _ => { };
        private event Action OnCompleted = () => { };
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the total duration of the timer in seconds.
        /// </summary>
        /// <remarks>
        /// A value of -1 indicates an infinite timer that will run indefinitely until stopped.
        /// A value of 0 will cause the timer to complete immediately when started.
        /// </remarks>
        public float Duration { get; private set; } = -1f;

        /// <summary>
        /// Gets the elapsed time in seconds since the timer started.
        /// </summary>
        /// <remarks>
        /// This value is reset to 0 when the timer is started or reset.
        /// </remarks>
        public float Elapsed { get; private set; } = 0f;

        /// <summary>
        /// Gets the normalized progress of the timer as a value between 0 and 1.
        /// </summary>
        /// <remarks>
        /// Returns 0 if the timer has no duration or hasn't started.
        /// Returns 1 when the timer has completed or exceeded its duration, or if duration is set to 0.
        /// Returns -1 when duration is -1 (infinite timer)
        /// </remarks>
        public float Progress
        {
            get
            {
                if (Duration > 0f)
                    return Mathf.Clamp01(Elapsed / Duration);
                else if (Duration == -1)
                    return -1f;
                else if (Duration == 0f)
                    return 1f;
                return 0f;
            }
        }

        /// <summary>
        /// Gets whether the timer is currently running.
        /// </summary>
        /// <remarks>
        /// This property is true when the timer is actively counting and false when
        /// paused, stopped, or completed.
        /// </remarks>
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
        /// <summary>
        /// Private constructor to enforce factory pattern usage.
        /// </summary>
        private Timer() { }

        /// <summary>
        /// Creates a new Timer instance with the specified duration.
        /// </summary>
        /// <param name="seconds">The duration of the timer in seconds. Use -1 for infinite duration.</param>
        /// <param name="runner">Optional MonoBehaviour to run the timer coroutine. If null, a temporary GameObject will be created.</param>
        /// <returns>A new Timer instance configured with the specified duration.</returns>
        /// <remarks>
        /// This is the recommended way to create Timer instances. The returned timer
        /// can be further configured using the fluent API before starting.
        /// </remarks>
        public static Timer Create(float seconds = -1, MonoBehaviour runner = null)
        {
            return new Timer()
                .SetDuration(seconds)
                .WithRunner(runner);
        }
        #endregion

        #region Configuration Methods
        /// <summary>
        /// Sets the duration of the timer.
        /// </summary>
        /// <param name="seconds">The duration in seconds. Use -1 for infinite duration.</param>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// Negative values (except -1) will be converted to -1 (infinite duration).
        /// A duration of 0 will cause the timer to complete immediately when started.
        /// </remarks>
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

        /// <summary>
        /// Configures the timer to run indefinitely (no completion).
        /// </summary>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// This is equivalent to calling SetDuration(-1f).
        /// </remarks>
        public Timer RunIndefinitely()
        {
            EnsureConfigurable();
            return SetDuration(-1f);
        }

        /// <summary>
        /// Registers a callback to be invoked when the timer starts.
        /// </summary>
        /// <param name="onStart">The action to execute when the timer starts.</param>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// Multiple callbacks can be registered and will all be executed when the timer starts.
        /// </remarks>
        public Timer OnStart(Action onStart)
        {
            EnsureConfigurable();
            OnStarted += onStart;
            return this;
        }

        /// <summary>
        /// Registers a callback to be invoked during each timer update.
        /// </summary>
        /// <param name="onUpdate">The action to execute during updates, receives an float from 0 to 1.</param>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// The callback receives a float parameter representing the normalized progress (0 to 1).
        /// Multiple callbacks can be registered and will all be executed during updates.
        /// </remarks>
        public Timer OnUpdate(Action<float> onUpdate)
        {
            EnsureConfigurable();
            OnUpdated += onUpdate;
            return this;
        }

        /// <summary>
        /// Registers a callback to be invoked when the timer completes.
        /// </summary>
        /// <param name="onComplete">The action to execute when the timer completes.</param>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// Multiple callbacks can be registered and will all be executed when the timer completes.
        /// For infinite timers, this callback will never be invoked.
        /// </remarks>
        public Timer OnComplete(Action onComplete)
        {
            EnsureConfigurable();
            OnCompleted += onComplete;
            return this;
        }

        /// <summary>
        /// Sets the MonoBehaviour that will run the timer coroutine.
        /// </summary>
        /// <param name="runner">The MonoBehaviour to use as the coroutine runner. If null, a temporary GameObject will be created.</param>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// If a runner is provided, it will be used to start and stop the timer coroutine.
        /// If no runner is provided, a temporary GameObject will be created and destroyed when the timer is disposed.
        /// </remarks>
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

        /// <summary>
        /// Configures whether the timer should use unscaled time instead of scaled time.
        /// </summary>
        /// <param name="useUnscaledTime">True to use Time.unscaledDeltaTime, false to use Time.deltaTime.</param>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// Unscaled time is useful for timers that should continue running even when the game is paused.
        /// </remarks>
        public Timer UseUnscaledTime(bool useUnscaledTime)
        {
            EnsureConfigurable();
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }

        /// <summary>
        /// Configures the timer to automatically dispose itself when completed.
        /// </summary>
        /// <returns>This timer instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called while the timer is running.</exception>
        /// <remarks>
        /// This is useful for one-shot timers that don't need to be manually disposed.
        /// </remarks>
        public Timer AutoDispose()
        {
            EnsureConfigurable();
            autoDispose = true;
            return this;
        }
        #endregion

        #region Control Methods
        /// <summary>
        /// Starts the timer countdown.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the timer has been disposed.</exception>
        /// <remarks>
        /// If the timer is already running, it will be stopped and restarted.
        /// If the duration is 0, the timer will complete immediately.
        /// </remarks>
        public void Start()
        {
            EnsureAvailable();
            Stop();
            EnsureRunner();
            IsRunning = true;
            OnStarted?.Invoke();

            if (Duration == 0f) // if duration is 0, complete immediately
            {
                Elapsed = Duration;
                OnUpdated?.Invoke(1f);
                OnCompleted?.Invoke();
                IsRunning = false;
                if (autoDispose)
                    Dispose();
                return;
            }

            tickRoutine = runner.StartCoroutine(Tick());
        }

        /// <summary>
        /// Pauses the timer countdown.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the timer has been disposed.</exception>
        /// <remarks>
        /// The timer will stop counting but retain its current elapsed time.
        /// Use Resume() to continue counting from where it left off.
        /// </remarks>
        public void Pause()
        {
            EnsureAvailable();
            IsRunning = false;
        }

        /// <summary>
        /// Resumes the timer countdown from where it was paused.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the timer has been disposed.</exception>
        /// <remarks>
        /// The timer will continue counting from its current elapsed time.
        /// </remarks>
        public void Resume()
        {
            EnsureAvailable();
            
            if (tickRoutine == null)
                throw new InvalidOperationException("Cannot Resume before Start()");
            
            IsRunning = true;
        }

        /// <summary>
        /// Restarts the timer from the beginning.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the timer has been disposed.</exception>
        /// <remarks>
        /// This is equivalent to calling Stop() followed by Start().
        /// </remarks>
        public void Restart()
        {
            EnsureAvailable();
            if (tickRoutine != null && IsRunning)
                Stop();

            Start();
        }

        /// <summary>
        /// Stops the timer countdown.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the timer has been disposed.</exception>
        /// <remarks>
        /// This stops the timer and resets the elapsed time to 0.
        /// Use Start() to restart from the beginning.
        /// </remarks>
        public void Stop()
        {
            EnsureAvailable();
            if (tickRoutine != null)
            {
                runner.StopCoroutine(tickRoutine);
                tickRoutine = null;
            }
            IsRunning = false;
            Elapsed = 0;
        }
        #endregion

        #region Disposal
        /// <summary>
        /// Disposes of the timer and cleans up resources.
        /// </summary>
        /// <remarks>
        /// This method stops the timer, clears all event subscriptions, and cleans up
        /// any temporary GameObjects that were created. After disposal, the timer
        /// cannot be used again.
        /// </remarks>
        public void Dispose()
        {
            if (isDisposed)
                return;

            OnStarted = null;
            OnUpdated = null;
            OnCompleted = null;

            if (tickRoutine != null)
            {
                runner.StopCoroutine(tickRoutine);
                tickRoutine = null;
            }

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
                runnerGameObject = new("Timer Runner");
                runnerGameObject.hideFlags = HideFlags.HideAndDontSave;
                runner = runnerGameObject.AddComponent<TimerRunnerMonoBehaviour>();
            }
        }
        #endregion

        #region Public Helpers
        /// <summary>
        /// Returns a string representation of the timer's elapsed time in MM:SS format.
        /// </summary>
        /// <returns>A formatted string showing minutes and seconds (e.g., "01:30").</returns>
        public override string ToString()
        {
            return ToTimerString(Elapsed);
        }

        /// <summary>
        /// Converts a time value in seconds to a formatted MM:SS string.
        /// </summary>
        /// <param name="seconds">The time value in seconds to format.</param>
        /// <returns>A formatted string showing minutes and seconds (e.g., "01:30").</returns>
        /// <remarks>
        /// Drops hours and only displays MM:SS format. 3601 seconds will be displayed as 01:01.
        /// </remarks>
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
        /// <remarks>
        /// This coroutine runs every frame while the timer is active, updating the elapsed time
        /// and triggering the appropriate events. It automatically handles completion and disposal.
        /// </remarks>
        private IEnumerator Tick()
        {
            Elapsed = 0f;
            OnUpdated?.Invoke(0f);

            // If timerDuration is -1, run indefinitely; otherwise, run until timeElapsedInSeconds >= timerDuration.
            while (Duration == -1f || Elapsed < Duration)
            {
                if (IsRunning)
                {
                    Elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    OnUpdated?.Invoke(Progress); // Pass normalized progress (0 to 1)
                }

                yield return null;
            }

            // Snap to duration
            Elapsed = Duration;
            OnUpdated?.Invoke(1f);

            // Invoke completion callback once the timer ends
            OnCompleted?.Invoke();
            tickRoutine = null;
            IsRunning = false;

            if (autoDispose)
            {
                Dispose(); // Clean up if one-shot timer
            }
        }
        #endregion

        /// <summary>
        /// Internal MonoBehaviour class used to run timer coroutines when no external runner is provided.
        /// </summary>
        private class TimerRunnerMonoBehaviour : MonoBehaviour { }
    }
}
