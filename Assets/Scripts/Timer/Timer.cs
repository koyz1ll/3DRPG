﻿using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

using Object = UnityEngine.Object;

/// <summary>
/// Allows you to run events on a delay without the use of <see cref="Coroutine"/>s
/// or <see cref="MonoBehaviour"/>s.
/// 
/// To create and start a Timer, use the <see cref="Register"/> method.
/// </summary>
public class Timer
{
#region Public Properties/Fields

    /// <summary>
    /// How long the timer takes to complete from start to finish.
    /// </summary>
    public float duration { get; private set; }

    /// <summary>
    /// Whether the timer will run again after completion.
    /// </summary>
    public bool isLooped { get; set; }

    /// <summary>
    /// Whether or not the timer completed running. This is false if the timer was cancelled.
    /// </summary>
    public bool isCompleted { get; private set; }

    /// <summary>
    /// Whether the timer uses real-time or game-time. Real time is unaffected by changes to the timescale
    /// of the game(e.g. pausing, slow-mo), while game time is affected.
    /// </summary>
    public bool usesRealTime { get; private set; }

    /// <summary>
    /// Whether the timer is currently paused.
    /// </summary>
    public bool isPaused { get { return this._timeElapsedBeforePause.HasValue; } }

    /// <summary>
    /// Whether or not the timer was cancelled.
    /// </summary>
    public bool isCancelled { get { return this._timeElapsedBeforeCancel.HasValue; } }

    /// <summary>
    /// Get whether or not the timer has finished running for any reason.
    /// </summary>
    public bool isDone { get { return this.isCompleted || this.isCancelled || this.isOwnerDestroyed; } }

    public object param => _param;

#endregion

#region Public Static Methods

    /// <summary>
    /// Register a new timer that should fire an event after a certain amount of time
    /// has elapsed.
    /// 
    /// Registered timers are destroyed when the scene changes.
    /// </summary>
    /// <param name="duration">The time to wait before the timer should fire, in seconds.</param>
    /// <param name="onTimerInvoke">An action to fire when the timer completes. It's parameter is real delta time
    /// since last once</param>
    /// <param name="isLooped">Whether the timer should repeat after executing.</param>
    /// <param name="useRealTime">Whether the timer uses real-time(i.e. not affected by pauses,
    /// slow/fast motion) or game-time(will be affected by pauses and slow/fast-motion).</param>
    /// <param name="autoDestroyOwner">An object to attach this timer to. After the object is destroyed,
    /// the timer will expire and not execute. This allows you to avoid annoying <see cref="NullReferenceException"/>s
    /// by preventing the timer from running and accessessing its parents' components
    /// after the parent has been destroyed.</param>
    /// <returns>A timer object that allows you to examine stats and stop/resume progress.</returns>
    public static Timer Register(float duration,
                                 Action<float, object> onTimerInvoke,
                                 bool isLooped = false,
                                 bool useRealTime = false,
                                 MonoBehaviour autoDestroyOwner = null,
                                 object param = null)
    {
        // create a manager object to update all the timers if one does not already exist.
        if (Timer._manager == null)
        {
            TimerManager managerInScene = Object.FindObjectOfType<TimerManager>();
            if (managerInScene != null)
            {
                Timer._manager = managerInScene;
            }
            else
            {
                GameObject managerObject = new GameObject {name = "TimerManager"};
                Timer._manager = managerObject.AddComponent<TimerManager>();
            }
        }

        return Timer._manager.RegisterTimer(duration, onTimerInvoke, isLooped, useRealTime, autoDestroyOwner, param);
    }

    /// <summary>
    /// Cancels a timer. The main benefit of this over the method on the instance is that you will not get
    /// a <see cref="NullReferenceException"/> if the timer is null.
    /// </summary>
    /// <param name="timer">The timer to cancel.</param>
    public static void Cancel(Timer timer)
    {
        if (timer != null)
        {
            timer.Cancel();
        }
    }

    /// <summary>
    /// Pause a timer. The main benefit of this over the method on the instance is that you will not get
    /// a <see cref="NullReferenceException"/> if the timer is null.
    /// </summary>
    /// <param name="timer">The timer to pause.</param>
    public static void Pause(Timer timer)
    {
        if (timer != null)
        {
            timer.Pause();
        }
    }

