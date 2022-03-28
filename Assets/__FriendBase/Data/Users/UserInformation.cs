namespace Data.Users
{
    [System.Serializable]
    public class UserInformation
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirebaseId { get; set; }
        public int Gems { get; set; }
        public int Gold { get; set; }

        public string Country { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Photo_url { get; set; }
        public bool Do_avatar_customization { get; set; }

        private AvatarCustomizationData avatarCustomizationData;

        public UserInformation()
        {
            Gems = 0;
            Gold = 0;
        }

        public AvatarCustomizationData GetAvatarCustomizationData()
        {
            if (avatarCustomizationData == null)
            {
                avatarCustomizationData = new AvatarCustomizationData();
            }
            return avatarCustomizationData;
        }

        public void Initialize(UserInformation userInformation)
        {
            UserId = userInformation.UserId;
            UserName = userInformation.UserName;
            FirebaseId = userInformation.FirebaseId;
            Gems = userInformation.Gems;
            Gold = userInformation.Gold;
            Country = userInformation.Country;
            Email = userInformation.Email;
            Gender = userInformation.Gender;
            Photo_url = userInformation.Photo_url;
        }
    }
}
