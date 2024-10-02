using System;
using UniRx;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core.Extensions
{
    public static class EventExtensions
    {
        public static IDisposable Bind(this UnityEvent unityEvent, UnityAction actions)
        {
            unityEvent.AddListener(actions);

            var disposable = new CancellationDisposable();
            disposable.Token.Register(() =>
            {
                unityEvent.RemoveListener(actions);
            });

            return disposable;
        }
        
        public static IDisposable Bind<T>(this UnityEvent<T> unityEvent, UnityAction<T> actions)
        {
            unityEvent.AddListener(actions);

            var disposable = new CancellationDisposable();
            disposable.Token.Register(() =>
            {
                unityEvent.RemoveListener(actions);
            });

            return disposable;
        }
        
        public static IDisposable Bind(this Button button, UnityAction actions)
        {
            button.onClick.AddListener(actions);

            var disposable = new CancellationDisposable();
            disposable.Token.Register(() =>
            {
                button.onClick.RemoveListener(actions);
            });

            return disposable;
        }
    }
}
