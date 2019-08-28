using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AWI
{
    public static class InvokeExtensions
    {
        public static void AInvokeStop(this MonoBehaviour caller, string tag = "")
        {
            AInvokeManager.instance.Stop(caller, tag);
        }
        public static void AInvokeUnscaled(this MonoBehaviour caller, System.Action action, float delay, string tag = "")
        {
            AInvokeManager.instance.Delay(caller, action, false, delay, tag);
        }
        public static void AInvokeSkipFrame(this MonoBehaviour caller, System.Action action, string tag = "")
        {
            AInvokeManager.instance.SkipFrame(caller, action, false, 1, tag);
        }
        public static void AInvokeSkipFrames(this MonoBehaviour caller, System.Action action, int frames, string tag = "")
        {
            AInvokeManager.instance.SkipFrame(caller, action, false, frames, tag);
        }
        
        public static void AInvokeUnscaledImmortal(this MonoBehaviour caller, System.Action action, float delay, string tag = "")
        {
            AInvokeManager.instance.Delay(caller, action, true, delay, tag);
        }
        public static void AInvokeSkipFrameImmortal(this MonoBehaviour caller, System.Action action, string tag = "")
        {
            AInvokeManager.instance.SkipFrame(caller, action, true, 1, tag);
        }
        public static void AInvokeSkipFramesImmortal(this MonoBehaviour caller, System.Action action, int frames, string tag = "")
        {
            AInvokeManager.instance.SkipFrame(caller, action, true, frames, tag);
        }
    }

    [DefaultExecutionOrder(-1000)]
    public class AInvokeManager : IndependetSingletonBehaviour<AInvokeManager>
    {
        public Dictionary<GameObject, List<MInvoke>> mInvokes_untagged = new Dictionary<GameObject, List<MInvoke>>();
        public Dictionary<string, Dictionary<GameObject, List<MInvoke>>> mInvokes = new Dictionary<string, Dictionary<GameObject, List<MInvoke>>>();

        public class MInvoke
        {
            public bool immortal = false;
            public GameObject caller;
            public Action action;
            public string tag = string.Empty;
            public bool cancel = false;
            public float delay;
            public int frames; 
            public bool skipFrame = false;
            public double startTime = 0;

            public double timeLife
            {
                get
                {
                    return GlobalTimer.timeSinceStartup - startTime;
                }
            }

            public bool delayed
            {
                get
                {
                    return timeLife <= delay;
                }
            }

            private void AddToDictionary(Dictionary<GameObject, List<MInvoke>> dict)
            {

                if (dict.ContainsKey(caller))
                {
                    dict[caller].Add(this);
                }
                else
                {
                    var list = new List<MInvoke>();
                    list.Add(this);
                    dict.Add(caller, list);
                }
            }

            public void Add()
            {
                if (string.IsNullOrEmpty(tag))
                {
                    AddToDictionary(instance.mInvokes_untagged);
                }
                else
                {
                    if (!instance.mInvokes.ContainsKey(tag))
                    {
                        instance.mInvokes.Add(tag, new Dictionary<GameObject, List<MInvoke>>());
                    }
                    AddToDictionary(instance.mInvokes[tag]);
                }
            }

            private void RemoveFromDictionary(Dictionary<GameObject, List<MInvoke>> dict)
            {
                dict[caller].Remove(this);
                if(dict[caller].Count == 0)
                {
                    dict.Remove(caller);
                }
            }
            
            public void Remove()
            {
                if (string.IsNullOrEmpty(tag))
                {
                    RemoveFromDictionary(instance.mInvokes_untagged);
                }
                else
                {
                    if (instance.mInvokes.ContainsKey(tag))
                    {
                        RemoveFromDictionary(instance.mInvokes[tag]);
                    }
                }
            }

        }

        IEnumerator InvokeCoroutine(MInvoke mInvoke)
        {
            while ((mInvoke.delayed  || mInvoke.frames > 0) && !mInvoke.cancel)
            {
                if (mInvoke.frames > 0)
                {
                    yield return null;
                    --mInvoke.frames;
                    continue;
                }
                else
                {
                    yield return null;
                }
            }
            if (mInvoke.action != null && !mInvoke.cancel)
            {
                if (mInvoke.immortal || mInvoke.caller != null)
                {
                    mInvoke.action();
                }
            }
            if(mInvoke.caller != null)
            {
                mInvoke.Remove();
            }
        }
        
        public void Stop(MonoBehaviour monoCaller, string tag = "")
        {
            if (string.IsNullOrEmpty(tag))
            {
                if (mInvokes_untagged.ContainsKey(monoCaller.gameObject))
                {
                    var list = mInvokes_untagged[monoCaller.gameObject];
                    foreach (var v in list)
                    {
                        v.cancel = true;
                    }
                }
            }
            else
            {
                if (mInvokes.ContainsKey(tag))
                {
                    var dict = mInvokes[tag];
                    if (dict.ContainsKey(monoCaller.gameObject))
                    {
                        var list = dict[monoCaller.gameObject];
                        foreach(var v in list)
                        {
                            v.cancel = true;
                        }
                    }
                }
            }
        }

        public void Delay(MonoBehaviour monoCaller, Action action, bool immortal, float time, string tag = "")
        {
            Invoke(monoCaller.gameObject, action, immortal, time, 0, tag);
        }

        public void SkipFrame(MonoBehaviour monoCaller, Action action, bool immortal, int frames, string tag = "")
        {
            Invoke(monoCaller.gameObject, action, immortal, 0, frames, tag);
        }

        public void Invoke(GameObject caller, Action action, bool immortal, float delay, int frames, string tag = "")
        {
            MInvoke mInvoke = new MInvoke();
            mInvoke.caller = caller;
            mInvoke.delay = delay;
            mInvoke.action = action;
            mInvoke.frames = frames;
            mInvoke.immortal = immortal;
            mInvoke.startTime = GlobalTimer.timeSinceStartup;
            mInvoke.Add();
            StartCoroutine(InvokeCoroutine(mInvoke));
        }

        void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;
        }


        static int _instanceCount = 0;
        public static int instanceCount = 0;

        void Update()
        {
            if (_instanceCount == 1)
            {
                if (Application.isPlaying)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    DestroyImmediate(this.gameObject);
                }
            }
            else
            {
                _instanceCount += 1;
                instance = this;
            }
        }

        private void LateUpdate()
        {
            if (_instanceCount != 0)
            {
                instanceCount = _instanceCount;
            }
            _instanceCount = 0;
        }

    }

}
