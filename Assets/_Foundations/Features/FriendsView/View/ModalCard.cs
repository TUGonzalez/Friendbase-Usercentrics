using System.Collections.Generic;
using FriendsView.Core.Domain;
using Snapshots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class ModalCard : MonoBehaviour
    {
        const int maxRequestShown = 99;


        public List<Button> reportButtons;
        public GameObject personalSubCard;
        public TextMeshProUGUI realName;
        public TextMeshProUGUI username;
        public TextMeshProUGUI gems;
        public TextMeshProUGUI coins;
        public Button toFriendsButton;

        public SnapshotAvatar snapshotAvatar;
        [SerializeField] GameObject requestCountGO;
        [SerializeField] TextMeshProUGUI requestCount;
        [SerializeField] TextMeshProUGUI friendsCount;

        public Button visitFriendButton;
        public Button unfriendButton;
        public GameObject friendSubCard;
        public GameObject online;
        public GameObject offline;

        public Button acceptRequestButton;
        public Button rejectRequestButton;
        public Button addFriendButton;
        public Button pendingButton;
        public GameObject requestSubCard;
        public GameObject requestFooter;
        public GameObject strangerFooter;
        public GameObject requestOnline;
        public GameObject requestOffline;

        public Sprite visitFriendOnline;
        public Sprite visitFriendOffline;


        public void SetPersonalCard(UserData userData)
        {
            HideSubCars();
            realName.text = userData.realName;
            username.text = userData.username;
            gems.text = userData.gems.ToString();
            coins.text = userData.gold.ToString();
            requestCountGO.SetActive(userData.friendRequestsCount > 0);
            toFriendsButton.gameObject.SetActive(userData.friendRequestsCount > 0 || userData.friendCount > 0);
            requestCount.SetText(userData.friendRequestsCount >= maxRequestShown ? "+" + maxRequestShown : userData.friendRequestsCount.ToString());
            friendsCount.SetText(userData.friendCount + " Friends");
            personalSubCard.SetActive(true);
        }

        public void SetPersonalCardLoading()
        {
            HideSubCars();
            realName.text = "Loading...";
            username.text = "Loading...";
            gems.text = "----";
            coins.text = "----";
            friendsCount.SetText("- Friends");
            personalSubCard.SetActive(true);
        }


        public void SetFriendCardLoading(string username)
        {
            HideSubCars();
            this.username.text = username;
            realName.text = "Loading...";
            friendsCount.SetText("- Friends");

            friendSubCard.SetActive(true);
        }


        public void SetFriendCard(FriendData friendData)
        {
            HideSubCars();
            username.text = friendData.username;
            realName.text = friendData.realName;
            friendsCount.SetText(friendData.friendCount + " Friends");

            if (friendData.inPublicArea)
            {
                visitFriendButton.image.sprite = visitFriendOnline;
                online.SetActive(true);
                offline.SetActive(false);
            }
            else
            {
                visitFriendButton.image.sprite = visitFriendOffline;

                online.SetActive(false);
                offline.SetActive(true);
            }

            friendSubCard.SetActive(true);
        }

        public void SetReqCard(FriendRequestData userData)
        {
            HideSubCars();
            username.text = userData.username;
            realName.text = userData.username;
            friendsCount.SetText(userData.friendCount + " Friends");


            if (true)
            {
                visitFriendButton.image.sprite = visitFriendOnline;
                requestOnline.SetActive(true);
                requestOffline.SetActive(false);
            }
            else
            {
                visitFriendButton.image.sprite = visitFriendOffline;

                requestOnline.SetActive(false);
                requestOffline.SetActive(true);
            }

            requestSubCard.SetActive(true);
            requestFooter.SetActive(true);
        }

        public void SetStrangerCard(UserData userData, bool requested)
        {

            HideSubCars();
            username.text = userData.username;
            realName.text = userData.username;
            friendsCount.SetText(userData.friendCount + " Friends");

            if (true)
            {
                visitFriendButton.image.sprite = visitFriendOnline;
                requestOnline.SetActive(true);
                requestOffline.SetActive(false);
            }
            else
            {
                requestOnline.SetActive(false);
                requestOffline.SetActive(true);
            }

            if (requested)
            {
                pendingButton.gameObject.SetActive(true);
                requestSubCard.SetActive(true);
                strangerFooter.SetActive(true);
            }
            else
            {

                addFriendButton.gameObject.SetActive(true);
                requestSubCard.SetActive(true);
                strangerFooter.SetActive(true);
            }
        }

        public void SendRequest()
        {
            addFriendButton.gameObject.SetActive(false);
            pendingButton.gameObject.SetActive(true);
        }

        void HideSubCars()
        {
            personalSubCard.SetActive(false);
            friendSubCard.SetActive(false);
            requestSubCard.SetActive(false);
            requestFooter.SetActive(false);
            strangerFooter.SetActive(false);
            addFriendButton.gameObject.SetActive(false);
            pendingButton.gameObject.SetActive(false);
        }
    }
}

//Todo: add online && in public area check to allow visiting friend