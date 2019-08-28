using System;
using UnityEngine;
using System.Collections.Generic;

namespace AWI
{
    public class PriorityListener
    {
        public System.Action action = null;
        public int priority = 0;
        public bool removeAfterInvoke = false;
    }

    public class EventPriority
    {
        private List<PriorityListener> m_Listeners = new List<PriorityListener>();

        public void AddListener(PriorityListener listener)
        {
            if (listener.action != null)
            {
                m_Listeners.Add(listener);
                Sort();
            }
        }

        public void AddListener(List<PriorityListener> listeners)
        {
            if (listeners == null || listeners.Count < 1)
            {
                return;
            }
            this.m_Listeners.AddRange(listeners);
            Sort();
        }

        void Sort()
        {
            m_Listeners.Sort((a, b) => a.priority.CompareTo(b.priority));
        }

        public void AddListener(Action action, int priority, bool removeAfterInvoke = false)
        {
            if (removeAfterInvoke)
            {
                Action selfRemoveAction = () => { };
                PriorityListener selfRemoveListener = new PriorityListener();
                selfRemoveAction = () =>
                {
                    action(); m_Listeners.Remove(selfRemoveListener);
                };
                selfRemoveListener.action = selfRemoveAction;
                selfRemoveListener.priority = priority;
                m_Listeners.Add(selfRemoveListener);
            }
            else
            {
                m_Listeners.Add(new PriorityListener() { action = action, priority = priority });
            }
            Sort();
        }

        public void RemoveAllListeners()
        {
            m_Listeners.Clear();
        }

        public void Invoke()
        {
            for(int i = 0; i < m_Listeners.Count; ++i)
            {
                var listener = m_Listeners[i];
                listener.action();
                if (listener.removeAfterInvoke)
                {
                    m_Listeners.RemoveAt(i);
                }
            }
        }

    }
}