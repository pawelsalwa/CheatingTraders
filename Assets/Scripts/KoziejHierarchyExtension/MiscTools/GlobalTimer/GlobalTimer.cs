using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AWI
{
    [DefaultExecutionOrder(-2000), ExecuteInEditMode]
    public class GlobalTimer : IndependetSingletonBehaviour<GlobalTimer>
    {

		//static HideFlags hiddenFlags = HideFlags.DontSave;
		static HideFlags hiddenFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;

        public class Updater
        {
            private int _count = 0;
            private double[] _update_intervals_ms = new double[128];
				private double _update_interval_ms = 0;
				private double _update_interval_ticks = 0;
				private double[] _update_intervals_ticks = new double[128];
				private List<Action> _action_list = new List<Action>();

            public bool debugMode = false;

            public List<Action> action_list { get { return _action_list; } }
            public double[] update_intervals_ms { get { return _update_intervals_ms; } }
            public double update_interval_ms { get { return _update_interval_ms; } }
				public double[] update_intervals_ticks { get { return _update_intervals_ticks; } }
				public double update_interval_ticks { get { return _update_interval_ticks; } }
				public int count { get { return _count; } }

            public event Action action
            {
                add
                {
                    if (_action_list.Contains(value))
                        return;
                    _action_list.Add(value);
                    ++_count;
                }
                remove
                {
                    var isRemoved = _action_list.Remove(value);
                    if (isRemoved)
                        --_count;
                }
            }

            private Action _action;
				private System.Diagnostics.Stopwatch _deepStopWatch;
				private System.Diagnostics.Stopwatch _totalStopWatch;

				public void Update()
            {
					 if(_deepStopWatch == null) {
						  _deepStopWatch = new System.Diagnostics.Stopwatch();
					 }
					 if(_totalStopWatch == null) {
						  _totalStopWatch = new System.Diagnostics.Stopwatch();
					 }
					 if (debugMode)
					 {
						  _totalStopWatch.Reset();
						  _totalStopWatch.Start();
					 }
                _action = null as Action;
                for (int i = 0; i < _action_list.Count; ++i)
                {
                    _action = _action_list[i];
                    if (_action == null)
                    {
                        _action_list.RemoveAt(i);
                        --i;
                        continue;
                    }
                    if (debugMode && i < _update_intervals_ms.Length)
                    {
								_deepStopWatch.Reset();
								_deepStopWatch.Start();
                        _action();
								_deepStopWatch.Stop();
                        _update_intervals_ms[i] = _deepStopWatch.ElapsedMilliseconds;
								_update_intervals_ticks[i] = _deepStopWatch.ElapsedTicks;
						  } else {
								_action();
						  }
                }
                if (debugMode)
                {
						  _totalStopWatch.Stop();
						  _update_interval_ms = _totalStopWatch.ElapsedMilliseconds;
						  _update_interval_ticks = _totalStopWatch.ElapsedTicks;
					 }
            }
        }

        const double _minEditorUpdateTime = 1.0f / 60.0f;

        private static Updater _update_high = new Updater();
        private static Updater _update = new Updater();
        private static Updater _tweensUpdate = new Updater();
        private static Updater _update_low = new Updater();
        private static Updater _update_onApplicationNotPlaying = new Updater();
        private static Updater _update_slow = new Updater();

#if UNITY_EDITOR
        public static class InspectiorGetters
        {
            public static Updater update_high { get { return _update_high; } }
            public static Updater update { get { return _update; } }
            public static Updater tweensUpdate { get { return _tweensUpdate; } }
            public static Updater update_low { get { return _update_low; } }
            public static Updater update_onApplicationNotPlaying { get { return _update_onApplicationNotPlaying; } }
            public static Updater update_slow { get { return _update_slow; } }
        }
#endif

        public static bool debug
        {
            set
            {
                _update_high.debugMode = value;
                _tweensUpdate.debugMode = value;
                _update.debugMode = value;
                _update_low.debugMode = value;
                _update_onApplicationNotPlaying.debugMode = value;
                _update_slow.debugMode = value;
            }
        }

		  // check if caller is not null (scripting rebuild issue)
		  public static event Action singleDelayCall;

		  public static event Action update_high
        {
            add { _update_high.action += value; }
            remove { _update_high.action -= value; }
        }
        public static event Action update
        {
            add { _update.action += value; }
            remove { _update.action -= value; }
        }
        public static event Action tweensUpdate
        {
            add { _tweensUpdate.action += value; }
            remove { _tweensUpdate.action -= value; }
        }
        public static event Action update_low
        {
            add { _update_low.action += value; }
            remove { _update_low.action -= value; }
        }
        public static event Action update_onApplicationNotPlaying
        {
            add { _update_onApplicationNotPlaying.action += value; }
            remove { _update_onApplicationNotPlaying.action -= value; }
        }
        public static event Action update_slow
        {
            add { _update_slow.action += value; }
            remove { _update_slow.action -= value; }
        }

        public static int instanceCount = 0;
        public static float fps = 0;

        static bool _initMark = false;

        static private float _currentSlowUpdateInterval = 0;

#if UNITY_EDITOR
		  static double _timeSinceLastUpdate_double = 0;
#endif

		  static float _timeSinceLastUpdate_float = 0;

        static float _realDeltaTime = 0;

        private static float _lastRealFps = 0;

        static int _updateCalledCount = 0;

        static public bool updateEnabled = true;

        static public float realDeltaTime
        {
            get
            {
                if (!_initMark)
                {
                    Init();
                }
                return _realDeltaTime;
            }
            private set
            {
                _realDeltaTime = value;
            }
        }

        static float m_SmoothUnscaledDeltaTime = 0.3f;
        static public float smoothUnscaledDeltaTime
        {
            get
            {
                return m_SmoothUnscaledDeltaTime;
            }
        }

        public static Action update_OnApplicationNotPlaying;

        public static double timeSinceStartup
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return EditorApplication.timeSinceStartup;
                }
                else
                {
#endif
                    return Time.realtimeSinceStartup;
#if UNITY_EDITOR
                }
#endif
            }
        }
