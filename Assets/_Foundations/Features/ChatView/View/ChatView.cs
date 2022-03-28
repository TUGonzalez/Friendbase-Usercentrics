using System;
using System.Collections;
using Architecture.Injector.Core;
using Architecture.MVP;
using ChatView.Core.Domain;
using ChatView.Infrastructure;
using ChatView.Presentation;
using ResponsiveUtilities;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChatView.View
{
    public class ChatView : WidgetBase, IChatView
    {
        string username;
        const int messagesFieldLimit = 1000;
        public string submitKey = "Submit";

        [SerializeField] GameObject chatUI;
        [SerializeField] Button sendButton;
        [SerializeField] Button closeButton;
        [SerializeField] TMP_InputField userMessage;
        [SerializeField] StringWidget receivedMessages;
        [SerializeField] ChatResponsiveInputUtilities chatResponsive;
        [SerializeField] GameObject welcomeMessage;
        [SerializeField] Color userMessagesColor;

        [SerializeField] ChatAnimationPlayer chatAnimationPlayer;

        RandomColorUtilities randomColor;

        readonly ISubject<Unit> onEnabled = new Subject<Unit>();
        readonly ISubject<Unit> onDisabled = new Subject<Unit>();
        readonly ISubject<Unit> onDisposed = new Subject<Unit>();
        readonly ISubject<Unit> sendButtonSubject = new Subject<Unit>();

        readonly CompositeDisposable disposables = new CompositeDisposable();
        private Image buttonImage;
        string usernameColor;

        public static event Action OnCloseChat;

        void Awake()
        {
            this.CreatePresenter<ChatViewPresenter, IChatView>();

            randomColor = new RandomColorUtilities(ChatServices.GetChatColorsList());
            ColorUtility.TryParseHtmlString(ChatServices.friendbaseBlackHex, out userMessagesColor);

            sendButton
                .OnClickAsObservable()
                .Subscribe(sendButtonSubject).AddTo(disposables);

            closeButton.onClick.AddListener(CloseChat);

            usernameColor = randomColor.RandomColorAsString();
            buttonImage = sendButton.GetComponent<Image>();
        }

        public IObservable<Unit> OnSend => sendButtonSubject;
        public IObservable<Unit> OnEnabled => onEnabled;
        public IObservable<Unit> OnDisabled => onDisabled;
        public IObservable<Unit> OnDisposed => onDisposed;

        public bool IsVisible => chatUI.activeSelf;

        public string UsernameColor => usernameColor;

        private Coroutine OnSubmitCoroutine;

        public string Username
        {
            get => username;
            set => username = value;
        }

        //TODO: restore cinemachine when ready
        public void ShowChat()
        {
            //CinemachineCamera.Singleton.SetCameraChatMode();
            chatAnimationPlayer.ChangeAnimationState(ChatAnimationStates.chat_bubble_static);
            chatUI.SetActive(true);
        }

        public void CloseChat()
        {
            //CinemachineCamera.Singleton.SetCameraNormalMode();
            chatAnimationPlayer.ChangeAnimationState(ChatAnimationStates.chat_bubble_static);
            chatUI.SetActive(false);
            OnCloseChat?.Invoke();
        }

        public void SetMessages(ChatData data)
        {
            //ClearMessagesOverLimit();
            var dataUsername = data.username;
            var dataUsernameColor = data.usernameColor;
            var message = data.message;

            if (message.Equals(""))
            {
                return;
            }

            if (dataUsername.Equals(username))
            {
                receivedMessages.Value += "<b><color=#" + ColorUtility.ToHtmlStringRGBA(userMessagesColor) + ">" +
                                          dataUsername + "</color></b>" + ": " + message + Environment.NewLine;
            }
            else
            {
                if (!chatAnimationPlayer.CurrentAnimationState.Equals(ChatAnimationStates.chat_bubble_idle))
                {
                    chatAnimationPlayer.PlaysNewMessageSequence();
                }

                receivedMessages.Value += "<b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                          dataUsername + "</color></b>" + ": " + message + Environment.NewLine;
            }
        }

        void ClearMessagesOverLimit()
        {
            if (receivedMessages.Value.Length > messagesFieldLimit)
            {
                receivedMessages.Value = receivedMessages.Value.Remove(0, messagesFieldLimit);
            }
        }

        public void SetMessageField()
        {
            //Todo: Remove if and flag, when RoomViewManager loading twice gets fixed if possible 
            if (string.IsNullOrEmpty(ChatServices.ChatHistory)) return;

            receivedMessages.Value = ChatServices.ChatHistory;
            welcomeMessage.SetActive(false);

            if (!ChatServices.ClearHistory)
            {
                ChatServices.ClearHistory = true;
            }
            else
            {
                ChatServices.ClearHistory = false;
                ChatServices.ChatHistory = "";
            }
        }

        public string GetTextToSend()
        {
            var userMessageText = userMessage.text;
            ClearInputField();
            return userMessageText;
        }

        void ClearInputField()
        {
            if (!string.IsNullOrWhiteSpace(userMessage.text)) welcomeMessage.SetActive(false);
            userMessage.text = "";
        }

        void OnEnable()
        {
            userMessage.characterLimit = ChatServices.maxMessageLenght;
            userMessage.onSubmit.AddListener(delegate { Submit(); });
            userMessage.onEndEdit.AddListener(delegate { OnEndEdit(); });
            userMessage.onSelect.AddListener(delegate { chatResponsive.RelocateInputField(true); });

            onEnabled.OnNext(Unit.Default);

            FriendsView.View.FriendsView.OnCustomization += SaveChatHistory;
        }

        void OnDisable()
        {
            userMessage.onSubmit.RemoveAllListeners();
            userMessage.onEndEdit.RemoveAllListeners();
            userMessage.onSelect.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();

            onDisabled.OnNext(Unit.Default);

            FriendsView.View.FriendsView.OnCustomization -= SaveChatHistory;
        }

        private void SaveChatHistory()
        {
            ChatServices.ChatHistory = receivedMessages.Value;
        }

        void OnDestroy()
        {
            disposables.Clear();
            onDisposed.OnNext(Unit.Default);
            closeButton.onClick.RemoveAllListeners();
        }

        void Submit()
        {
            OnSubmitCoroutine = StartCoroutine(SubmitCoroutine());
        }

        void OnEndEdit()
        {
            if (OnSubmitCoroutine != null)
                return; /*If is submitting don't do on end edit behaviour*/
            //ToggleSendButton(true);
            chatResponsive.RelocateInputField(false);
        }

        IEnumerator SubmitCoroutine()
        {
            yield return null; //Wait a frame so keyboard closes
            sendButton.onClick.Invoke();
            yield return null; // wait a frame to reactivate keyboard
            userMessage.ActivateInputField();
            TouchScreenKeyboard.Open("");
            //ToggleSendButton(false);
            OnSubmitCoroutine = null;
        }
    }
}