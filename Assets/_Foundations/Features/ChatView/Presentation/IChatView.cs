using System;
using Architecture.MVP;
using ChatView.Core.Domain;
using UniRx;
using UnityEngine.EventSystems;

namespace ChatView.Presentation
{
    public interface IChatView : IPresentable
    {
        IObservable<Unit> OnSend { get; }
        IObservable<Unit> OnEnabled { get; }
        IObservable<Unit> OnDisabled { get; }
        IObservable<Unit> OnDisposed { get; }
        bool IsVisible { get; }
        string Username { get; set; }
        string UsernameColor { get; }
        void ShowChat();
        void CloseChat();
        void SetMessages(ChatData messageData);
        string GetTextToSend();
    }
}