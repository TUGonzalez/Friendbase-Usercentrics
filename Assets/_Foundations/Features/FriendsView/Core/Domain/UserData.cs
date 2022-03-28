namespace FriendsView.Core.Domain
{
    public class UserData
    {
        public readonly int userID;
        public readonly string firebaseUid;
        public readonly int gold;
        public readonly string realName;
        public readonly int gems;
        public readonly string username;
        public readonly int friendCount;
        public readonly int friendRequestsCount;
        public readonly bool inPublicArea;
        //public readonly string connectionStatus;

        public UserData(string firebaseUid, int userID, int gems, int gold, string realName, string username,
            int friendCount, int friendRequestsCount, bool inPublicArea)
        {
            this.userID = userID;
            this.gold = gold;
            this.realName = realName;
            this.gems = gems;
            this.username = username;
            this.friendCount = friendCount;
            this.friendRequestsCount = friendRequestsCount;
            this.inPublicArea = inPublicArea;
            this.firebaseUid = firebaseUid;
            //this.connectionStatus = connectionStatus;
        }

        public FriendRequestData ToRequestData(int requestId, RequestStatusOptions requestStatusOptions)
        {
            return new FriendRequestData(requestId, username, realName, userID, firebaseUid, requestStatusOptions,
                friendCount, null);
        }

        public FriendData ToFriendData()
        {
            return new FriendData(userID, firebaseUid, username, realName, inPublicArea, friendCount, gems, gold,
                friendRequestsCount, null,null,null );//Todo complete this
        }
    }
}