    /// <summary>
    /// Resume a timer. The main benefit of this over the method on the instance is that you will not get
    /// a <see cref="NullReferenceException"/> if the timer is null.
    /// </summary>
    /// <param name="timer">The timer to resume.</param>
    public static void Resume(Timer timer)
    {
        if (timer != null)
        {
            timer.Resume();
        }
    }

    public static void CancelAllRegisteredTimers()
    {
        if (Timer._manager != null)
        {
            Timer._manager.CancelAllTimers();
        }

        // if the manager doesn't exist, we don't have any registered timers yet, so don't
        // need to do anything in this case
    }

#endregion

#region Only for TimerManager used

    /// <summary>
    /// Please don't call this function, it's only for TimerManager
    /// </summary>
    public void Initialize(float duration,
                           Action<float, object> onTimerInvoke,
                           bool isLooped,
                           bool usesRealTime,
                           MonoBehaviour autoDestroyOwner,
                           object param)
    {
        Reset();
        this.duration = duration;
        this._onTimerInvoke = onTimerInvoke;

        this.isLooped = isLooped;
        this.usesRealTime = usesRealTime;

        this._autoDestroyOwner = autoDestroyOwner;
        this._hasAutoDestroyOwner = autoDestroyOwner != null;

        this._startTime = this.GetWorldTime();
        this._lastUpdateTime = this._startTime;
        this._lastFireTime = this._startTime;

        this._param = param;
    }

    /// <summary>
    /// Please don't call this function, it's only for TimerManager
    /// </summary>
    public void Reset()
    {
        this.isCompleted = false;

        this.duration = 0f;
        this._onTimerInvoke = null;

        this.isLooped = false;
        this.usesRealTime = false;


        this._autoDestroyOwner = null;
        this._hasAutoDestroyOwner = false;

        this._startTime = 0f;
        this._lastUpdateTime = 0f;
        this._lastFireTime = 0f;

        this._timeElapsedBeforeCancel = null;
        this._timeElapsedBeforePause = null;
    }

#endregion

#region Public Methods

    /// <summary>
    /// Stop a timer that is in-progress or paused. The timer's on completion callback will not be called.
    /// </summary>
    public void Cancel()
    {
        if (this.isDone)
        {
            return;
        }

        this._timeElapsedBeforeCancel = this.GetTimeElapsed();
        this._timeElapsedBeforePause = null;
    }

    /// <summary>
    /// Pause a running timer. A paused timer can be resumed from the same point it was paused.
    /// </summary>
    public void Pause()
    {
        if (this.isPaused || this.isDone)
        {
            return;
        }

        this._timeElapsedBeforePause = this.GetTimeElapsed();
    }

    /// <summary>
    /// Continue a paused timer. Does nothing if the timer has not been paused.
    /// </summary>
    public void Resume()
    {
        if (!this.isPaused || this.isDone)
        {
            return;
        }

        this._timeElapsedBeforePause = null;
    }

    /// <summary>
    /// Get how many seconds have elapsed since the start of this timer's current cycle.
    /// </summary>
    /// <returns>The number of seconds that have elapsed since the start of this timer's current cycle, i.e.
    /// the current loop if the timer is looped, or the start if it isn't.
    /// 
    /// If the timer has finished running, this is equal to the duration.
    /// 
    /// If the timer was cancelled/paused, this is equal to the number of seconds that passed between the timer
    /// starting and when it was cancelled/paused.</returns>
    public float GetTimeElapsed()
    {
        if (this.isCompleted || this.GetWorldTime() >= this.GetFireTime())
        {
            return this.duration;
        }

        return this._timeElapsedBeforeCancel ?? this._timeElapsedBeforePause ?? this.GetWorldTime() - this._startTime;
    }

    /// <summary>
    /// Get how many seconds remain before the timer completes.
    /// </summary>
    /// <returns>The number of seconds that remain to be elapsed until the timer is completed. A timer
    /// is only elapsing time if it is not paused, cancelled, or completed. This will be equal to zero
    /// if the timer completed.</returns>
    public float GetTimeRemaining()
    {
        return this.duration - this.GetTimeElapsed();
    }

    /// <summary>
    /// Get how much progress the timer has made from start to finish as a ratio.
    /// </summary>
    /// <returns>A value from 0 to 1 indicating how much of the timer's duration has been elapsed.</returns>
    public float GetRatioComplete()
    {
        return this.GetTimeElapsed() / this.duration;
    }

