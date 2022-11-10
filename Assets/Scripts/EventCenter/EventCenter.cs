using System;
using System.Collections.Generic;

public class EventCenter:Singleton<EventCenter>
{
        private Dictionary<string, List<Action<string, object>>> normalEventDic = new  Dictionary<string, List<Action<string, object>>>();

        protected override void Awake()
        {
                base.Awake();
                DontDestroyOnLoad(this);
        }

        public void SubscribeNormalEvent(string eventId, Action<string, object> action)
        {
                AddNormalEvent(eventId, action);
        }
        
        public void UnSubscribeNormalEvent(string eventId, Action<string, object> action)
        {
                RemoveNormalEvent(eventId, action);
        }

        public void NotifyNormalEvent(string eventId, object arg = null)
        {
                normalEventDic.TryGetValue(eventId, out var list);
                if (list != null && list.Count > 0)
                {
                        foreach (var action in list)
                        {
                                action?.Invoke(eventId, arg);
                        }
                }
        }

        private void AddNormalEvent(string eventId, Action<string, object> action)
        {
                if (!normalEventDic.ContainsKey(eventId))
                {
                        var list = new List<Action<string, object>>();
                        normalEventDic.Add(eventId, list);
                        
                }
                normalEventDic[eventId].Add(action);
        }

        private void RemoveNormalEvent(string eventId, Action<string, object> action)
        {
                normalEventDic.TryGetValue(eventId, out var list);
                if (list.Contains(action))
                {
                        list.Remove(action);
                }
        }
}