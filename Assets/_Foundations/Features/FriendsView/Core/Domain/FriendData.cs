using Data.Users;

namespace FriendsView.Core.Domain
{
    public class FriendData
    {
        public readonly string username;
        public readonly string realName;
        public readonly string fireBaseUID;
        public readonly int userID;
        public readonly int gems;
        public readonly int gold;
        //public readonly bool online;
        public readonly int friendCount;
        public readonly int friendRequestsCount;
        public readonly bool inPublicArea;
        public readonly string chatRoomId;
        public readonly string chatRoomName;
        public readonly AvatarCustomizationData avatarCustomizationData;
        
        public FriendData(int userID, string fireBaseUid, string username, string realName, bool inPublicArea,
            int friendCount, int gems, int gold, int friendRequestsCount, string chatRoomId, string chatRoomName, AvatarCustomizationData avatarCustomizationData )
        {
            this.username = username;
            this.realName = realName;
            this.userID = userID;
            //this.online = online;
            this.inPublicArea = inPublicArea;
            this.friendCount = friendCount;
            this.gems = gems;
            this.gold = gold;
            this.friendRequestsCount = friendRequestsCount;
            this.avatarCustomizationData = avatarCustomizationData;
            this.chatRoomId = chatRoomId;
            this.chatRoomName = chatRoomName;
            fireBaseUID = fireBaseUid;
        }

        public UserData ToUserData()
        {
            return new UserData(fireBaseUID, userID, gems, gold, realName, username, friendCount, friendRequestsCount,
                inPublicArea);
        }
    }
}