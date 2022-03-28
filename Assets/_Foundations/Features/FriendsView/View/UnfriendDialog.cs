using System;
using FriendsView.Core.Domain;
using TMPro;
using UniRx;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace FriendsView.View
{
    public class UnfriendDialog : MonoBehaviour
    {
        public Button confirmBtn;
        public IObservable<Unit> OnConfirmBtn => confirmBtn.OnClickAsObservable();
        
        public TextMeshProUGUI username;
        public TextMeshProUGUI usernameOk;

        public void SetUnfriendBox(FriendData friendData)
        {
            username.SetText(friendData.username);
            usernameOk.SetText(friendData.username);
        }
    }
}