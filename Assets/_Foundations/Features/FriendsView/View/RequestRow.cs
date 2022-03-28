using FriendsView.Core.Domain;
using Snapshots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class RequestRow : MonoBehaviour
    {
        FriendRequestData data;
        
        public Button acceptButton;
        public Button rejectButton;
        public Button rowButton;

        public GameObject requestedState;
        public GameObject acceptedState;

        public TextMeshProUGUI usernameText;
        public SnapshotAvatar snapshotAvatar;

        public void SetFriendRequestRow(FriendRequestData data)
        {
            this.data = data;
            usernameText.SetText(data.username);
            
            snapshotAvatar.CreateSnaphot(null, data.avatarCustomizationData);

        }

        public void AcceptRequest()
        {
            acceptedState.SetActive(true);
            requestedState.SetActive(false);
        }

        public void RejectRequest()
        {
            gameObject.SetActive(false);
        }
        
        public void ResetRow()
        {
            acceptedState.SetActive(false);
            requestedState.SetActive(true);
        }
        

        public FriendRequestData GetFriendRequestData()
        {
            return data;
        }
    }
}