using FriendsView.Core.Domain;
using Snapshots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class FriendRow : MonoBehaviour
    {
        public FriendData data;
        public Button friendCardButton;
        public Button rowButton;
        public Button visitFriendButton;

        public TextMeshProUGUI usernameText;
        public TextMeshProUGUI connectionStatusText;

        public Image connectionStatusImage;
        public SnapshotAvatar snapshotAvatar;

        public Sprite onlineStatusImage;

        public Sprite offlineStatusImage;
        public Sprite offlineVisitButton;
        public Sprite onlineVisitButton;

        public void SetFriendRow(FriendData friendData)
        {
            data = friendData;

            usernameText.SetText(data.username);
            if (data.inPublicArea)
            {
                //connectionStatusText.SetText("ON LINE");
                connectionStatusText.SetText("IN PUBLIC AREA");
                connectionStatusImage.sprite = onlineStatusImage;
                visitFriendButton.image.sprite = onlineVisitButton;
            }
            else
            {
                //connectionStatusText.SetText("OFF LINE");
                connectionStatusText.SetText("IN PRIVATE AREA");
                connectionStatusImage.sprite = offlineStatusImage;
                visitFriendButton.image.sprite = offlineVisitButton;
            }
            
            snapshotAvatar.CreateSnaphot(null, friendData.avatarCustomizationData);

        }

        public FriendData GetFriendRowData()
        {
            return data;
        }
    }
}