#if UNITY_EDITOR
		  static private int repaintTick = -1;
        static private int lastRepaintTick = 0;

		  static public void LowRepaint()
        {
            repaintTick = System.Environment.TickCount;
        }
#endif

		  static float current_deltaTime
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return (float)(EditorApplication.timeSinceStartup - _timeSinceLastUpdate_double);
                }
                else
                {
#endif
                    return Time.realtimeSinceStartup - _timeSinceLastUpdate_float;
#if UNITY_EDITOR
                }
#endif
            }
        }

        static void UpdateTimeSinceLastUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _timeSinceLastUpdate_double = EditorApplication.timeSinceStartup;
            }
            else
            {
#endif
                _timeSinceLastUpdate_float = Time.realtimeSinceStartup;
#if UNITY_EDITOR
            }
#endif
        }

        static bool UpdateDeltaTime()
        {
            realDeltaTime = current_deltaTime;
            _currentSlowUpdateInterval += realDeltaTime;
            UpdateTimeSinceLastUpdate();
            if (realDeltaTime > 0)
            {
                return true;
            }
            return false;
        }

        static bool UpdateSlowInterval()
        {
            if (_currentSlowUpdateInterval < 0.15f)
            {
                return false;
            }
            _currentSlowUpdateInterval = 0;
            return true;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
                EditorApplication.update += EditorUpdate;
            }
            else
            {
#endif
                GlobalTimer.instance = GlobalTimer.instance;
#if UNITY_EDITOR
            }
#endif
            _initMark = true;
            UpdateTimeSinceLastUpdate();
            UpdateDeltaTime();
        }

        static void EditorUpdate()
        {
            if (!Application.isPlaying && current_deltaTime >= _minEditorUpdateTime)
            {
                UpdateTime();
                UpdateSmoothUnscaledTarget();
            }
        }

        void Update()
        {
            if (_updateCalledCount > 0)
            {
                if (Application.isPlaying)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    DestroyImmediate(this.gameObject);
                }
                return;
            }
            _updateCalledCount += 1;
            GlobalTimer.instance = this;
            if (Application.isPlaying)
            {
                UpdateTime();
                UpdateSmoothUnscaledTarget();
            }
        }

        void LateUpdate()
        {
            if (_updateCalledCount != 0)
            {
                instanceCount = _updateCalledCount;
            }
            _updateCalledCount = 0;
        }

        static void UpdateTime()
        {
            if (!UpdateDeltaTime())
            {
                return;
            }
            if (UpdateSlowInterval())
            {
                if (!Application.isPlaying)
                {
#if UNITY_EDITOR
                    if(lastRepaintTick < repaintTick)
                    {
                        //SceneView.RepaintAll();
                        lastRepaintTick = Environment.TickCount;
                    }
#endif
                }
                _update_slow.Update();
            }

            UpdateFps();

            if (Application.isPlaying || updateEnabled)
            {
                UpdateActions();
            }
        }

        static void UpdateActions()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (update_OnApplicationNotPlaying != null)
                {
                    update_OnApplicationNotPlaying();
                }
            }
#endif
				if(singleDelayCall != null) {
					 singleDelayCall();
					 singleDelayCall = null;
				}
            _update_high.Update();
            _update.Update();
            _tweensUpdate.Update();
            _update_low.Update();
        }

        static void UpdateFps()
        {
            float lastRealFps = GlobalTimer._lastRealFps;
            float currentRealFps = 1 / realDeltaTime;
            float fps_delta = Mathf.Abs(currentRealFps - lastRealFps);
            fps = Mathf.Lerp(fps, currentRealFps, fps_delta * realDeltaTime);
            GlobalTimer._lastRealFps = currentRealFps;
        }

        static void UpdateSmoothUnscaledTarget()
        {
            if (Application.isPlaying)
            {
                float smoothUnscaledTarget = Time.unscaledDeltaTime > 0.1f ? 0.1f : Time.unscaledDeltaTime;
                float smoothUnscaledLerpDelta = m_SmoothUnscaledDeltaTime > Time.unscaledDeltaTime ? 0.85f : 0.5f;
                m_SmoothUnscaledDeltaTime = Mathf.Lerp(m_SmoothUnscaledDeltaTime, smoothUnscaledTarget, smoothUnscaledLerpDelta);
            }
            else
            {
                float smoothUnscaledTarget = realDeltaTime > 0.1f ? 0.1f : realDeltaTime;
                float smoothUnscaledLerpDelta = m_SmoothUnscaledDeltaTime > realDeltaTime ? 0.85f : 0.5f;
                m_SmoothUnscaledDeltaTime = Mathf.Lerp(m_SmoothUnscaledDeltaTime, smoothUnscaledTarget, smoothUnscaledLerpDelta);
            }
        }

        protected override void AfterCreateInstance()
        {
            base.AfterCreateInstance();
			gameObject.hideFlags = hiddenFlags;
        }

    }

}
