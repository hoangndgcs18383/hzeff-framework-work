using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// High-performance event dispatching system with minimal allocations
/// </summary>
public static class EventDispatcher
{
    // Using a nested dictionary for efficient two-level lookup
    private static readonly Dictionary<string, Dictionary<int, Action<object>>> _eventHandlers = 
        new Dictionary<string, Dictionary<int, Action<object>>>(32);
    
    // Object pool to reduce GC allocations
    private static readonly Queue<Dictionary<int, Action<object>>> _dictionaryPool = 
        new Queue<Dictionary<int, Action<object>>>(8);

    /// <summary>
    /// Registers an event listener with automatic key generation
    /// </summary>
    public static void Register(string eventId, Action<object> callback)
    {
        if (callback == null || callback.Target == null) return;
        
        var target = callback.Target;
        var key = GetKey(target);
        
        RegisterWithKey(eventId, key, callback);
    }

    /// <summary>
    /// Registers an event listener with a specific key
    /// </summary>
    public static void RegisterWithKey(string eventId, int key, Action<object> callback)
    {
        if (callback == null) return;

        Dictionary<int, Action<object>> handlers;
        if (!_eventHandlers.TryGetValue(eventId, out handlers))
        {
            handlers = GetDictionaryFromPool();
            _eventHandlers[eventId] = handlers;
        }

        handlers[key] = callback;
    }

    /// <summary>
    /// Posts an event to all registered listeners
    /// </summary>
    public static void Post(string eventId, object param = null)
    {
        Dictionary<int, Action<object>> handlersCopy = null;
        
        lock (_eventHandlers)
        {
            if (_eventHandlers.TryGetValue(eventId, out var handlers))
            {
                // Create a copy of the handlers to avoid modification during iteration
                handlersCopy = new Dictionary<int, Action<object>>(handlers);
            }
        }

        if (handlersCopy != null)
        {
            foreach (var handler in handlersCopy.Values)
            {
                try
                {
                    handler?.Invoke(param);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Event handler failed for {eventId}: {ex}");
                }
            }
        }
    }

    /// <summary>
    /// Unregisters all listeners for a specific key
    /// </summary>
    public static void Unregister(int key)
    {
        // Create list of event IDs to avoid modifying collection during iteration
        var eventIds = new List<string>(_eventHandlers.Keys);

        foreach (var eventId in eventIds)
        {
            var handlers = _eventHandlers[eventId];
            if (handlers.Remove(key) && handlers.Count == 0)
            {
                _eventHandlers.Remove(eventId);
                ReturnDictionaryToPool(handlers);
            }
        }
    }

    /// <summary>
    /// Unregisters a specific listener for an event
    /// </summary>
    public static void Unregister(string eventId, int key)
    {
        if (_eventHandlers.TryGetValue(eventId, out var handlers))
        {
            if (handlers.Remove(key) && handlers.Count == 0)
            {
                _eventHandlers.Remove(eventId);
                ReturnDictionaryToPool(handlers);
            }
        }
    }

    /// <summary>
    /// Clears all event listeners
    /// </summary>
    public static void Clear()
    {
        foreach (var handlers in _eventHandlers.Values)
        {
            ReturnDictionaryToPool(handlers);
        }
        _eventHandlers.Clear();
    }

    #region Helper Methods

    private static Dictionary<int, Action<object>> GetDictionaryFromPool()
    {
        return _dictionaryPool.Count > 0 ? 
            _dictionaryPool.Dequeue() : 
            new Dictionary<int, Action<object>>(8);
    }

    private static void ReturnDictionaryToPool(Dictionary<int, Action<object>> dictionary)
    {
        dictionary.Clear();
        _dictionaryPool.Enqueue(dictionary);
    }

    public static int GetKey(object target)
    {
        return target.GetHashCode();
    }

    #endregion
}

/// <summary>
/// MonoBehaviour extensions for convenient event handling
/// </summary>
public static class EventDispatcherExtensions
{
    /// <summary>
    /// Registers an event listener using the MonoBehaviour instance as the key
    /// </summary>
    public static void RegisterEvent(this MonoBehaviour sender, string eventId, Action<object> callback)
    {
        if (sender == null || callback == null) return;
        EventDispatcher.RegisterWithKey(eventId, sender.GetInstanceID(), callback);
    }

    /// <summary>
    /// Unregisters all events for this MonoBehaviour instance
    /// </summary>
    public static void UnregisterEvents(this MonoBehaviour sender)
    {
        if (sender == null) return;
        EventDispatcher.Unregister(sender.GetInstanceID());
    }

    /// <summary>
    /// Unregisters a specific event for this MonoBehaviour instance
    /// </summary>
    public static void UnregisterEvent(this MonoBehaviour sender, string eventId)
    {
        if (sender == null) return;
        EventDispatcher.Unregister(eventId, sender.GetInstanceID());
    }

    /// <summary>
    /// Posts an event with optional parameter
    /// </summary>
    public static void PostEvent(this MonoBehaviour sender, string eventId, object param = null)
    {
        EventDispatcher.Post(eventId, param);
    }

    /// <summary>
    /// Posts an event without parameters
    /// </summary>
    public static void PostEvent(this MonoBehaviour sender, string eventId)
    {
        EventDispatcher.Post(eventId);
    }
}