    /// <summary>
    /// Get how much progress the timer has left to make as a ratio.
    /// </summary>
    /// <returns>A value from 0 to 1 indicating how much of the timer's duration remains to be elapsed.</returns>
    public float GetRatioRemaining()
    {
        return this.GetTimeRemaining() / this.duration;
    }

#endregion

#region Private Static Properties/Fields

    // responsible for updating all registered timers
    private static TimerManager _manager;

#endregion

#region Private Properties/Fields

    private bool isOwnerDestroyed { get { return this._hasAutoDestroyOwner && this._autoDestroyOwner == null; } }

    private Action<float, object> _onTimerInvoke;
    private float _startTime;
    private float _lastFireTime;
    private float _lastUpdateTime;

    // for pausing, we push the start time forward by the amount of time that has passed.
    // this will mess with the amount of time that elapsed when we're cancelled or paused if we just
    // check the start time versus the current world time, so we need to cache the time that was elapsed
    // before we paused/cancelled
    private float ? _timeElapsedBeforeCancel;
    private float ? _timeElapsedBeforePause;

    // after the auto destroy owner is destroyed, the timer will expire
    // this way you don't run into any annoying bugs with timers running and accessing objects
    // after they have been destroyed
    private MonoBehaviour _autoDestroyOwner;
    private bool _hasAutoDestroyOwner;

    private object _param;

#endregion

#region Private Methods

    private float GetWorldTime()
    {
        return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetFireTime()
    {
        return this._startTime + this.duration;
    }

    private float GetTimeDelta()
    {
        return this.GetWorldTime() - this._lastUpdateTime;
    }

    private void Update()
    {
        if (this.isDone)
        {
            return;
        }

        if (this.isPaused)
        {
            this._startTime += this.GetTimeDelta();
            this._lastUpdateTime = this.GetWorldTime();
            return;
        }

        var worldTime = this.GetWorldTime();
        this._lastUpdateTime = worldTime;
        float deltaTime = worldTime - this.GetFireTime();
        if (deltaTime >= 0)
        {
            if (this._onTimerInvoke != null)
            {
                this._onTimerInvoke(worldTime - this._lastFireTime, this._param);
            }

            if (this.isLooped)
            {
                this._startTime = worldTime - deltaTime;
                this._lastFireTime = worldTime;
            }
            else
            {
                this.isCompleted = true;
            }
        }
    }

#endregion

#region Manager Class (implementation detail, spawned automatically and updates all registered timers)

    /// <summary>
    /// Manages updating all the <see cref="Timer"/>s that are running in the application.
    /// This will be instantiated the first time you create a timer -- you do not need to add it into the
    /// scene manually.
    /// </summary>
    private class TimerManager : MonoBehaviour
    {
        private List<Timer> _timers = new List<Timer>();

        // buffer adding timers so we don't edit a collection during iteration
        private List<Timer> _timersToAdd = new List<Timer>();

        private void Awake()
        {
            Object.DontDestroyOnLoad(this.gameObject);
        }

        private void RegisterTimerInternal(Timer timer)
        {
            this._timersToAdd.Add(timer);
        }

        public void CancelAllTimers()
        {
            for (var index = 0; index < this._timers.Count; index++)
            {
                Timer timer = this._timers[index];
                timer.Cancel();
            }

            for (var index = 0; index < this._timersToAdd.Count; index++)
            {
                Timer timer = this._timersToAdd[index];
            }

            this._timers.Clear();
            this._timersToAdd.Clear();
        }

        // update all the registered timers on every frame
        [UsedImplicitly]
        private void Update()
        {
            this.UpdateAllTimers();
        }

        private void UpdateAllTimers()
        {
            if (this._timersToAdd.Count > 0)
            {
                this._timers.AddRange(this._timersToAdd);
                this._timersToAdd.Clear();
            }

            for (var index = 0; index < this._timers.Count; ++index)
            {
                Timer timer = this._timers[index];
                timer.Update();

                if (!timer.isDone) continue;

                this._timers.RemoveAt(index);
                --index;
                
            }
        }

        public Timer RegisterTimer(float timer,
                                   Action<float, object> onTimerInvoke,
                                   bool isLooped,
                                   bool useRealTime,
                                   MonoBehaviour autoDestroyOwner,
                                   object param)
        {
            var timerInstance = new Timer();
            timerInstance.Initialize(timer, onTimerInvoke, isLooped, useRealTime, autoDestroyOwner, param);
            RegisterTimerInternal(timerInstance);

            return timerInstance;
        }
    }

#endregion
}
