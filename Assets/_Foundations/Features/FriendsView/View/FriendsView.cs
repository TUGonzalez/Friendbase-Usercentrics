using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using Architecture.MVP;
using FriendsView.Core.Domain;
using FriendsView.Presentation;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class FriendsView : WidgetBase, IFriendsView
    {
        public static event Action OnCustomization;
        [SerializeField] Button avatarButton;
        [SerializeField] Button customizationButton;

        [SerializeField] ModalCard card;

        [SerializeField] List<Button> closeButtons;

        [SerializeField] GameObject mainRequestNumberGO;
        [SerializeField] TextMeshProUGUI mainRequestNumber;

        [SerializeField] GameObject occluder;
        [SerializeField] ModalSocial socialList;
        [SerializeField] DialogsLayout dialogsLayout;

        public DialogsLayout DialogsLayout => dialogsLayout;
        public ModalCard Card => card;
        public ModalSocial SocialList => socialList;

        [SerializeField] private NotificationsAnimationsController notificationsAnimator;

        public IObservable<Unit> OnAvatarButton => avatarButton.OnClickAsObservable();
        public IObservable<Unit> OnFriendsButton => card.toFriendsButton.OnClickAsObservable();
        public IObservable<Unit> OnVisitFriendsBtn => card.visitFriendButton.OnClickAsObservable();
        public IObservable<Unit> OnReqCardAcceptBtn => card.acceptRequestButton.OnClickAsObservable();
        public IObservable<Unit> OnReqCardRejectBtn => card.rejectRequestButton.OnClickAsObservable();
        public IObservable<Unit> OnAddFriendButton => card.addFriendButton.OnClickAsObservable();
        public IObservable<Unit> OnUnFriendsButton => card.unfriendButton.OnClickAsObservable();
        public IObservable<Unit> OnRequestButton => notificationsAnimator.requestButton.OnClickAsObservable();
        public IObservable<Unit> OnFriendButton => notificationsAnimator.friendButton.OnClickAsObservable();

        public IEnumerable<IObservable<Unit>> OnCloseButtons =>
            closeButtons.Select(button => button.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnReportButtons =>
            card.reportButtons.Select(button => button.OnClickAsObservable());

        readonly ISubject<Unit> onStart = new Subject<Unit>();
        public IObservable<Unit> OnStart => onStart;

        private List<GameObject> sections = new List<GameObject>();

        private void Start()
        {
            this.CreatePresenter<FriendsPresenter, IFriendsView>();
            onStart.OnNext(Unit.Default);
        }

        void OnEnable()
        {
            customizationButton.onClick.AddListener(() => OnCustomization?.Invoke());
        }

        void OnDisable()
        {
            customizationButton.onClick.RemoveAllListeners();
        }

        public void ShowSection(ViewSection section)
        {
            HideSections();
            occluder.SetActive(true);

            dialogsLayout.ShowSection(section);

            switch (section)
            {
                case ViewSection.PersonalCard:
                    Card.gameObject.SetActive(true);
                    Card.personalSubCard.gameObject.SetActive(true);
                    break;
                case ViewSection.FriendListModal:
                    SocialList.gameObject.SetActive(true);
                    break;
                case ViewSection.FriendCard:
                    Card.gameObject.SetActive(true);
                    Card.friendSubCard.gameObject.SetActive(true);
                    break;
                case ViewSection.FriendReqCard:
                    Card.gameObject.SetActive(true);
                    Card.requestSubCard.gameObject.SetActive(true);
                    Card.requestFooter.gameObject.SetActive(true);
                    break;
                case ViewSection.StrangerCard:
                    Card.gameObject.SetActive(true);
                    Card.requestSubCard.gameObject.SetActive(true);
                    Card.strangerFooter.gameObject.SetActive(true);
                    break;
            }
        }

        //Todo: can those be on an IEnumerable method?
        public void HideSections()
        {
            Card.gameObject.SetActive(false);
            Card.personalSubCard.gameObject.SetActive(false);
            Card.friendSubCard.gameObject.SetActive(false);
            SocialList.gameObject.SetActive(false);
            occluder.SetActive(false);
            DialogsLayout.HideSections();
            Card.requestSubCard.gameObject.SetActive(false);
            Card.requestFooter.gameObject.SetActive(false);
            Card.strangerFooter.gameObject.SetActive(false);
        }

        public void SetPersonalCard(UserData userData)
        {
            Card.SetPersonalCard(userData);
        }

        public void SetFriendCard(FriendData friendData)
        {
            Card.SetFriendCard(friendData);
        }

        public bool IsShowingFriendRows()
        {
            return socialList.IsShowingFriendRows();
        }

        public void SetRequestCard(FriendRequestData data)
        {
            Card.SetReqCard(data);
        }

        public void SetStrangerCard(UserData data, bool requested)
        {
            Card.SetStrangerCard(data, requested);
        }


        public void SetFriendModal(List<FriendData> friend)
        {
            if (friend.Count > 0)
            {
                socialList.SetFriendListButton(true);
                if (!IsShowingFriendRows())
                {
                    SwitchTabs();
                }

                SocialList.SetFriendRows(friend);
            }
            else
            {
                socialList.SetFriendListButton(false);
                if (IsShowingFriendRows())
                {
                    SwitchTabs();
                }
            }
        }

        public void SetFriendListButton(int friendCount)
        {
            if (friendCount > 0)
            {
                socialList.SetFriendListButton(true);
            }
            else
            {
                socialList.SetFriendListButton(false);
            }
        }

        public void SetFriendRequestModal(List<FriendRequestData> friendReq)
        {
            SocialList.SetRequestRows(friendReq);
        }

        public void SwitchTabs()
        {
            SocialList.SwitchTabs();
        }

        public void UpdateAvatarButtonRequestsNumber(int request)
        {
            if (request > 0)
            {
                mainRequestNumberGO.SetActive(true);
                mainRequestNumber.SetText(request.ToString());
            }
            else
            {
                mainRequestNumberGO.SetActive(false);
                mainRequestNumber.SetText("0");
            }
        }

        public void PlayNotification(string username, bool isRequest)
        {
            notificationsAnimator.PlayNotification(username, isRequest);
        }
    }
}