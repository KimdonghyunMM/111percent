using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public enum GameEventType
{
    HP_REFRESH,
}

public interface IEventListener
{
    void OnEvent(GameEventType eType, Component sender, object param = null);
}

public class GameEventHandler
{
    private static Dictionary<GameEventType, List<IEventListener>> listeners = new Dictionary<GameEventType, List<IEventListener>>();

    public static void AddListener(GameEventType eType, IEventListener listener)
    {
        if (listeners.TryGetValue(eType, out var listenerList))
        {
            listenerList.Add(listener);
            return;
        }

        listenerList = new List<IEventListener>();
        listenerList.Add(listener);
        listeners.Add(eType, listenerList);
    }

    public static void PostNotification(GameEventType eType, Component sender, object param = null)
    {
        if (!listeners.TryGetValue(eType, out var listenList))
            return;

        for (var i = 0; i < listenList.Count; i++)
            listenList?[i].OnEvent(eType, sender, param);
    }

    public static void RemoveEvent(GameEventType eType) => listeners.Remove(eType);

    public void RemoveRedundancies()
    {
        var tempListeners = new Dictionary<GameEventType, List<IEventListener>>();
        foreach (var item in listeners)
        {
            for (var i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Value[i] == null)
                    item.Value.RemoveAt(i);
            }

            if (item.Value.Count > 0)
                tempListeners.Add(item.Key, item.Value);
        }

        listeners = tempListeners;
    }
}
