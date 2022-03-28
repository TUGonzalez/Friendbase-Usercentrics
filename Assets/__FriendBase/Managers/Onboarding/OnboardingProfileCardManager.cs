using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Snapshots;
using Data;
using Architecture.Injector.Core;
using DG.Tweening;
using Data.Users;

namespace Onboarding
{
    public class OnboardingProfileCardManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI TxtUserName;
        [SerializeField] private TextMeshProUGUI TxtName;
        [SerializeField] private TextMeshProUGUI TxtNumberOfFriends;
        [SerializeField] private TextMeshProUGUI TxtGemsAmount;
        [SerializeField] private TextMeshProUGUI TxtGoldAmount;
        [SerializeField] private SnapshotAvatar SnapshotAvatar;
        [SerializeField] private GameObject PersonalSubcard;
        [SerializeField] private GameObject FriendSubcard;
        [SerializeField] private GameObject CloseContainer;
        [SerializeField] private TextMeshProUGUI TxtClose;
        private IGameData gameData;

        public void ShowMyProfile()
        {
            this.gameObject.SetActive(true);

            gameData = Injection.Get<IGameData>();
            TxtUserName.text = gameData.GetUserInformation().UserName;
            TxtName.text = gameData.GetUserInformation().UserName;
            TxtNumberOfFriends.text = "0 Friend";
            TxtGemsAmount.text = gameData.GetUserInformation().Gems.ToString();
            TxtGoldAmount.text = gameData.GetUserInformation().Gold.ToString();
            SnapshotAvatar.CreateSnapshot();
            PersonalSubcard.SetActive(true);
            FriendSubcard.SetActive(false);

            CloseContainer.SetActive(true);
            TxtClose.text = "Close";
            CloseContainer.transform.localScale = new Vector3(0f, 0f, 0f);
            CloseContainer.gameObject.transform.DOScale(1, 0.3f).SetDelay(0.4f).SetEase(Ease.OutExpo);
        }

        public void ShowFriendProfile(AvatarCustomizationData avatarCustomizationData)
        {
            this.gameObject.SetActive(true);

            gameData = Injection.Get<IGameData>();
            TxtUserName.text = "Friendbase";
            TxtName.text = "Your friends from Friendbase";
            TxtNumberOfFriends.text = "220 Friend";

            SnapshotAvatar.CreateSnaphot(null, avatarCustomizationData);
            PersonalSubcard.SetActive(false);
            FriendSubcard.SetActive(true);

            CloseContainer.SetActive(true);
            TxtClose.text = "Close";
            CloseContainer.transform.localScale = new Vector3(0f, 0f, 0f);
            CloseContainer.gameObject.transform.DOScale(1, 0.3f).SetDelay(0.4f).SetEase(Ease.OutExpo);
        }
        
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
