using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Spikescape.Utility.Threading
{
    /// <summary>
    /// A MonoBehaviour that can be used to dispatch actions to the main Unity thread.
    /// </summary>
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> _actions = new();
        private static int _mainThreadId;
        private static bool _initialized = false;

        private void Awake()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }

            _initialized = true;
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            while (_actions.TryDequeue(out var action))
            {
                try { action?.Invoke(); }
                catch (Exception ex) { Debug.LogException(ex); }
            }
        }

        public static void Run(Action action)
        {
            if (action == null) { return; }

            if (!_initialized)
            {
                Debug.LogWarning("MainThreadDispatcher not initialized yet. Running action immediately.");
                action();
                return;
            }

            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId) { action(); }
            else { _actions.Enqueue(action); }
        }
    }
}
